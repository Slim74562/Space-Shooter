using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private float _yBounds = 8f;
    private float _xBounds = 12f;
    private float _speed = 3;
    private GameObject[] _enemies;
    private GameObject _closestEnemy;

    // Start is called before the first frame update
    void Start()
    {
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (_enemies != null)
        {
            _closestEnemy = _enemies[0];
            float leastDistance = Mathf.Infinity;

            foreach (GameObject enemy in _enemies)
            {
                float distance = Vector3.Distance(enemy.transform.position, transform.position);
                if (distance < leastDistance)
                {
                    _closestEnemy = enemy;
                    leastDistance = distance;

                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {

        if (_closestEnemy != null)
        {
            Vector3 direction = _closestEnemy.transform.position - transform.position;
            transform.Translate(direction * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
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
}
