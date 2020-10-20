using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _explosionPrefab;
    private Player _player;
    private Collider2D _collider2d;
    private float _speed = 1;
    [SerializeField]
    private float _maxFireRate = 5.0f;
    [SerializeField]
    private float _minFireRate = 3.0f;
    private float _fireRate;
    private float _canFire = 1;
    private bool _isDead = false;
    private bool _playerDead = false;
    [SerializeField]
    private AudioClip _explosionAudio;
    private AudioSource _audioSource;
    [SerializeField]
    private int _enemyScoreValue;
    [SerializeField]
    private AudioClip _laserClip;
    [SerializeField]
    private AudioClip _shieldHitClip;
    [SerializeField]
    private GameObject _shieldVisualizer;
    private int _enemyRare = 3;
    private bool _isShieldActive = false;
    private string _name;
    private bool _isMoving = true;
    private float _movementWait = 3f;
    private float _originalSpeed = 0.5f;
    private Rigidbody2D _rigidbody;
    private bool _isSmartEnemy = false;
    private bool _isRegularEnemy = true;

    // Start is called before the first frame update
    void Start()
    {
        _name = transform.name;

        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
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

        if (_enemyRare == Random.Range(0, 5))
        {
            _shieldVisualizer.SetActive(true);
            _isShieldActive = true;
        }
        
        if (_name == "Smart_Enemy")
        {
            StartCoroutine(MovementCoolDown());
            _isSmartEnemy = true;
            _isRegularEnemy = false;
            _collider2d = GetComponent<CircleCollider2D>();            
        } 
        else if (_name == "Enemy")
        {
            _collider2d = GetComponent<BoxCollider2D>();            
        }

        if (_collider2d == null)
        {
            Debug.LogError("Collider2D on " + _name + " is null");
        }

        _rigidbody = GetComponent<Rigidbody2D>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody on " + _name + " is null");
        }
    }

    private IEnumerator MovementCoolDown()
    {
        while (_isMoving)
        {
            yield return new WaitForSeconds(_movementWait);
            _speed = 0;
            yield return new WaitForSeconds(_movementWait);
            _speed = _originalSpeed;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (_isRegularEnemy)
        {
            RegularEnemyMovement();
        }
        else if (_isSmartEnemy)
        {
            SmartEnemyMovement();
        }
    }

    private void SmartEnemyMovement()
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

    private void RegularEnemyMovement()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);
        if (transform.position.x > 10)
        {
            _speed = -_speed;
            transform.position = new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z);
        }
        else if (transform.position.x < -10)
        {
            _speed = -_speed;

            transform.position = new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z);
        }

        FireRegularLaser();

        if (transform.position.y < -6.5f)
        {
            transform.position = new Vector3(Random.Range(-10f, 10f), 6.5f, 0);
        }
    }

    public bool IsPlayerDead()
    {
        _playerDead = true;
        return _playerDead;
    }

    void FireRegularLaser()
    {
        if (Time.time > _canFire && !_isDead)
        {
            _fireRate = Random.Range(_minFireRate, _maxFireRate);
            _canFire = Time.time + _fireRate;
            GameObject _Laser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            _Laser.transform.parent = transform;
            AudioSource.PlayClipAtPoint(_laserClip, transform.position);
            Laser[] lasers = _Laser.GetComponentsInChildren<Laser>();
            lasers[0].EnemyLaser();
            lasers[1].EnemyLaser();
        }
    }

    public void KillEnemy()
    {
        if (!_isShieldActive)
        {
            _audioSource.Play();
            _collider2d.enabled = false;
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
        if (other.tag == "Player")
        {
            if (_player != null)
            {
                _player.Damage();
                _player.SetScore(_enemyScoreValue);
            }
            KillEnemy();
        }

        if (other.tag == "Laser")
        {
            if (!other.GetComponent<Laser>().IsEnemyLaser())
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
