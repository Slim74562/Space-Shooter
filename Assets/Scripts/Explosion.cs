using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CamShake>().Shake(0.05f);
        Destroy(this.gameObject, 2.0f);
    }
}
