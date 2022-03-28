using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class gameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-2, 93), 5, Random.Range(-20, 77));
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
    
    }
}
