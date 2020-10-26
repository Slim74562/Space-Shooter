using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 25f;
    [SerializeField]
    private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;
    private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if(_spawnManager == null)
        {
            Debug.LogError("Spawn Manager on Asteroid is Null");
        }
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player on Asteroid is Null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    private void DestroyAsteroid()
    {
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        if (_spawnManager.GetSpawnStatus())
        {
            _player.ReloadAmmo();
            _player.ExtraLife();
            GameObject.Find("Canvas").GetComponent<UIManager>().UpdateWave(_spawnManager.GetWaveCount().ToString());
        }
        Destroy(gameObject, 0.25f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            if (!other.GetComponent<Laser>().IsEnemyLaser())
            {
                GetComponent<CircleCollider2D>().enabled = false;
                Destroy(other.gameObject);
                DestroyAsteroid();
            }          
        }

        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().Damage();
            DestroyAsteroid();
        }

        
    }
}
