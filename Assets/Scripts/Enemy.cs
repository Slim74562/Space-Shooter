using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _LaserPrefab;
    private Player _player;
    private Animator _anim;
    private BoxCollider2D _coll;
    private float _speed = 1;
    private float _fireRate = 5.0f;
    private float _canFire = 1;    
    private bool _isDead = false;
    private bool _playerDead = false;
    [SerializeField]
    private AudioClip _explosionAudio;
    private AudioSource _audioSource;
    [SerializeField]
    private int _enemyScoreValue = 10;
    [SerializeField]
    private AudioClip _laserClip;


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is Null");
        }

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("Animator is Null");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Audio Source on Enemy is Null");
        }
        else
        {
            _audioSource.clip = _explosionAudio;
            _audioSource.volume = .25f;
        }        
        _coll = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        FireLaser();

        if (transform.position.y < -5.0f)
        {
            transform.Translate(new Vector3(Random.Range(-8f, 8f), 6.5, 0));
        }
    }

    public bool IsPlayerDead()
    {
        _playerDead = true;
        return _playerDead;
    }

    void FireLaser()
    {
        if (Time.time > _canFire && !_isDead)
        {            
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject _Laser =  Instantiate(_LaserPrefab, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(_laserClip, transform.position);
            Laser[] lasers = _Laser.GetComponentsInChildren<Laser>();
            lasers[0].EnemyLaser();
            lasers[1].EnemyLaser();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {        
        if (other.tag == "Player")
        {
            _audioSource.Play();
            if (_player != null)
            {
                _player.Damage();
                _player.SetScore(_enemyScoreValue);
            }
            _coll.enabled = false;
            _isDead = true;
            _speed = 0;
            _anim.SetTrigger("OnEnemyDeath");
            Destroy(this.gameObject, 2.5f);
        }

        if (other.tag == "Laser")
        {
            if (!other.GetComponent<Laser>().GetLaserType())
            {
                _audioSource.Play();
                Destroy(other.gameObject);
                _coll.enabled = false; //can use Destroy(GetComponent<_coll>)
                _isDead = true;
                _speed = 0;
                _anim.SetTrigger("OnEnemyDeath");
                Destroy(this.gameObject, 2.5f);
                if (_player != null)
                {
                    _player.SetScore(_enemyScoreValue);
                }
            }
            
        }
    }
}
