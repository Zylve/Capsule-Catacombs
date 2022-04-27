using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

// Title screen script.
public class startScene : MonoBehaviour
{
    // Load the lobby screen when the button is clicked.
    public void enterLobby()
    {
        SceneManager.LoadScene("Loading Screen");
    }

    // Change the nickname of the player when the input field is updated.
    public void updateNickname(string _nickname)
    {
        PhotonNetwork.NickName = _nickname;
        Debug.Log(PhotonNetwork.NickName);
    }
}