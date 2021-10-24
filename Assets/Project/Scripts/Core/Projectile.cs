using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : Photon.MonoBehaviour, IDamageable
{
    public Rigidbody rb;
    public float _MovementSpeed;
    public float _ReflectionsRemaining = 1;
    private bool _IsDestroyed = false;


    private bool _FlaggedForDestruction;
    [SerializeField] private ParticleSystem _ReflectPS;
    [SerializeField] private ExplosionSize _ExplosionSize;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (_IsDestroyed)
            return;
        
        if (PhotonNetwork.isMasterClient)
        {

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 0.5f))
            {
                
                if (hit.transform.gameObject.TryGetComponent(out IDamageable damageable))
                {
                    this.Damage();
                    damageable.Damage();
                }
                else
                    Hit(hit.point, hit.normal);
                
            }
            rb.velocity = transform.forward * _MovementSpeed;
            
        }
    }

    void Hit(Vector3 hit, Vector3 hitNormal)
    {
        if (_ReflectionsRemaining > 0)
        {
            Vector3 incomingVec = hit - transform.position;
            transform.forward = Vector3.Reflect(incomingVec, hitNormal);
            _ReflectionsRemaining--;
            _ReflectPS.Play();
            return;
        }

        if (PhotonNetwork.isMasterClient)
        {
            AppManager._Instance.SpawnExplosion(_ExplosionSize, transform.position);
            _IsDestroyed = true;
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(NetworkingEvents._OnDestroyEvent, new object[]{photonView.viewID}, true,
                raiseEventOptions);
        }
    }

    public void Damage()
    {
        AppManager._Instance.SpawnExplosion(_ExplosionSize, transform.position);
        _IsDestroyed = true;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(NetworkingEvents._OnDestroyEvent, new object[]{photonView.viewID}, true, raiseEventOptions);
    }
    
}
