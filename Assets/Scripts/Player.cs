using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public or private ref and type (int, float, bool, string) for variable
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
    private float _powerupDuration = 10f;
    private bool _isDamageAvail = true;
    private GameManager _gameManager;
    private int _pointCount = 1;
    private int _ammoCount = 15;
    private SpriteRenderer _shieldRenderer;
    private int _shieldHit = 0;
    private int _shieldMaxHealth = 2;

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
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        FireLaser();
        _uiManager.UpdateAmmo(_ammoCount);
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
                _lives -= 1;
                Debug.Log("Lives Left = " + _lives);

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
                _uiManager.UpdateLives(_lives);
            }
            else
            {
                // Shield Strength Phase 1
                _shieldHit++;
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
                Debug.Log("Shield Hit");
                
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
        _isSpeedBoostActive = true;
        _speed = _boostSpeed;
        StartCoroutine(SpeedPowerDownRoutine());
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
            _audioSource.volume = 0.15f;
            _audioSource.Play();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); //user input horizontal control
        float verticalInput = Input.GetAxis("Vertical");

        //transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime); // can use transform.Translate(new Vector3(1, 0, 0)) to move right also
        //Time.deltaTime is real time seconds

       // transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);

        transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * _speed * Time.deltaTime); //efficient way to do

        //if y pos > 0 then y = 0

        //can use transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
        else if (transform.position.y <= -5f)
        {
            transform.position = new Vector3(transform.position.x, -5f, 0);
        }

        if (transform.position.x >= 11f)
        {
            transform.position = new Vector3(-11f, transform.position.y, 0);
        }
        else if (transform.position.x <= -11f)
        {
            transform.position = new Vector3(11f, transform.position.y, 0);
        }

        // Thrusters Phase 1
        if (Input.GetKey(KeyCode.LeftShift) && !_isSpeedBoostActive)
        {
            _speed = _thrustSpeed;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && !_isSpeedBoostActive)
        {
            _speed = _normalSpeed;
        }
    }

    public void SetScore(int points)
    {
        _score += points;
        if (_score % (500 / _pointCount) == 0)
        {
            _lives = 3;
            _pointCount++;
            _leftEngine.SetActive(false);
            _rightEngine.SetActive(false);
            _uiManager.UpdateLives(3);
        }
        _uiManager.UpdateScore(_score);
    }

    public int GetScore()
    {
        return _score;
    }
}
