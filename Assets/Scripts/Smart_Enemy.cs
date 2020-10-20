using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smart_Enemy : MonoBehaviour
{
    private Player _player;
    private Rigidbody2D _rigidbody;
    private float _originalSpeed = 0.5f;
    private float _speed = 0.5f;
    private float _movementWait = 3;
    private bool _isMoving = true;
    [SerializeField]
    private GameObject _explosionPrefab;
    private int _enemyScoreValue = 25;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player on Smart Enemy is Null");
        }

        _rigidbody = GetComponent<Rigidbody2D>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody2d on Smart Enemy is Null");
        }

        StartCoroutine(MovementCoolDown());
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
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

    private void CalculateMovement()
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

    public void KillEnemy()
    {
        _speed = 0;
        transform.tag = "Dying";
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
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

        if (other.tag == "Laser" && !other.GetComponent<Laser>().IsEnemyLaser())
        {
            KillEnemy();
            if (_player != null)
            {
                _player.SetScore(_enemyScoreValue);
            }
        }
    }
}
