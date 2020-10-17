using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8;
    [SerializeField]
    private GameObject _explosionPrefab; 
    private bool _isEnemyLaser = false;


    // Update is called once per frame
    void Update()
    {
        if (_isEnemyLaser)
        {
            MoveDown();
        }
        else
        {
            MoveUp();
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if (transform.position.y >= 8f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= -8f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
    
    public void EnemyLaser()
    {
        _isEnemyLaser = true;
    }

    public bool GetLaserType()
    {
        return _isEnemyLaser;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_isEnemyLaser)
        {
            if (other.tag == "Player")
            {
                other.GetComponent<Player>().Damage();
                Destroy(this.gameObject);
            }

            if (other.tag == "Powerup")
            {
                Instantiate(_explosionPrefab, other.transform.position, Quaternion.identity);
                Destroy(other.gameObject);
                Destroy(this.gameObject);
            }
        }
    }
}
