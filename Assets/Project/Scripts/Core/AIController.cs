using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : Photon.MonoBehaviour
{
    [SerializeField] private TankController _TankController;
    [SerializeField] private float _LookSpeed = 100f;
    private float _TargetSearchRate = 1.5f;
    private float _Cooldown = 3f;
    private float _CooldownTimer = 3f;

    bool CanSeeTarget(TankController tank)
    {
        if (tank == null)
            return false;
        
        Vector3 turretPos = _TankController.ShootPoint.position;
        Vector3 target = tank.transform.position;
        Vector3 adjustedTarget = new Vector3(target.x, turretPos.y, target.z);
        Ray ray = new Ray(turretPos, adjustedTarget - turretPos);
            
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.gameObject.TryGetComponent(out TankController other) && other == tank)
            {
                return true;
            }
        }

        return false;
    }
    public TankController FindClosestOrValidTarget(TankController[] tanks)
    {
        if (tanks == null || tanks.Length == 0)
            return null;
        
        TankController[] distanceSorted = tanks.OrderBy((d) => (d.transform.position - transform.position).sqrMagnitude).ToArray();
        TankController distanceObject = distanceSorted.First();
        
        foreach (var tank in distanceSorted)
        {
            if (CanSeeTarget(tank))
            {
                return tank;
            }
        }
        
        return distanceObject;
    }
    

    private void OnDestroy()
    {
        _Running = false;
    }

    private TankController _CurrentTarget = null;
    private bool _Running = false;
    private IEnumerator UpdateTargets()
    {
        while (_Running)
        {
            TankController[] _OtherTanks = GameObject.FindObjectsOfType<TankController>()
                .Where(i => i.photonView.owner != this.photonView.owner).ToArray();

            _CurrentTarget = FindClosestOrValidTarget(_OtherTanks);

            yield return new WaitForSeconds(_TargetSearchRate);
        }

    }
    
    
    private void Update()
    {
        // look for player
        if (base.photonView.isMine)
        {
            if (_Running == false)
            {
                _Running = true;
                StartCoroutine(UpdateTargets());
            }
            
            if (_CurrentTarget != null )
            {
                if (_CooldownTimer >= _Cooldown && CanSeeTarget(_CurrentTarget))
                {
                    _TankController.FireWeapon();
                    _CooldownTimer = 0f;
                }
                
                _TankController.TurnTurret(_CurrentTarget.transform.position,_LookSpeed * Time.deltaTime);
                
            }
            _CooldownTimer += Time.deltaTime;
            
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
