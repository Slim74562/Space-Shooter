﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private float _speed = 3;
    private GameObject[] _enemies;
    private GameObject _closestEnemy;
    private Vector3 _direction;

    // Start is called before the first frame update
    void Start()
    {
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (_enemies.Length != 0)
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
        else
        {
            Debug.LogError("Enemy array on Homing Missile is null");
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
            _direction = _closestEnemy.transform.position - transform.position;
            transform.Translate(_direction * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
        }
    }   
}