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
    private GameObject _explosionPrefab;

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
        Destroy(this.gameObject, 2.5f);

    }
}
