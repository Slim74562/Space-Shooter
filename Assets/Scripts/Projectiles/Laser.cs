using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5;
    private bool _isEnemyLaser = false;
    private bool _isWheelLaser = false;
    private GameObject _player;
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
    }

    void WheelMove()
    {
        transform.Translate(_translateDirection * _speed * Time.deltaTime);
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    public void SetLaserType(string type)
    {
        GetComponent<Projectile>().SetEnemyProjectile(true);
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
}
