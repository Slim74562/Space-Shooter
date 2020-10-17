using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    private float _downSpeed = 1f;
    [SerializeField]
    private int _powerupID;
    [SerializeField]
    private AudioClip _powerupClip;
    private Transform _player;
    private Rigidbody2D _powerupRigidBody2d;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _powerupRigidBody2d = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            Vector2 direction = _player.position - transform.position;
            _powerupRigidBody2d.velocity = (direction * _downSpeed);
        }
        else
        {
            _powerupRigidBody2d.velocity = new Vector2(0, -_downSpeed);
                }
        transform.Translate(Vector3.down * _downSpeed * Time.deltaTime);
        if (transform.position.y < -7f)
        {
            //transform.position = new Vector3(Random.Range(-8f, 8f), 6.5f, 0);
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                AudioSource.PlayClipAtPoint(_powerupClip, transform.position);
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    case 3:
                        player.ReloadAmmo();
                        break;
                    case 4:
                        player.ExtraLife();
                        break;
                    case 5:
                        player.Fireball();
                        break;
                }
            }            
            Destroy(this.gameObject);
        }
    }
}
