using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class AIController : Photon.MonoBehaviour
{
    [SerializeField] private TankController _TankController;
    [SerializeField] private float _LookSpeed = 100f;
    private float _TargetSearchRate = 1f;
    private float _Cooldown = 3f;
    private float _CooldownTimer = 3f;

    private bool CanShoot()
    {
        return _CooldownTimer > _Cooldown;
    }
    
    bool CanSeeTarget(TankController tank)
    {
        if (tank == null)
            return false;
        
        Vector3 turretPos = _TankController.ShootPoint.position;
        Vector3 target = tank.transform.position;
        Vector3 adjustedTarget = new Vector3(target.x, turretPos.y, target.z);
        
        
        Ray ray = new Ray(turretPos, adjustedTarget - turretPos);
            
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
        {
           
            if (hit.transform.gameObject.TryGetComponent(out TankController other))
            {
                if (other == tank)
                    return true;
                else if (other.photonView.owner != photonView.owner)
                {
                    _CurrentTarget = other;
                    return true;
                }
            }
            else // test deflect
            {
                Vector3 barrelForward = _TankController.ShootPoint.forward;
                
                Ray calcRay = new Ray((turretPos - barrelForward) + new Vector3(0,barrelForward.y,0f), _TankController.Turret.forward);
                //Debug.DrawRay(calcRay.origin, calcRay.direction * 1000f, Color.cyan, 1f);
                RaycastHit calchit;
                if (Physics.Raycast(calcRay, out calchit, float.MaxValue))
                {
                    Debug.DrawLine(calcRay.origin, calchit.point, Color.blue, 0.2f);
                    if (hit.transform.gameObject.TryGetComponent(out TankController calcOther))
                    {
                        if (calcOther.photonView.owner != photonView.owner)
                        {
                            _CurrentTarget = calcOther;
                            return true;
                        }
                    }
                    else
                    {
                        
                        Vector3 deflect = Vector3.Reflect(calchit.point - turretPos, calchit.normal).normalized;
                        Vector3 deflectModified =
                            new Vector3(deflect.x,0f, deflect.z); 
                        
                        
                        if (Physics.Raycast(calchit.point, deflectModified, out RaycastHit hitBounce, float.MaxValue))
                        {
                            
                            if (hitBounce.transform.gameObject.TryGetComponent(out TankController otherDeflectTank) && otherDeflectTank.photonView.owner != this.photonView.owner)
                            {
                                Debug.DrawLine(calchit.point, hitBounce.point, Color.red, 0.2f);
                                Debug.Log("Found Target");
                                _DeflectTarget = calchit.point;
                                _CurrentTarget = otherDeflectTank;
                                return true;
                            }
                            else
                            {
                                Debug.DrawLine(calchit.point, hitBounce.point, Color.blue, 0.2f);
                            }
                        }
                    }
                }
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
    private Vector3 _DeflectTarget = Vector3.zero;
    
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

    private float _RandomLook = 0.5f;
    private float _RandomLookTimer = 0.5f;

    private Vector3 _RandomLookPos = Vector3.zero;
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
                bool canSee =  CanSeeTarget(_CurrentTarget);
                if (_CooldownTimer >= _Cooldown && canSee)
                {
                    _TankController.FireWeapon();
                    _CooldownTimer = 0f;
                    
                }
                else if (_RandomLookTimer > _RandomLook && !canSee)
                {
                    _RandomLookPos = _CurrentTarget.transform.position + ( UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(-25,25f));
                    _RandomLookTimer = 0f;
                }
                
                if (!canSee)
                {
                    _TankController.TurnTurret(_RandomLookPos,_LookSpeed * Time.deltaTime);
                }

                if (canSee)
                {
                    _TankController.TurnTurret(_CurrentTarget.transform.position,_LookSpeed * Time.deltaTime);
                }


            }
            _CooldownTimer += Time.deltaTime;
            _RandomLookTimer += Time.deltaTime;
            
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
