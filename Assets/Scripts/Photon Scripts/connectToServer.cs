using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;


// Connects to the Master server.
public class connectToServer : MonoBehaviourPunCallbacks
{
    // Connects to the Photon Server
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // Joins the lobby once connected
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    // Once joined, loads the room manager scene.
    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("create or join");
    }
}
