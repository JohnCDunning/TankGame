using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEngine;

public class TankController : Photon.MonoBehaviour, IDamageable
{
    [SerializeField] private Rigidbody _Rigidbody;
    [SerializeField] private float _MovementSpeed = 1f;
    [SerializeField] private float _RotationSpeed = 100f;
    [SerializeField] private Transform _Turret;
    public Transform Turret { get => _Turret; }
    public Transform ShootPoint { get => _TurretShootPoint; }
    
    [SerializeField] private Transform _Body;

    [PreviouslySerializedAs("_TurrentShootPoint")] [SerializeField]
    private Transform _TurretShootPoint;

    [SerializeField] private Transform _MineDeployLocation;
    [SerializeField] private Animator _TurretRecoilAnimator;

    private Vector3 _LastVelocity = Vector3.zero;

    public void MoveTank(Vector3 direction)
    {
        _LastVelocity = _Body.forward * (direction.z * _MovementSpeed);

        TurnTank(direction.x);
    }

    public void Damage()
    {
        if (PhotonNetwork.isMasterClient && !photonView.isSceneView)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
            PhotonNetwork.RaiseEvent(NetworkingEvents._OnPlayerDefeated,
                new object[] {photonView.viewID, photonView.owner.ID}, true, raiseEventOptions);
        }
        else if (PhotonNetwork.isMasterClient)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
            PhotonNetwork.RaiseEvent(NetworkingEvents._OnDestroyEvent, new object[] {photonView.viewID}, true,
                raiseEventOptions);
        }
    }

    private void FixedUpdate()
    {
        if (base.photonView.isMine)
            _Rigidbody.velocity = _LastVelocity;
    }

    public void TurnTank(float turnDirection)
    {
        _Body.transform.Rotate(new Vector3(0f, (turnDirection * _RotationSpeed) * Time.deltaTime, 0f), Space.World);
    }

    public void TurnTurret(Vector3 lookTarget)
    {
        Vector3 direction = lookTarget - transform.position;
        float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        _Turret.rotation = Quaternion.Euler(0, rotation, 0f);
    }

    public void TurnTurret(Vector3 lookTarget, float step)
    {
        Vector3 direction = lookTarget - transform.position;
        float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        _Turret.rotation = Quaternion.RotateTowards(_Turret.rotation, Quaternion.Euler(0, rotation, 0), step);
    }
    



    public void FireWeapon()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.MasterClient};
        object[] content = new object[]
        {
            _TurretShootPoint.position,
            _Turret.rotation.eulerAngles
        };


        PhotonNetwork.RaiseEvent(NetworkingEvents._CreateProjectileEvent, content, true, raiseEventOptions);

        photonView.RPC("RunTurretAnimation", PhotonTargets.All);
    }

    public void DeployMine()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.MasterClient};
        object[] content = new object[]
        {
            _MineDeployLocation.position,
            _MineDeployLocation.rotation.eulerAngles
        };


        PhotonNetwork.RaiseEvent(NetworkingEvents._CreateMineEvent, content, true, raiseEventOptions);
    }

    [PunRPC]
    void RunTurretAnimation()
    {
        _TurretRecoilAnimator.SetTrigger("Shoot");
    }
}

