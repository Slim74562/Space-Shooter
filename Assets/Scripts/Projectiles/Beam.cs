using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    GameObject _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogError("Player on Beam is Null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        _player.transform.Translate((transform.position - _player.transform.position) * 5 * Time.deltaTime);
        _player.GetComponent<Player>().FreezePlayer();
    }
}
