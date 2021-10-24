using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class AIController : Photon.MonoBehaviour
{
    [SerializeField] private TankController _TankController;
    [SerializeField] private float _LookSpeed = 100f;
    private float _TargetSearchRate = 1f;
    [SerializeField] private float _ShootCooldown = 3f;
    private float _ShootCooldownTimer = 1000;
    [SerializeField] private bool _UseDeflection = true;
    [SerializeField] private bool _UseMovement = true;
    [SerializeField] private int _DeflectionCount = 5;
    private bool CanShoot()
    {
        return _ShootCooldownTimer > _ShootCooldown;
    }
    bool CanSeeTarget(TankController tank, out bool aimingAtTarget)
    {
        aimingAtTarget = false;
        if (tank == null)
            return false;
        
        Vector3 turretPos = _TankController.ShootPoint.position;
        Vector3 target = tank.transform.position;
        Vector3 adjustedTarget = new Vector3(target.x, turretPos.y, target.z);
        
        
        Ray ray = new Ray(turretPos, adjustedTarget - turretPos);
            
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
        {
           
            if (hit.transform.TryGetComponent(out TankController other))
            {
                if (Physics.Raycast(turretPos, _TankController.Turret.forward, out RaycastHit aimedHit, float.MaxValue))
                {
                    if (aimedHit.transform.TryGetComponent<TankController>(out var aimedTankController))
                    {
                        if (aimedTankController.photonView.owner != photonView.owner)
                        {
                            aimingAtTarget = true;
                            _CurrentTarget = aimedTankController;
                            return true;
                        }
                    }
                }
                else
                {
                    aimingAtTarget = false;
                    
                    if (other == tank)
                        return true;
                    else if (other.photonView.owner != photonView.owner)
                    {
                        _CurrentTarget = other;
                        return true;
                    }
                }
            }
            else if (_UseDeflection)
            {
                Vector3 barrelForward = _TankController.ShootPoint.forward;

                Vector3 lastPosition = (turretPos - barrelForward) + new Vector3(0,barrelForward.y,0f);
                Vector3 lastDirection = _TankController.Turret.forward;

                RaycastHit lastHit;
                
                for (int i = 0; i < _DeflectionCount; i++)
                {
                    if (Physics.Raycast(lastPosition,lastDirection , out lastHit, float.MaxValue))
                    {
                        
                        if (lastHit.transform.TryGetComponent(out TankController calcOther))
                        {
                            if (calcOther.photonView.owner != photonView.owner)
                            {
                                Debug.DrawLine(lastPosition, lastHit.point, Color.red, 0.2f);
                                _CurrentTarget = calcOther;
                                aimingAtTarget = true;
                                return true;
                            }
                            else
                            {
                                aimingAtTarget = false;
                                return false;
                            }
                        }else if (lastHit.transform.TryGetComponent<IDamageable>(out IDamageable damageable))
                        {
                            aimingAtTarget = false;
                            return false;
                        }
                        else
                        {
                            Debug.DrawLine(lastPosition, lastHit.point, Color.blue, 0.2f);
                            
                            lastDirection = Vector3.Reflect(lastHit.point - lastPosition, lastHit.normal).normalized;
                            lastDirection = new Vector3(lastDirection.x, 0f, lastDirection.z);
                            lastPosition = lastHit.point;
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
            if (CanSeeTarget(tank, out bool aimingAtTarget))
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
    private Vector3 _MovePosition = Vector3.zero;
    private float _RepathDistance = 10f;
    [SerializeField] private NavMeshAgent _Agent;

    public IEnumerator HandleMovement()
    {
        while (_Running && _UseMovement)
        {
            
            // check if the angle of the move vector is greater than ?? 
            if (Vector3.Angle(this.transform.forward, _Agent.desiredVelocity) > 10f)
            {
                float dot = Vector3.Dot(this.transform.right, _Agent.desiredVelocity);
                _Agent.speed = 0f;
                _TankController.TurnTank(dot);
            }
            else
            {
                _Agent.speed = 3f;
                if (!_Agent.hasPath)
                {
                    Vector3 dir = UnityEngine.Random.insideUnitSphere * _RepathDistance;
                    if (NavMesh.SamplePosition(dir, out NavMeshHit hit, _RepathDistance, 1))
                        _MovePosition = hit.position;

                    _Agent.SetDestination(_MovePosition);

                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDrawGizmos()
    {
        Vector3[] paths = _Agent.path.corners;
        if (paths.Length > 0)
        {
            Gizmos.DrawLine(this.transform.position, paths[0]);
            for (int i = 0; i < paths.Length; i++)
            {
                if (i + 1 < paths.Length)
                {
                    Gizmos.DrawLine(paths[i], paths[i + 1]);
                }
            }
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
                StartCoroutine(HandleMovement());
            }
            
            if (_CurrentTarget != null )
            {
                bool canSee =  CanSeeTarget(_CurrentTarget, out bool aimingAtTarget);
                if (_ShootCooldownTimer >= _ShootCooldown && aimingAtTarget)
                {
                    _TankController.FireWeapon(_DeflectionCount);
                    _ShootCooldownTimer = 0f;
                    
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
            _ShootCooldownTimer += Time.deltaTime;
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
