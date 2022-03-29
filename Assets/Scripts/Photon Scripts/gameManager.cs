using UnityEngine;
using Photon.Pun;

/*
    Script is run upon loading the Game scene.
    Creates a new player instance across the network.
*/

public class gameManager : MonoBehaviour
{
    // Creates a reference to the Player prefab through the inspector.
    public GameObject playerPrefab;

    void Start()
    {
        // Choose a new random Vector3 position to spawn the player in.
        Vector3 spawnPos = new Vector3(Random.Range(-2, 93), 5, Random.Range(-20, 77));

        // Instantiates the player over the network, at the above position, with no rotation.
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
    
    }
}