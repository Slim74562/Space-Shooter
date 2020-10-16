using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    private bool _isShaking = false;
    private float _shakeDuration;
    private Vector3 initialPosition = new Vector3(0, 0, -10);

    // Update is called once per frame
    void Update()
    {
        if (_isShaking)
        {            
            IEnumerator ShakeforDuration()
            {
                yield return new WaitForSeconds(_shakeDuration);
                _isShaking = false;
                transform.position = initialPosition;
            }
            StartCoroutine(ShakeforDuration());
            transform.position = initialPosition + UnityEngine.Random.insideUnitSphere * 2;
        }
    }

    public void Shake(float duration)
    {
        _isShaking = true;
        _shakeDuration = duration;
    }
}
