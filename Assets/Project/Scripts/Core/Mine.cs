using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Photon.MonoBehaviour
{
    [SerializeField] private float _SelfDestroyTimer = 10f;
    private float _DestroyTime;
    private bool _HasDestroyedSelf;
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
            //- The projectile will explode now
            PhotonNetwork.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (PhotonNetwork.isMasterClient && other.gameObject.TryGetComponent(out TankController tankController))
        {
            Explode();
        }
    }
}
