using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

public class Mine : Photon.MonoBehaviour
{
    [SerializeField] private float _SelfDestroyTimer = 10f;
    private float _DestroyTime;
    private bool _HasDestroyedSelf;

    [SerializeField] private float _ExplosionRange = 2f;
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
        if (PhotonNetwork.isMasterClient)
        {
            Collider[] results = new Collider[]{};
            var size = Physics.OverlapSphereNonAlloc(this.transform.position, _ExplosionRange, results);
            results.ForEach(i =>
            {
                if (i.TryGetComponent(out IDamageable damageable))
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