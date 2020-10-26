using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public or private ref and type (int, float, bool, string) for variable
    [SerializeField]
    private float _speed = 5.5f;
    private float _normalSpeed = 5.5f;
    private float _thrustSpeed = 10.0f;
    private float _boostSpeed = 15.0f;
    [SerializeField] //before a private variable allows for adjusting value in unity editor
    private float _fireRate = 0.15f;
    private float _canFire = -1f;
    [SerializeField]
    private int _score = 0;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private GameObject _LaserPrefab;
    private SpawnManager _spawnManager;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _fireballPrefab;
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;
    private UIManager _uiManager;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private GameObject _leftEngine;
    [SerializeField]
    private GameObject _rightEngine;
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _explosionAudio;
    [SerializeField]
    private AudioClip _laserClip;
    [SerializeField]
    private AudioClip _outOfAmmoClip;
    [SerializeField]
    private AudioClip _fireballClip;
    [SerializeField]
    private AudioClip _shieldHitClip;
    [SerializeField]
    private float _powerupDuration = 10f;
    private bool _isDamageAvail = true;
    private GameManager _gameManager;
    private int _pointCount = 1;
    private int _ammoCount = 15;
    private SpriteRenderer _shieldRenderer;
    private int _shieldHit = 0;
    private int _shieldMaxHealth = 2;
    private bool _isThrustAvail = true;
    private GameObject _thrusters;
    private float _startTime = 0f;
    private float _holdTime = 5.0f;
    private bool _isThrustCool = true;
    private bool _haveFireball = false;
    private CamShake _camera;
    private Powerup _powerup;
    private bool _isPlayerFrozen = false;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("The Game Manager is Null");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is Null");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is Null");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Explode Source on Player is Null");
        }

        _shieldRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();
        if (_shieldRenderer == null)
        {
            Debug.LogError("Shield SpriteRenderer on Player is Null");
        }

        _thrusters = transform.GetChild(1).gameObject;
        if (_thrusters == null)
        {
            Debug.LogError("thrusters on Player is Null");
        }

        _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CamShake>();
        if (_camera == null)
        {
            Debug.LogError("Main Camera on Player is Null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        FireLaser();
    }

    private void CamShake(float shakeDuration)
    {
        _camera.Shake(shakeDuration);
    }

    public void Damage()
    {
        if (_isDamageAvail)
        {
            _audioSource.clip = _explosionAudio;
            _audioSource.volume = .25f;
            _audioSource.Play();
            if (!_isShieldActive)
            {
                CamShake(2f);
                _lives -= 1;
                _uiManager.UpdateLives(_lives);
                
                switch (_lives)
                {
                    case 2:
                        _leftEngine.SetActive(true);
                        _rightEngine.SetActive(false);
                        break;
                    case 1:
                        _rightEngine.SetActive(true);
                        break;
                    case 0:
                        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                        Destroy(this.gameObject);
                        _spawnManager.OnPlayerDeath();
                        break;
                }
            }
            else
            {
                CamShake(.25f);
                // Shield Strength Phase 1
                _shieldHit++; 
                _audioSource.clip = _shieldHitClip;
                _audioSource.volume = .25f;
                _audioSource.Play();

                switch (_shieldHit)
                {
                    case 3:
                        _isShieldActive = false;
                        _shieldVisualizer.SetActive(false);
                        break;
                    case 2:
                        _shieldRenderer.color = Color.red;
                        break;
                    case 1:
                        _shieldRenderer.color = Color.white;
                        break;
                }
            }
            StartCoroutine(DamagePowerDownRoutine());
        }
    }

    IEnumerator DamagePowerDownRoutine()
    {
        _isDamageAvail = false;
        yield return new WaitForSeconds(2.0f);
        _isDamageAvail = true;
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void SpeedBoostActive()
    {
        if (!_isPlayerFrozen)
        {
            _isSpeedBoostActive = true;
            _speed = _boostSpeed;
            _thrusters.transform.localPosition = new Vector3(0, -6.5f);
            _thrusters.transform.localScale = new Vector3(1, 3, 1);
            StartCoroutine(SpeedPowerDownRoutine());
        }        
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldRenderer.color = Color.yellow;
        _shieldVisualizer.SetActive(true);
        _shieldHit = 0;
    }

    public void ReloadAmmo()
    {
        _ammoCount = 15;
    }

    public void ExtraLife()
    {
        _lives++;
        switch (_lives)
        {
            case 3:
                _leftEngine.SetActive(false);
                break;
            case 2:
                _leftEngine.SetActive(true);
                _rightEngine.SetActive(false);
                break;
            default:
                break;
        }

        _uiManager.UpdateLives(_lives);
    }
        
    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(_powerupDuration);
        _isTripleShotActive = false;
    }

    IEnumerator SpeedPowerDownRoutine()
    {
        yield return new WaitForSeconds(_powerupDuration);
        _isSpeedBoostActive = false;
        _speed = _normalSpeed;
        _thrusters.transform.localPosition = new Vector3(0, -3.2f);
        _thrusters.transform.localScale = new Vector3(1, 1, 1);
    }

    IEnumerator FrozenPlayerPowerDownRoutine()
    {
        yield return new WaitForSeconds(_powerupDuration / 2);
        _thrusters.GetComponent<SpriteRenderer>().color = Color.white;
        _isPlayerFrozen = false;
        
    }

    IEnumerator ThrusterAvail()
    {
        //disable thrusters even though button may still be down
        yield return new WaitForSeconds(5f);
        DisableThrusters();
    }

    IEnumerator ThrusterCoolDown()
    {
        yield return new WaitForSeconds(10f);
        _isThrustCool = true;
        _isThrustAvail = true;
    }

    void Thrusters()
    {
        // Thrusters Phase 1
        if (Input.GetKeyDown(KeyCode.LeftShift) && !_isSpeedBoostActive && _isThrustCool && !_isPlayerFrozen)
        {

            _isThrustCool = false;
            StartCoroutine(ThrusterAvail());

        }
        if (Input.GetKey(KeyCode.LeftShift) && !_isSpeedBoostActive && _isThrustAvail && !_isPlayerFrozen)
        {
            _thrusters.transform.localPosition = new Vector3(0, -5);
            _thrusters.transform.localScale = new Vector3(1, 2, 1);
            _speed = _thrustSpeed;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && !_isSpeedBoostActive && _isThrustAvail && !_isPlayerFrozen)
        {
            DisableThrusters();
        }
    }

    void DisableThrusters()
    {
        _thrusters.transform.localPosition = new Vector3(0, -3.2f);
        _thrusters.transform.localScale = new Vector3(1, 1, 1);
        _speed = _normalSpeed;
        _isThrustCool = false;
        _isThrustAvail = false;
        StartCoroutine(ThrusterCoolDown());
    }

    void FireLaser()
    {
        // Ammo Count Phase 1
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _ammoCount > 0)
        {
            AudioSource.PlayClipAtPoint(_laserClip, transform.position);
            _canFire = Time.time + _fireRate;
            _ammoCount--;
            if (_isTripleShotActive)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity); //quaternion.identity = default rotation
            }
            else
            {
                Instantiate(_LaserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity); //quaternion.identity = default rotation
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space) && _ammoCount == 0)
        {
            _audioSource.clip = _outOfAmmoClip;
            _audioSource.volume = 0.05f;
            _audioSource.Play();
        }
        if (Input.GetKeyDown(KeyCode.Z) && _haveFireball)
        {
            _audioSource.clip = _fireballClip;
            _audioSource.volume = 0.5f;
            _audioSource.Play();
            Instantiate(_fireballPrefab, transform.position + new Vector3(0, 1.75f, 0), Quaternion.identity);
            _haveFireball = false;
            _uiManager.UpdateFireball(_haveFireball);
        }
        _uiManager.UpdateAmmo(_ammoCount);
    }

    public void Fireball()
    {
        _haveFireball = true;
        _uiManager.UpdateFireball(_haveFireball);
    }
    
    public void FreezePlayer()
    {
        _isPlayerFrozen = true;
        _thrusters.GetComponent<SpriteRenderer>().color = Color.blue;
    }

    void CalculateMovement()
    {
        float xMax = 15;
        if (!_isPlayerFrozen)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * _speed * Time.deltaTime);

            if (transform.position.y >= 5)
            {
                transform.position = new Vector3(transform.position.x, 5, 0);
            }
            else if (transform.position.y <= -5f)
            {
                transform.position = new Vector3(transform.position.x, -5f, 0);
            }

            if (transform.position.x >= xMax)
            {
                transform.position = new Vector3(-xMax, transform.position.y, 0);
            }
            else if (transform.position.x <= -xMax)
            {
                transform.position = new Vector3(xMax, transform.position.y, 0);
            }

            Thrusters();

        }
        else
        {
            StartCoroutine(FrozenPlayerPowerDownRoutine());
        }

    }

    public void SetScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public int GetScore()
    {
        return _score;
    }
}
