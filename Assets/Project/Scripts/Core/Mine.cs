using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Unity.Mathematics;
using UnityEngine;

public class Mine : Photon.MonoBehaviour
{
    [SerializeField] private float _SelfDestroyTimer = 3f;
    [SerializeField] private GameObject _ParticlePrefab;
    private float _DestroyTime;
    private bool _HasDestroyedSelf;

    private float _ExplosionRange = 4f;

    private float _PulseT = 0f;
    [SerializeField] private float _PulseCiel = 1f;
    private bool _CielReached = false;

    [SerializeField] private MeshRenderer _TargetRenderer;
    private Material _TargetMaterial;
    
    
    private static readonly int _EmissionT = Shader.PropertyToID("EmissionT");
    private static readonly int _EmissionMaxT = Shader.PropertyToID("EmissionMaxT");

    private float _ArmingDelay = 0.5f;
    private float _ArmingTimer = 0f;
    
    [SerializeField] private Light _Light;
    
    private void Awake()
    {
        _TargetMaterial = _TargetRenderer.material;
        _PulseCiel = _SelfDestroyTimer / 4;
    }

    private void Update()
    {


        if (_ArmingTimer < _ArmingDelay)
        {
            _ArmingTimer += Time.deltaTime;
            return;
        }

        if (!_CielReached)
        {
            _PulseT += Time.deltaTime;
            if (_PulseT >= _PulseCiel)
            {
                _CielReached = true;
            }
        }
        else
        {
            _PulseT -= Time.deltaTime;
            if (_PulseT <= 0f)
            {
                _CielReached = false;
                _PulseCiel /= 2;

            }
        }

        _Light.intensity = Mathf.Lerp(1f, 5f, _PulseT / _PulseCiel);
        _TargetMaterial.SetFloat(_EmissionT, _PulseT);
        _TargetMaterial.SetFloat(_EmissionMaxT, _PulseCiel);


        if (photonView.isMine)
        {

            _DestroyTime += Time.deltaTime;
            if (!_HasDestroyedSelf && _DestroyTime >= _SelfDestroyTimer)
            {
                Explode();
            }
        }
    }

    private void Explode()
    {
        _HasDestroyedSelf = true;
        AppManager._Instance.SpawnExplosion(ExplosionSize.large, transform.position);
        //Instantiate(_ParticlePrefab,this.transform.position, quaternion.Euler(-90, 0, 0), null);
        
        if (photonView.isMine)
        {
            
            Collider[] results = Physics.OverlapSphere(this.transform.position, _ExplosionRange);
            results.ForEach(i =>
            {
                if (i.gameObject.TryGetComponent(out IDamageable damageable))
                    damageable.Damage();
            });
            
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (_ArmingTimer < _ArmingDelay)
            return;
        
        if (PhotonNetwork.isMasterClient && col.gameObject.TryGetComponent(out TankController tankController))
        {
            Explode();
        }
    }
}