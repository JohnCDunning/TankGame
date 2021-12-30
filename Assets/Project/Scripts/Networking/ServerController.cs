using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerController : Photon.MonoBehaviour
{
    public static ServerController Instance { get; private set; }

    private Dictionary<PhotonPlayer, bool> _PlayersAliveDictionary = new Dictionary<PhotonPlayer, bool>();

    [SerializeField] private GameObject _Canvas;
    private string _CurrentScene = "";
    
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _CurrentScene = SceneManager.GetActiveScene().name;
            DontDestroyOnLoad(this.gameObject);
        }else 
            Destroy(this.gameObject);
    }

    [PunRPC]
    public void ValidateHostScene(string currentHostScene)
    {
        if (SceneManager.GetActiveScene().name != currentHostScene)
        {
            StartCoroutine(LoadSceneAsync(currentHostScene));
        }
    }
    
    public void PlayerConnected(PhotonPlayer otherPlayer)
    {
        if (!_PlayersAliveDictionary.ContainsKey(otherPlayer))
        {
            _PlayersAliveDictionary.Add(otherPlayer, true);
        }
        else
            _PlayersAliveDictionary[otherPlayer] = true;
    }
    public virtual void OnJoinedRoom()
    {
        if (PhotonNetwork.isMasterClient)
            PlayerConnected(PhotonNetwork.player);
    }
    
    public void PlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (_PlayersAliveDictionary.ContainsKey(otherPlayer))
        {
            _PlayersAliveDictionary.Remove(otherPlayer);
        }

        CheckPlayerStatus();
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (PhotonNetwork.isMasterClient)
        {
            PlayerDisconnected(otherPlayer);
        }
    }
    public virtual void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.isMasterClient)
        {
            PlayerConnected(newPlayer);
            
            string scene = SceneManager.GetActiveScene().name;
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.Others};
            PhotonNetwork.RaiseEvent(NetworkingEvents._ValidateScene, new object[] {scene}, true, raiseEventOptions);
        }


    }

    public void CheckPlayerStatus()
    {
        bool allAreDefeated = true;
        foreach (var playerPair in _PlayersAliveDictionary)
        {
            if (playerPair.Value)
            {
                allAreDefeated = false;
                break;
            }
        }

        if (allAreDefeated)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(NetworkingEvents._PlayersDefeated, null, true, raiseEventOptions);
        }
    }

    public void OnPlayerDefeated(int playerID)
    {
        if (!PhotonNetwork.isMasterClient)
            return;
        
        PhotonPlayer player = PhotonNetwork.playerList.First(i => i.ID == playerID);
        if (!_PlayersAliveDictionary.ContainsKey(player))
        {
            _PlayersAliveDictionary.Add(player, false);
        }
        else
            _PlayersAliveDictionary[player] = false;

        CheckPlayerStatus();


    }

    public void DefeatCallback()
    {
        _Canvas.SetActive(true);
    }

    public void Restart()
    {
        _Canvas.SetActive(false);
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(NetworkingEvents._PlayerRestarted, null, true, raiseEventOptions);
    }

    public IEnumerator LoadSceneAsync (string scene)
    {
        
        PhotonNetwork.LoadLevel(scene);
        yield return new WaitForFixedUpdate();

        _CurrentScene = scene;
        
        GameObject.FindObjectOfType<OnJoinedInstantiate>().OnJoinedRoom();
        
        _Canvas.SetActive(false);
        
    }

    public void ServerRestart()
    {
        if (PhotonNetwork.isMasterClient)
        {
            List<PhotonPlayer> tempContainer = _PlayersAliveDictionary.Keys.ToList();
            _PlayersAliveDictionary.Clear();
            // respawn players
            foreach (var player in tempContainer)
            {
                _PlayersAliveDictionary.Add(player, true);
            }

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            string scene = SceneManager.GetActiveScene().name;
            PhotonNetwork.RaiseEvent(NetworkingEvents._LoadScenario, new object[]{scene} , true, raiseEventOptions);

        }
        _Canvas.SetActive(false);
    }
    
}
