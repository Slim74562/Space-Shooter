using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    private GameObject _enemy;
    [SerializeField]
    private float _enemyNotNullSpeed = 5;
    [SerializeField]
    private float _enemyNullSpeed = 3;
    [SerializeField]
    private AudioClip _fireballShotClip;
    private Player _player;
    

    private GameObject[] enemies;
    void Start()
    {
        AudioSource.PlayClipAtPoint(_fireballShotClip, transform.position, 0.25f);
        _enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (_enemy == null)
        {
            Debug.LogError("Enemy on Fireball is null");
        }

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player on Fireball is null");
        }
    }
    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {

        if (_enemy != null)
        {
            _speed = _enemyNotNullSpeed;
            Vector2 direction = _enemy.transform.position - transform.position;
            GetComponent<Rigidbody2D>().velocity = (direction * _speed);
        }
        else
        {
            _speed = _enemyNullSpeed;
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
            GetComponent<Projectile>().CheckBounds();
        }


    }

    IEnumerator WaitforArray()
    {
        yield return new WaitForSeconds(0.5f);
        _enemy = GameObject.FindGameObjectWithTag("Enemy");
        Move();
        AudioSource.PlayClipAtPoint(_fireballShotClip, transform.position, 0.25f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Enemy killEnemy = other.GetComponent<Enemy>();
            killEnemy.ClearShield();
            _player.SetScore(10);
            killEnemy.KillEnemy();
            StartCoroutine(WaitforArray());
        }
    }
}
