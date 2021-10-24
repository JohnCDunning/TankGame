using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Shake(0.05f,0.04f));
        }
    }
    IEnumerator Shake(float magnitude, float duration)
    {
        Vector3 startPos = Camera.main.transform.localPosition;
        float currentTime = 0;
        while(currentTime < duration)
        {
            float x = Random.Range(-1, 1) * magnitude;
            float y = Random.Range(-1, 1) * magnitude;
            Camera.main.transform.localPosition = new Vector3(x, y, startPos.z);

            currentTime += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.localPosition = startPos;
        yield return null;
    }
}
