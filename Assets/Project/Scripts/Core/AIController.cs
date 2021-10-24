using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : Photon.MonoBehaviour
{
    [SerializeField] private TankController _TankController;
    [SerializeField] private float _LookSpeed = 100f;

    private float _Cooldown = 3f;
    private float _CooldownTimer = 3f;
    
    
    private void Update()
    {
        // look for player
        if (base.photonView.isMine)
        {
            TankController[] _OtherTanks = GameObject.FindObjectsOfType<TankController>()
                .Where(i => i.photonView.owner != this.photonView.owner).ToArray();

            foreach (var tank in _OtherTanks)
            {
                
                Vector3 target = tank.transform.position;
                _TankController.TurnTurret(target,_LookSpeed * Time.deltaTime);


                if (_CooldownTimer > _Cooldown)
                {
                    Vector3 turretPos = _TankController.ShootPoint.position;

                    Vector3 adjustedTarget = new Vector3(target.x, turretPos.y, target.z);
                    Ray ray = new Ray(turretPos, adjustedTarget - turretPos);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.transform.gameObject.TryGetComponent(out TankController other))
                        {
                            if (_OtherTanks.Contains(other))
                            {
                                _TankController.FireWeapon();
                                _CooldownTimer = 0f;
                            }
                        }
                    }
                }

                break;
            }
            if (_CooldownTimer <= _Cooldown)
            {
                _CooldownTimer += Time.deltaTime;
            }
        }

       
        
     
        
        /* partial for ai hit detection model
 *
 *  Vector3 turretPos = _TurrentShootPoint.position;
    Vector3 adjustedTarget = new Vector3(target.x, turretPos.y, target.z);
    Ray ray = new Ray(turretPos, adjustedTarget - turretPos);
    
    if (Physics.Raycast(ray, out RaycastHit hit))
    {
        Debug.DrawLine(turretPos, hit.point, Color.blue, 5f);

        Ray bounceRay = new Ray(hit.point, Vector3.Reflect( hit.point - turretPos,hit.normal));
        if (Physics.Raycast(bounceRay, out RaycastHit hitBounce))
        {
            Debug.DrawLine(hit.point, hitBounce.point * 100f, Color.red, 5f);
        }
    }

 * 
 */
    
    }
}
