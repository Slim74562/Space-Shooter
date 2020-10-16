using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] powerups;
    [SerializeField]
    private float _minEnemySpawnTime = 3.0f;
    [SerializeField]
    private float _maxEnemySpawnTime = 5.0f;
    [SerializeField]
    private float _minPowerupSpawnTime = 5.0f;
    [SerializeField]
    private float _maxPowerupSpawnTime = 10.0f;
    private bool _stopSpawning = false;
    private int _powerupCount = 0;
    private int _maxPowerupCount = 5;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()  //using IEnumerator allows for use of yield keyword ** Coroutine
    {
        yield return new WaitForSeconds(3.0f);
        while(!_stopSpawning)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 5, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(Random.Range(_minEnemySpawnTime, _maxEnemySpawnTime));
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(Random.Range(_minPowerupSpawnTime, _maxPowerupSpawnTime));
        while (!_stopSpawning)
        {
            int randomPowerUp = Random.Range(0, powerups.Length);
            if (randomPowerUp == 5 && _powerupCount < _maxPowerupCount)
            {
                _powerupCount++;
                randomPowerUp = Random.Range(0, powerups.Length - 1);
            }
            else if (randomPowerUp == 5 && _powerupCount >= _maxPowerupCount)
            {
                _powerupCount = 0;
            }
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 5, 0);
            Instantiate(powerups[randomPowerUp], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(_minPowerupSpawnTime, _maxPowerupSpawnTime));
        }
    }

    public void OnPlayerDeath()
    {        
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
        foreach (GameObject powerup in _powerups)
        {
            Destroy(powerup);
        }
        _stopSpawning = true;
    }
}
