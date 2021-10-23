using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleAfterPlay : MonoBehaviour
{
    [SerializeField] private ParticleSystem _ParticleSystem;
    private float _DestroyDelay = 0f;
    void Start()
    {
        _DestroyDelay = _ParticleSystem.main.duration;
        StartCoroutine(LateDestroy(_DestroyDelay));
    }

    private IEnumerator LateDestroy(float time)
    {
        yield return new WaitForSeconds(time); 
        Destroy(this.gameObject);
    }
}
