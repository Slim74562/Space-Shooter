using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5;
    [SerializeField]
    private GameObject _explosionPrefab; 
    private bool _isEnemyLaser = false;
    private bool _isWheelLaser = false;
    private GameObject _player;
    private float _yBounds = 8f;
    private float _xBounds = 12f;
    private Vector3 _translateDirection;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogError("Player on Laser is null");
        } 
        else
        {
            float xDirection = 0.25f, yDirection = 0.25f;
            int xWhole = 0, yWhole = 0;

            if (_player.transform.position.x < transform.position.x || _player.transform.position.x > transform.position.x)
            {
                xDirection = _player.transform.position.x - transform.position.x;
                xWhole = (int)xDirection / 1;
                if (xWhole != 0)
                {
                    xDirection -= xWhole;
                }
            }

            if (_player.transform.position.y < transform.position.y || _player.transform.position.y > transform.position.y)
            {
                yDirection = _player.transform.position.y - transform.position.y;
                yWhole = (int)yDirection / 1;
                if (yWhole != 0)
                {
                    yDirection -= yWhole;
                }
            }

            _translateDirection = new Vector3(xDirection, yDirection);
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isEnemyLaser)
        {
            MoveDown();
        }
        else if (_isWheelLaser)
        {
            WheelMove();
        }
        else
        {
            MoveUp();
        }
        CheckBounds();
    }

    void CheckBounds()
    {
        if (transform.position.x > _xBounds || transform.position.x < -_xBounds)
        {
            Destroy(gameObject);
        }
        if (transform.position.y > _yBounds || transform.position.y < -_yBounds)
        {
            Destroy(gameObject);
        }
    }

    void WheelMove()
    {
        transform.Translate(_translateDirection * _speed * Time.deltaTime);
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if (transform.position.y >= _yBounds)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= -_yBounds)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
    }

    public bool IsEnemyLaser()
    {
        if (_isEnemyLaser)
        {
            return true;
        } 
        else if (_isWheelLaser)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetLaserType(string type)
    {
        switch (type)
        {
            case "Enemy(Clone)":
                {
                    _isEnemyLaser = true;
                    _isWheelLaser = false;
                    break;
                }
            case "Wheel_Enemy(Clone)":
                {
                    _isWheelLaser = true;
                    _isEnemyLaser = false;
                    break;
                }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(IsEnemyLaser())
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Player>().Damage();

                Destroy(gameObject);
            }

            if (other.CompareTag("Powerup"))
            {
                Instantiate(_explosionPrefab, other.transform.position, Quaternion.identity);
                Destroy(other.gameObject);
                Destroy(gameObject);
            }

        }
    }
}
