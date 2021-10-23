using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    [SerializeField] private Rigidbody _Rigidbody;
    [SerializeField] private float _MovementSpeed = 0f;
    public void MoveTank(Vector3 direction)
    {
        _Rigidbody.velocity = direction.normalized * _MovementSpeed;
        TurnTank(direction.normalized);
    }

    public void TurnTank(Vector3 direction)
    {
        this.transform.forward = direction;
    }

    public void TurnTurret(float lookAngle)
    {
        // look at mouse
    }

    public void FireWeapon()
    {
        
    }
}
