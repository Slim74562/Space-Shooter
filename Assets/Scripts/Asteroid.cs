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

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if(_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is Null");
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
        GameObject.Find("Canvas").GetComponent<UIManager>().UpdateWave(_spawnManager.GetWaveCount().ToString());
        Destroy(this.gameObject, 0.25f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            if (!other.GetComponent<Laser>().IsEnemyLaser())
            {
                Destroy(other.gameObject);
                DestroyAsteroid();
            }          
        }

        if (other.tag == "Player")
        {
            other.GetComponent<Player>().Damage();
            DestroyAsteroid();
        }

        
    }
}
