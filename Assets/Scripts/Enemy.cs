using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyLaserPrefab;
    [SerializeField]
    private GameObject _wheelBulletPrefab;
    [SerializeField]
    private GameObject _explosionPrefab;
    private Player _player;
    private float _speed = 1;
    private float _maxFireRate = 5.0f;
    private float _minFireRate = 3.0f;
    private float _fireRate;
    private float _canFire = -1;
    private bool _isDead = false;
    [SerializeField]
    private AudioClip _explosionAudio;
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _laserClip;
    [SerializeField]
    private AudioClip _shieldHitClip;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private int _enemyScoreValue;
    private int _enemyRare = 3;
    private bool _isShieldActive = false;
    private string _name;
    private bool _isMoving = true;
    private float _movementWait = 3f;
    private float _originalSpeed = 0.5f;
    private Rigidbody2D _rigidbody;
    private bool _isKamakaziEnemy = false;
    private bool _isRegularEnemy = false;
    private bool _isWheelEnemy = false;
    private BoxCollider2D _boxCollider2D;
    private CircleCollider2D _circleCollider2D;
    private int _vertical = 1;
    private int _horizontal = 1;
    private float _xBounds = 9f;
    private float _yBounds = 6f;

    // Start is called before the first frame update
    private void Start()
    {
        _name = transform.name;

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player on " + _name + " is Null");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Audio Source on " + _name + " is Null");
        }
        else
        {
            _audioSource.clip = _explosionAudio;
            _audioSource.volume = .25f;
        }

        switch (_name)
        {
            case "Wheel_Enemy(Clone)":
                _isWheelEnemy = true;
                _isKamakaziEnemy = false;
                _isRegularEnemy = false;
                _circleCollider2D = GetComponent<CircleCollider2D>();
                if (_circleCollider2D == null)
                {
                    Debug.LogError("Circle Collider2D on Wheel Enemy is null");
                }
                break;
            case "Kamakazi_Enemy(Clone)":
                StartCoroutine(MovementCoolDown());
                _isKamakaziEnemy = true;
                _isRegularEnemy = false;
                _isWheelEnemy = false;
                _circleCollider2D = GetComponent<CircleCollider2D>();
                if (_circleCollider2D == null)
                {
                    Debug.LogError("Circle Collider2D on Kamakazi Enemy is null");
                }
                break;
            case "Enemy(Clone)":
                _isRegularEnemy = true;
                _isKamakaziEnemy = false;
                _isWheelEnemy = false;
                _boxCollider2D = GetComponent<BoxCollider2D>();
                if (_boxCollider2D == null)
                {
                    Debug.LogError("BoxCollider2D on Enemy is null");
                }
                break;
        }

        if (_enemyRare == Random.Range(0, 5) && GameObject.FindGameObjectWithTag("Respawn").GetComponent<SpawnManager>().GetWaveCount() > 2)
        {
            _shieldVisualizer.SetActive(true);
            _isShieldActive = true;
        }

        _rigidbody = GetComponent<Rigidbody2D>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody on " + _name + " is null");
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isRegularEnemy)
        {
            RegularEnemyMovement();
        }
        else if (_isKamakaziEnemy)
        {
            KamakaziEnemyMovement();
        }
        else if (_isWheelEnemy)
        {
            WheelMovement();
        }
    }

    private void KamakaziEnemyMovement()
    {
        if (_player != null)
        {
            Vector2 direction = _player.transform.position - transform.position;
            _rigidbody.velocity = (direction * _speed);
        }
        else
        {
            _rigidbody.velocity = Vector3.down * _speed;
        }

    }

    private IEnumerator MovementCoolDown()
    {
        while (_isMoving)
        {
            yield return new WaitForSeconds(_movementWait);
            _speed = 0;
            yield return new WaitForSeconds(_movementWait);
            _speed = _originalSpeed / 2;
        }

    }

    private void RegularEnemyMovement()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);
        if (transform.position.x > _xBounds)
        {
            _speed = -_speed;
            transform.position = new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z);
        }
        else if (transform.position.x < -_xBounds)
        {
            _speed = -_speed;

            transform.position = new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z);
        }

        if (transform.position.y < -_yBounds)
        {
            transform.position = new Vector3(Random.Range(-_xBounds + 0.25f, _xBounds - 0.25f), _yBounds - 0.25f, 0);
        }

        FireRegularLaser();
    }

    private void WheelMovement()
    {
        
        if (transform.position.x > _xBounds)
        {
            _horizontal = -1;
        }
        else if (transform.position.x < -_xBounds)
        {
            _horizontal = 1;

        }

        if (transform.position.y < -_yBounds)
        {
            _vertical = 1;
        }
        else if (transform.position.y > _yBounds)
        {
            _vertical = -1;
        }
        transform.Translate(new Vector3(_horizontal, _vertical, 0) * _speed * Time.deltaTime);
        WheelFire();
    }

    private void WheelFire()
    {
        if (Time.time > _canFire && !_isDead)
        {
            _fireRate = Random.Range(_minFireRate, _maxFireRate);
            _canFire = Time.time + _fireRate;

            if (Random.Range(0, 3) == 1)
            {
                GameObject bullet = Instantiate(_wheelBulletPrefab, transform.position, Quaternion.identity);
                bullet.GetComponent<Projectile>().SetEnemyProjectile(true);
                AudioSource.PlayClipAtPoint(_laserClip, transform.position);
            }
            else
            {
                GameObject laser = Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
                laser.GetComponent<Laser>().SetLaserType(_name);
                AudioSource.PlayClipAtPoint(_laserClip, transform.position);
            }
        }
    }

    void FireRegularLaser()
    {
        if (Time.time > _canFire && !_isDead)
        {
            _fireRate = Random.Range(_minFireRate, _maxFireRate);
            _canFire = Time.time + _fireRate;
            GameObject laser = Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(_laserClip, transform.position);
            if (_isRegularEnemy)
            {
                Laser[] lasers = laser.GetComponentsInChildren<Laser>();
                lasers[0].SetLaserType(_name);
                lasers[1].SetLaserType(_name);
            }
            
        }
    }

    public void KillEnemy()
    {
        if (!_isShieldActive)
        {
            _isDead = true;
            _speed = 0;
            transform.tag = "Dying";
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        else
        {
            AudioSource.PlayClipAtPoint(_shieldHitClip, transform.position);
            _shieldVisualizer.SetActive(false);
            _isShieldActive = false;
        }
    }

    public void ClearShield()
    {
        _isShieldActive = false;
        _shieldVisualizer.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();
                _player.SetScore(_enemyScoreValue);
            }
            KillEnemy();
        }

        if (other.CompareTag("Projectile"))
        {
            if (!other.GetComponent<Projectile>().IsEnemyProjectile())
            {
                Destroy(other.gameObject);
                if (_player != null && !_isShieldActive)
                {
                    _player.SetScore(_enemyScoreValue);
                }
                KillEnemy();
            }
        }
    }
}
