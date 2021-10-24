using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDamage : MonoBehaviour
{
    public float LerpSpeed = 3;
    public float _AddedScaleOnHit = 0.2f;
    private Vector3 scaleAtStart;
    Material mat;
    private void Awake()
    {
        scaleAtStart = transform.localScale;
        mat = GetComponent<MeshRenderer>().material;
    }
   
    public void ObjectHit()
    {
        mat.SetFloat("LerpTime", 1);
        StopCoroutine(HitFlash());
        StartCoroutine(HitFlash());
    }
    IEnumerator HitFlash()
    {
        Vector3 currentScale = transform.localScale + (Vector3.one * _AddedScaleOnHit);
        float t = 1;
        while (mat.GetFloat("LerpTime") > 0)
        {
            t -= LerpSpeed * Time.deltaTime;
            mat.SetFloat("LerpTime", t);
            transform.localScale = Vector3.Lerp(scaleAtStart, currentScale, t);
           yield return null;
        }
        mat.SetFloat("LerpTime", 0);
        transform.localScale = scaleAtStart;
        yield return null;
    }
}
