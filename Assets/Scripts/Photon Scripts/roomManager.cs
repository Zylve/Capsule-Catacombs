using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class roomManager : MonoBehaviourPunCallbacks
{    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void createRoom()
    {
        PhotonNetwork.CreateRoom("Test Room");
    }

    public void joinRoom()
    {
        PhotonNetwork.JoinRoom("Test Room");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}