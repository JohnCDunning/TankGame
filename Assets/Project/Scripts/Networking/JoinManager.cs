using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinManager : MonoBehaviour
{
    [SerializeField] private Transform _SpawnPosition;
    
    void OnJoinedRoom()
    {
        if (PhotonNetwork.isMasterClient == false)
        {
            return;
        }

        GameObject playerObject = PhotonNetwork.InstantiateSceneObject("Player", _SpawnPosition.position, _SpawnPosition.rotation, 0, null);
    }
}
