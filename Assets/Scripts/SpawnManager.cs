using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    private float _minEnemySpawnTime = 3.0f;
    private float _maxEnemySpawnTime = 5.0f;
    private float _minPowerupSpawnTime = 5f;
    private float _maxPowerupSpawnTime = 10.0f;
    private bool _stopSpawning = false;
    private int _powerupCount = 4;
    private int _lastIndex;
    private int _rareCount;
    [SerializeField]
    private GameObject[] _enemyPrefabs;
    [SerializeField]
    private GameObject _asteroidPrefab;
    private int _waveCount = 3;
    private int _maxEnemyCount = 0;
    private int _enemyCount = 0;

    public void StartSpawning()
    {
        _stopSpawning = false;
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    public int GetWaveCount()
    {
        return _waveCount;
    }

    IEnumerator SpawnEnemyRoutine()  //using IEnumerator allows for use of yield keyword ** Coroutine
    {
        yield return new WaitForSeconds(1.5f);
        _maxEnemyCount = 7 * _waveCount;
        while (!_stopSpawning)
        {
            _enemyCount++;
            if (_enemyCount <= _maxEnemyCount)
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-10f, 10f), 5, 0);
                GameObject newEnemy;
                if (Random.Range(0,1) == 0 && _waveCount > 2)
                {
                    newEnemy = Instantiate(_enemyPrefabs[2], posToSpawn, Quaternion.identity);
                }
                if (Random.Range(0, 3) == 0 && _waveCount > 1)
                {
                    newEnemy = Instantiate(_enemyPrefabs[1], posToSpawn, Quaternion.identity);
                }
                else
                {
                    newEnemy = Instantiate(_enemyPrefabs[0], posToSpawn, Quaternion.identity);
                }
                newEnemy.transform.parent = _enemyContainer.transform;
                yield return new WaitForSeconds(Random.Range(_minEnemySpawnTime, _maxEnemySpawnTime));
            }
            else
            {
                _enemyCount = 0;
                _stopSpawning = true;       
            }            
        }        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");        
        while (enemies.Length != 0)
        {
            Debug.Log("Enemies left: " + enemies.Length);
            yield return new WaitForSeconds(.5f);
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
        }
        _waveCount++;
        Text waveText = GameObject.Find("WaveText").GetComponent<Text>();
        waveText.text = "Wave Complete";
        waveText.enabled = true;
        yield return new WaitForSeconds(1f);
        waveText.enabled = false;
        Instantiate(_asteroidPrefab, new Vector3(0, 4.5f, 0), Quaternion.identity);
        StopCoroutine(SpawnPowerupRoutine());
        StopCoroutine(SpawnEnemyRoutine());
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(9.0f);
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
