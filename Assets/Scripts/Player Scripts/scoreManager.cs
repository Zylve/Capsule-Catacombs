using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class scoreManager : MonoBehaviourPunCallbacks
{
    public GameObject leaderboardBG;
    public GameObject defaultInfo;
    public List<playerInfo> playerList = new List<playerInfo>();

    private void Update()
    {
        
        var photonViews = UnityEngine.Object.FindObjectsOfType<PhotonView>();
        foreach(var pView in photonViews)
        {
            var player = pView.Owner;
            if(player!=null)
            {
                playerInfo pInfo = new playerInfo(pView);
                playerList.Add(pInfo);
            }
        }
        playerList.Sort(new Comparison<playerInfo>((x, y) => x.playerScore.CompareTo(y.playerScore)));
        foreach (var item in playerList)
        {
            Debug.Log(item.playerView.name + item.playerScore);
        }
    }
}


// OOP yayaya
public class playerInfo
{
    public PhotonView playerView;
    public int playerScore;

    public playerInfo(PhotonView pView)
    {
        playerView = pView;
        playerScore = pView.transform.root.GetComponent<playerController>().score;
    }
}