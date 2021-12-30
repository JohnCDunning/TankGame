using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    private float _ActiveMagnitude;
    private Camera _CachedCamera;
    public void TriggerShake(float mag, float dur)
    {
        StopAllCoroutines();
        StartCoroutine(Shake(mag,dur));
    }
    IEnumerator Shake(float magnitude, float duration)
    {
        if (!_CachedCamera)
            _CachedCamera = Camera.main;
        
        if (_CachedCamera)
        {
            Vector3 startPos = _CachedCamera.transform.localPosition;
            float currentTime = 0;
            _ActiveMagnitude += magnitude;
            while (currentTime < duration)
            {
                float x = Random.Range(-1, 1) * _ActiveMagnitude;
                float y = Random.Range(-1, 1) * _ActiveMagnitude;
                _CachedCamera.transform.localPosition = new Vector3(x, y, startPos.z);

                currentTime += Time.deltaTime;
                yield return null;
            }

            _ActiveMagnitude -= magnitude;
            _CachedCamera.transform.localPosition = startPos;
        }

        yield return null;
    }
        
}
