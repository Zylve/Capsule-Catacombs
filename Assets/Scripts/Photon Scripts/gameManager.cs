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
        Vector3 spawnPos = new Vector3(Random.Range(-20, 20), 3, Random.Range(-20, 20));
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
    }
}
