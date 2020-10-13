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
    

    private GameObject[] enemies;
    void Start()
    {
        AudioSource.PlayClipAtPoint(_fireballShotClip, transform.position, 0.25f);
        _enemy = GameObject.FindGameObjectWithTag("Enemy");
        Debug.Log("_enemy is " + _enemy);
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
            if (transform.position.y >= 45f)
            {
                Destroy(this.gameObject);
            }
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
            other.GetComponent<Enemy>().KillEnemy();
            other.tag = "Dying";
            StartCoroutine(WaitforArray());
        }
    }
}
