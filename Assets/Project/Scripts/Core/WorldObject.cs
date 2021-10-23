using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour, IDamageable
{
    public float _Health = 1;
    public void Damage()
    {
        _Health -= 1;
        if (_Health <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
