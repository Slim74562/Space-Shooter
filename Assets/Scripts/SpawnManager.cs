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
    private float _minPowerupSpawnTime = 3.0f;
    private float _maxPowerupSpawnTime = 5.0f;
    private bool _stopSpawning = true;
    private int _powerupCount = 4;
    private int _lastIndex;
    private int _rareCount;
    [SerializeField]
    private GameObject[] _enemyPrefabs;
    [SerializeField]
    private GameObject _bossPrefab;
    [SerializeField]
    private GameObject _asteroidPrefab;
    private int _waveCount = 5;
    private int _maxEnemyCount = 0;
    private int _enemyCount = 0;
    private bool _isPlayerDead = false;
    private bool _keepPowerupSpawn = false;

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

    public bool GetSpawnStatus()
    {
        return _stopSpawning;
    }

    IEnumerator SpawnEnemyRoutine()  //using IEnumerator allows for use of yield keyword ** Coroutine
    {

        GameObject newEnemy;
        yield return new WaitForSeconds(1.5f);
        if (_waveCount == 5)
        {
            _enemyCount++;
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 4, 0);
            newEnemy = Instantiate(_bossPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            _keepPowerupSpawn = true;
        }
        else
        {
            _maxEnemyCount = 5 * _waveCount;
            while (!_stopSpawning)
            {
                _enemyCount++;
                if (_enemyCount <= _maxEnemyCount)
                {
                    Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 5, 0);
                    int randomNum = Random.Range(0, (12 - _waveCount));
                    if (randomNum == 0 && _waveCount > 1)
                    {
                        newEnemy = Instantiate(_enemyPrefabs[2], posToSpawn, Quaternion.identity);
                    }
                    else if (randomNum == 1 && _waveCount > 2)
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
                    _keepPowerupSpawn = true;
                }
            }
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            while (enemies.Length != 0)
            {
                yield return new WaitForSeconds(.5f);
                enemies = GameObject.FindGameObjectsWithTag("Enemy");
            }
            if (!_isPlayerDead)
            {
                _keepPowerupSpawn = false;
                _waveCount++;
                Text waveText = GameObject.Find("WaveText").GetComponent<Text>();
                waveText.text = "Wave Complete";
                waveText.enabled = true;
                yield return new WaitForSeconds(1.5f);
                waveText.enabled = false;
                Instantiate(_asteroidPrefab, new Vector3(0, 4.5f, 0), Quaternion.identity);
            }
            StopCoroutine(SpawnPowerupRoutine());
            StopCoroutine(SpawnEnemyRoutine());
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(9.0f);
        while (!_stopSpawning || _keepPowerupSpawn)
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
                if (_rareCount % 3 == 0)
                {
                    _powerupCount++;
                }
            }
            else
            {
                _rareCount++;
                if (_rareCount % 3 == 0)
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
        _isPlayerDead = true;
        GameObject[] enemies, powerups, lasers;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        powerups = GameObject.FindGameObjectsWithTag("Powerup");
        lasers = GameObject.FindGameObjectsWithTag("Projectile");
        if (enemies == null || powerups == null)
        {
            Debug.Log("GameObject Array in SpawnManager is null");
        }
        foreach (GameObject enemy in enemies)
        {
            if (enemy.name.Equals("Boss(Clone)"))
            {
                enemy.GetComponent<Boss>().KillBoss();
            }
            else
            {
                enemy.GetComponent<Enemy>().KillEnemy();
            }
        }
        enemies = null;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies != null)
        {
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<Enemy>().KillEnemy();
            }
        }
        foreach (GameObject powerup in powerups)
        {
            Destroy(powerup);
        }
        foreach (GameObject laser in lasers)
        {
            Destroy(laser);
        }
    }
}
