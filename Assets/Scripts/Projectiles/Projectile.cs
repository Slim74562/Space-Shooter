using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Projectile : MonoBehaviour
{
    private bool _isEnemyProjectile = false;
    [SerializeField]
    private GameObject _explosionPrefab;
    private float _yBounds = 8f;
    private float _xBounds = 12f;
    private Player _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        CheckBounds();
    }

    public bool IsEnemyProjectile()
    {
        return _isEnemyProjectile;
    }

    public void SetEnemyProjectile(bool enemyProj)
    {
        _isEnemyProjectile = enemyProj;
    }

    public void CheckBounds()
    {        
        if (transform.position.x > _xBounds || transform.position.x < -_xBounds)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
        if (transform.position.y > _yBounds || transform.position.y < -_yBounds)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isEnemyProjectile)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Player>().Damage();

                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                Destroy(gameObject);
            }

            if (other.CompareTag("Powerup"))
            {
                Instantiate(_explosionPrefab, other.transform.position, Quaternion.identity);
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
