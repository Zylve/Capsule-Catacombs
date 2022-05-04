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
        playerScore = pView.gameObject.GetComponent<playerController>().score;
    }
}