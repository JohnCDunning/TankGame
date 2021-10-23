using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkingEvents : MonoBehaviour
{
    public static readonly byte _CreateProjectileEvent = 0;
    public static readonly byte _CreateMineEvent = 1;
    public static readonly byte _OnDestroyEvent = 2;

    public static NetworkingEvents Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        PhotonNetwork.OnEventCall += OnEvent;
    }

    private void OnDestroy()
    {
        PhotonNetwork.OnEventCall -= OnEvent;
    }

    public void OnEvent(byte eventCode, object content, int senderId)
    {
        
        // Do something
        if (eventCode == NetworkingEvents._CreateProjectileEvent)
        {
            object[] data = (object[])content;
            
            Vector3 pos = (Vector3) data[0];
            Vector3 rot = (Vector3) data[1];
            
            PhotonNetwork.InstantiateSceneObject("Projectile", pos, Quaternion.Euler(rot), 0,
                null);
        }else if (eventCode == NetworkingEvents._CreateMineEvent)
        {
            object[] data = (object[])content;
            
            Vector3 pos = (Vector3) data[0];
            Vector3 rot = (Vector3) data[1];

            PhotonNetwork.InstantiateSceneObject("Mine", pos, Quaternion.Euler(rot), 0, null);
        }else if (eventCode == _OnDestroyEvent)
        {
            object[] data = (object[]) content;
            PhotonView pv = PhotonView.Find((int) data[0]);
            if (pv.isMine)
            {
                PhotonNetwork.Destroy(pv);
            }
        }
    }
    
}
