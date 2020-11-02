using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Bullet : MonoBehaviour
{
    private int _speed = 1;
    private Transform _playerTransform;
    private Vector3 _translateDirection;
    [SerializeField]
    private GameObject _explosionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        if (_playerTransform == null)
        {
            Debug.LogError("Player Transform on Bullet is Null");
            _translateDirection = Vector3.up;
        }
        else
        {
            _translateDirection = _playerTransform.position - transform.position;
        }
        GetComponent<Projectile>().SetEnemyProjectile(true);
    }

    // Update is called once per frame
    void Update()
    {
        _translateDirection = _playerTransform.position - transform.position;
        this.transform.Translate(_translateDirection * _speed * Time.deltaTime);
    }    
}
