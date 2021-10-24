using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitDamage))]
public class WorldObject : Photon.MonoBehaviour, IDamageable
{
    public float _Health = 1;
    [SerializeField] private ExplosionSize _ExplosionSize;
    private HitDamage hitDamage;
    private void Awake()
    {
        hitDamage = GetComponent<HitDamage>();
    }
    public void Damage()
    {
        photonView.RPC("HitDamage",PhotonTargets.All);

        _Health -= 1;
        if (_Health <= 0)
        {
            AppManager._Instance.RequestExplosion(_ExplosionSize, transform.position);
            PhotonNetwork.Destroy(gameObject);
        }
    }
    [PunRPC]
    void HitDamage()
    {
        hitDamage.ObjectHit();
    }
}
