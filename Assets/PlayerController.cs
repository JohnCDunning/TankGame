using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Photon.MonoBehaviour
{
    [SerializeField] private TankController _TankController;
    
    private void Update()
    {
        if (base.photonView.isMine)
        {
            // get input
            _TankController.MoveTank(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")));
        }
    }
}
