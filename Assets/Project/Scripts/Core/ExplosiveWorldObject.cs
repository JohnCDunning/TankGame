using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(HitDamage))]
[RequireComponent(typeof(PhotonView))]
public class ExplosiveWorldObject : Photon.MonoBehaviour,IDamageable
{
    private bool _HasDestroyedSelf;
    [SerializeField] private float _ExplosionRange = 4f;

    public float _Health = 1;
    [SerializeField] private ExplosionSize _ExplosionSize;
    private HitDamage hitDamage;

    private void Awake()
    {
        hitDamage = GetComponent<HitDamage>();
    }
    public void Damage()
    {
        photonView.RPC("HitDamage", PhotonTargets.All);
        _Health -= 1;
        if (_Health <= 0)
        {
            photonView.RPC("Explode", PhotonTargets.All);
            AppManager._Instance.RequestExplosion(_ExplosionSize, transform.position);
            PhotonNetwork.Destroy(gameObject);
        }
    }
    [PunRPC]
    void HitDamage()
    {
        hitDamage.ObjectHit();
    }

    [PunRPC]
    private void Explode()
    {
        IDamageable current = this;
        _HasDestroyedSelf = true;
        
        if (photonView.isMine)
        {
            
            Collider[] results = Physics.OverlapSphere(this.transform.position, _ExplosionRange);
            results.ForEach(i =>
            {
                if (i.gameObject.TryGetComponent(out IDamageable damageable) && damageable != current)
                    damageable.Damage();
            });
        }
    }
}