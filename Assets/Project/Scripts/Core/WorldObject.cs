using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour, IDamageable
{
    public float _Health = 1;
    [SerializeField] private ExplosionSize _ExplosionSize;
    public void Damage()
    {
        _Health -= 1;
        if (_Health <= 0)
        {
            AppManager._Instance.SpawnExplosion(_ExplosionSize, transform.position);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
