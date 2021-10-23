using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkLight : MonoBehaviour
{
    private float _BlinkTime = 1f;
    private float _BlinkTimer = 0f;
    private Light _Light;

    private void Start()
    {
        _Light = GetComponent<Light>();
    }

    private bool flip = false;
    private void Update()
    {
        if (!flip)
        {
            _BlinkTimer += Time.deltaTime;
            flip = _BlinkTimer >= _BlinkTime;
        }

        if (flip)
        {
            _BlinkTimer -= Time.deltaTime;
            flip = _BlinkTimer <= 0f;
        }

        _Light.intensity = Mathf.Lerp(0, 10f, _BlinkTimer / _BlinkTime);

    }
}
