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
    private float _minPowerupSpawnTime = 20.0f;
    [SerializeField]
    private float _maxPowerupSpawnTime = 60.0f;
    private bool _stopSpawning = false;

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
        yield return new WaitForSeconds(10.0f);
        while (!_stopSpawning)
        {            
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 5, 0);
            int randomPowerUp = Random.Range(0, powerups.Length);
            Instantiate(powerups[randomPowerUp], posToSpawn, Quaternion.identity);
            float randomWait = Random.Range(_minPowerupSpawnTime, _maxPowerupSpawnTime);
            Debug.Log("Powerup Wait: " + randomWait);
            yield return new WaitForSeconds(randomWait);
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
            Destroy(enemy);
        }
        foreach (GameObject powerup in _powerups)
        {
            Destroy(powerup);
        }
        _stopSpawning = true;
    }
}
