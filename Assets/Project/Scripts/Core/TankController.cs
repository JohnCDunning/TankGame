using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TankController : MonoBehaviour
{
    [SerializeField] private Rigidbody _Rigidbody;
    [SerializeField] private float _MovementSpeed = 1000f;
    [SerializeField] private float _RotationSpeed = 100f;
    [SerializeField] private Transform _Turret;
    [SerializeField] private Transform _Body;
    [SerializeField] private Transform _TurrentShootPoint;
    public void MoveTank(Vector3 direction)
    {
        _Rigidbody.velocity = _Body.forward * ((direction.z * _MovementSpeed)  * Time.deltaTime);

        TurnTank(direction.x);
    }

    public void TurnTank(float turnDirection)
    {
        _Body.transform.Rotate(new Vector3(0f, turnDirection * _RotationSpeed * Time.deltaTime, 0f), Space.World);
    }

    public void TurnTurret(Vector3 lookTarget)
    {
        Vector3 direction = lookTarget - transform.position;
        float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        _Turret.rotation = Quaternion.Euler(0, rotation, 0f);
    }

    public void FireWeapon(Vector3 target)
    {
        Vector3 turretPos = _TurrentShootPoint.position;
        Vector3 adjustedTarget = new Vector3(target.x, turretPos.y, target.z);
        Ray ray = new Ray(turretPos, adjustedTarget - turretPos);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.DrawLine(turretPos, hit.point, Color.blue, 5f);

            Ray bounceRay = new Ray(hit.point, hit.normal);
            if (Physics.Raycast(bounceRay, out RaycastHit hitBounce))
            {

                Debug.DrawLine(hit.point, hitBounce.point * 100f, Color.red, 5f);
            }
        }
    }
}
