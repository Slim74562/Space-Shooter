using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Boss : MonoBehaviour
{
    private Player _player;
    private GameObject[] _projectiles;
    private float _leastDistance = 0;
    private GameObject _closestProjectile;
    private int _health = 0;
    private float _canFire = -1;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private GameObject[] _weaponPrefabs;
    private SpriteRenderer _spriteRender;
    private bool _isDead = false;
    private bool _beamActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player on Boss is Null");
        }

        _spriteRender = GetComponent<SpriteRenderer>();
        if (_spriteRender == null)
        {
            Debug.LogError("Sprite Renderer on Boss is Null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckProjectiles();
        FireWeapon();
    }

    private void Movement(Vector3 tooClose)
    {
            float xPos = (transform.position.x - tooClose.x);
            float yPos = (transform.position.y - tooClose.y);
            if (xPos == transform.position.x)
            {
                xPos += Random.Range(-8f, 8f);
            }
            if (yPos == transform.position.y)
            {
                yPos += Random.Range(-12f, 12f);
            }
            transform.position = new Vector3(xPos, yPos, 0);        
    }

    private void FireWeapon()
    {
        if (Time.time > _canFire && !_isDead)
        {
            float fireRate = Random.Range(1.0f, 5.0f);
            _canFire = Time.time + fireRate;
            int randomNum = Random.Range(0, _weaponPrefabs.Length);
            GameObject weapon = null;
            switch (randomNum)
            {
                case 0: // Bullet
                    weapon = Instantiate(_weaponPrefabs[0], transform.position, Quaternion.identity);
                    break;
                case 1: // Laser
                    weapon = Instantiate(_weaponPrefabs[1], transform.position, Quaternion.identity);
                    weapon.GetComponent<Laser>().SetLaserType("Wheel_Enemy(Clone)");
                    break;
                case 2: // Beam
                    if(!_beamActivated)
                    {
                        _beamActivated = true;
                        _weaponPrefabs[2].SetActive(true);
                        IEnumerator DestroyBeam()
                        {
                            yield return new WaitForSeconds(5.0f);
                            _weaponPrefabs[2].SetActive(false);
                            _beamActivated = false;
                        }
                        StartCoroutine(DestroyBeam());
                    }
                    

                    break;
                case 3:
                    weapon = Instantiate(_weaponPrefabs[3], transform.position, Quaternion.identity);
                    break;
            }
            if (weapon != null)
            {
                weapon.GetComponent<Projectile>().SetEnemyProjectile(true);
            }
            
        }
    }

    private void CheckProjectiles()
    {
        _projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        if (_projectiles.Length != 0)
        {
            foreach (GameObject g in _projectiles)
            {
                if (!g.GetComponent<Projectile>().IsEnemyProjectile())
                {
                    float dist = Vector3.Distance(g.transform.position, transform.position);
                    if (dist < _leastDistance || _leastDistance == 0)
                    {
                        _leastDistance = dist;
                        _closestProjectile = g;
                    }
                }
            }
            _projectiles = null;
            if (_leastDistance < 2 && _closestProjectile != null)
            {
                Movement(_closestProjectile.transform.position);
                _closestProjectile = null;
                _leastDistance = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile") && !collision.GetComponent<Projectile>().IsEnemyProjectile())
        {            
            if (_closestProjectile == collision.gameObject)
            {
                _closestProjectile = null;
            }
            Destroy(collision.gameObject);
            CheckHealth();
        }


        if (collision.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();
                CheckHealth();
            }
        }
    }

    private void CheckHealth()
    {
        _health++;
        switch (_health)
        {
            case 0:
                {
                    _spriteRender.color = Color.white;
                    break;
                }
            case 1:
                {
                    _spriteRender.color = Color.blue;
                    break;
                }
            case 2:
                {
                    _spriteRender.color = Color.green;
                    break;
                }
            case 3:
                {
                    _spriteRender.color = Color.yellow;
                    break;
                }
            case 4:
                {
                    _spriteRender.color = Color.red;
                    break;
                }
            case 5:
                {
                    KillBoss();
                    break;
                }
        }
    }

    public void KillBoss()
    {
        GameObject explode = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        explode.transform.localScale = new Vector3(2, 2, 2);
        _player.SetScore(150);
        Destroy(gameObject);
    }
}
