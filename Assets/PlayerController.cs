using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Photon.MonoBehaviour
{
    [SerializeField] private TankController _TankController;
    private Vector3 _LastPoint = Vector3.zero;
    private void Update()
    {
        if (base.photonView.isMine)
        {
            // get input
            _TankController.MoveTank(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxisRaw("Vertical")));
            if (Input.GetMouseButtonDown(0))
            {
                _TankController.FireWeapon(_LastPoint);
            }


            if (Camera.main)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    _TankController.TurnTurret(hit.point);
                    _LastPoint = hit.point;
                }
            }
            else
            {
                Camera.SetupCurrent(GameObject.FindObjectOfType<Camera>());
            }
        }
    }

    private void OnDrawGizmos()
    {
        
        if (Camera.main)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Gizmos.DrawSphere(hit.point, 0.2f);
            }
            
        }

    }
}
