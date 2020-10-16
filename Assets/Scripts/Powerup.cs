using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    private float _downSpeed = 1.5f;
    [SerializeField]
    private int _powerupID;
    [SerializeField]
    private AudioClip _powerupClip;


    // Update is called once per frame
    void Update()
    {
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
