using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    private bool _isShaking = false;
    private float _shakeDuration = 0.5f;
    private float _shakeMagnitude;
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
            transform.position = initialPosition + UnityEngine.Random.insideUnitSphere * _shakeMagnitude;
        }
    }

    public void Shake(float magnitude)
    {
        _isShaking = true;
        _shakeMagnitude = magnitude;
    }
}
