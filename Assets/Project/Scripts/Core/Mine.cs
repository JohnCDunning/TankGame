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
    private void Update()
    {
        if (!PhotonNetwork.isMasterClient)
            return;
        
        _DestroyTime += Time.deltaTime;
        if (!_HasDestroyedSelf && _DestroyTime >= _SelfDestroyTimer)
        {
            Explode();
        }
    }

    private void Explode()
    {
        _HasDestroyedSelf = true;
        Instantiate(_ParticlePrefab,this.transform.position, quaternion.Euler(-90, 0, 0), null);
        
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
        if (PhotonNetwork.isMasterClient && col.gameObject.TryGetComponent(out TankController tankController))
        {
            Explode();
        }
    }
}