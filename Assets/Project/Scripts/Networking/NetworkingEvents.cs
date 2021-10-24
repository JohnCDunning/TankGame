using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkingEvents : MonoBehaviour
{
    public static readonly byte _CreateProjectileEvent = 0;
    public static readonly byte _CreateMineEvent = 1;
    public static readonly byte _OnDestroyEvent = 2;
    public static readonly byte _OnPlayerDefeated = 3;
    public static readonly byte _LoadScenario = 4;
    public static readonly byte _PlayerRestarted = 5;
    public static readonly byte _PlayersDefeated = 6;
    public static readonly byte _ValidateScene = 7;
  
    public static NetworkingEvents Instance { get; private set; }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            PhotonNetwork.OnEventCall += OnEvent;
        }
        else 
            Destroy(this);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            PhotonNetwork.OnEventCall -= OnEvent;
    }

    public void OnEvent(byte eventCode, object content, int senderId)
    {
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
            if (pv && pv.isMine)
            {
                PhotonNetwork.Destroy(pv);
            }
        }else if (eventCode == _OnPlayerDefeated)
        {         
            object[] data = (object[]) content;
            PhotonView pv = PhotonView.Find((int) data[0]);
           
            if (PhotonNetwork.isMasterClient)
            {
                int playerID = (int) data[1];
                ServerController.Instance.OnPlayerDefeated(playerID);
            }

            if (pv.isMine)
            {
                PhotonNetwork.Destroy(pv);
            }
        }else if (eventCode == _PlayerRestarted)
        {
            ServerController.Instance.ServerRestart();
        }else if (eventCode == _PlayersDefeated)
        {
            ServerController.Instance.DefeatCallback();
        }else if (eventCode == _LoadScenario)
        {
            object[] data = (object[]) content;
            string scene = (string) data[0];
            StartCoroutine(ServerController.Instance.LoadSceneAsync(scene));
        }else if (eventCode == _ValidateScene)
        {
            object[] data = (object[]) content;
            string scene = (string) data[0];
            
            if (SceneManager.GetActiveScene().name != scene)
            {
                StartCoroutine(ServerController.Instance.LoadSceneAsync(scene));
            }
        }
    }
    
}
