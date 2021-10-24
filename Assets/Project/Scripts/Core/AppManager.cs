using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AppManager : Photon.MonoBehaviour
{
    public static AppManager _Instance;
    [SerializeField] private Volume _Volume;

    public ExplosionObjects[] _ExplosionObjects;

    public AppManager Instance { get => _Instance;}

    private ClampedFloatParameter _CAIntensity;
    private ChromaticAberration _CachedChromaticAbberation;
    
    private void Awake()
    {
        _Instance = this;

        if (_Volume.sharedProfile.TryGet(typeof(ChromaticAberration), out ChromaticAberration ca))
        {
            _CAIntensity = ca.intensity;
            _CachedChromaticAbberation = ca;
        }
    }

    IEnumerator ScreenEffect(ExplosionSize size)
    {

        float target = 0f;
        float time = 0f;
        float t = 0f;
            
        switch (size)
        {
            case ExplosionSize.small:
                time = 0.2f;
                target = .1f;
                break;
            case ExplosionSize.medium:
                target = .25f;
                time = 0.25f;
                break;
            case ExplosionSize.large:
                target = .5f;
                time = 0.3f;
                break;
            case ExplosionSize.huge:
                time = 0.4f;
                target = 1f;
                break;
        }

        while (t < time)
        {
            t += Time.deltaTime;
            _CachedChromaticAbberation.intensity.value = Mathf.Lerp(0, target, t / time);
            yield return new WaitForEndOfFrame();
        }

        t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            _CachedChromaticAbberation.intensity.value = Mathf.Lerp(0, _CAIntensity.value, t / time);
            yield return new WaitForEndOfFrame();
        }
    }
    
    public void RequestExplosion(ExplosionSize explosionSize, Vector3 position)
    {
        photonView.RPC("SpawnExplosion", PhotonTargets.All, explosionSize,position);
    }
    [PunRPC]
    public void SpawnExplosion(ExplosionSize explosionSize, Vector3 position)
    {
        StartCoroutine(ScreenEffect(explosionSize));
        for (int i = 0; i < _ExplosionObjects.Length; i++)
        {
            if(_ExplosionObjects[i]._ExplosionSize == explosionSize)
            {
                Instantiate(_ExplosionObjects[i]._ExplosionPrefab, position, Quaternion.identity);
                return;
            }
        }
    }
}

[System.Serializable]
public class ExplosionObjects
{
    public ExplosionSize _ExplosionSize;
    public GameObject _ExplosionPrefab;
}
