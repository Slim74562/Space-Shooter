﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private float _minEnemySpawnTime = 3.0f;
    [SerializeField]
    private float _maxEnemySpawnTime = 5.0f;
    [SerializeField]
    private float _minPowerupSpawnTime = 10.0f;
    [SerializeField]
    private float _maxPowerupSpawnTime = 20.0f;
    private bool _stopSpawning = false;
    private int _powerupCount = 4;
    private int _lastIndex;
    private int _rareCount;
    private int _enemyCount = 0;
    [SerializeField]
    private GameObject _smartEnemyPrefab;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()  //using IEnumerator allows for use of yield keyword ** Coroutine
    {
        yield return new WaitForSeconds(3.0f);
        while (!_stopSpawning)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-10f, 10f), 5, 0);
            GameObject newEnemy;
            if (_enemyCount > 2)
            {
                newEnemy = Instantiate(_smartEnemyPrefab, posToSpawn, Quaternion.identity);
                _enemyCount = 0;
            }
            else
            {
                newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            }
            _enemyCount++;
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(Random.Range(_minEnemySpawnTime, _maxEnemySpawnTime));
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(Random.Range(_minPowerupSpawnTime, _maxPowerupSpawnTime));
        while (!_stopSpawning)
        {
            int randomPowerUp = Random.Range(0, _powerupCount);
            while (_lastIndex == randomPowerUp)
            {
                randomPowerUp = Random.Range(0, _powerupCount);
            }
            _lastIndex = randomPowerUp;

            if (_powerupCount < _powerups.Length)
            {
                _rareCount++;
                if (_rareCount % 5 == 0)
                {
                    _powerupCount++;
                }
            }
            else
            {
                _rareCount++;
                if (_rareCount % 5 == 0)
                {
                    _rareCount = 0;
                    _powerupCount = 4;
                }
            }
            Vector3 posToSpawn = new Vector3(Random.Range(-10f, 10f), 5, 0);
            Instantiate(_powerups[randomPowerUp], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(_minPowerupSpawnTime, _maxPowerupSpawnTime));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
        GameObject[] _enemies, _powerups;
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        _powerups = GameObject.FindGameObjectsWithTag("Powerup");
        if (_enemies == null || _powerups == null)
        {
            Debug.Log("GameObject Array in SpawnManager is null");
        }
        foreach (GameObject enemy in _enemies)
        {
            enemy.GetComponent<Enemy>().KillEnemy();
        }
        _enemies = null;
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (_enemies != null)
        {
            foreach (GameObject enemy in _enemies)
            {
                enemy.GetComponent<Enemy>().KillEnemy();
            }
        }
        foreach (GameObject powerup in _powerups)
        {
            Destroy(powerup);
        }
    }
}
