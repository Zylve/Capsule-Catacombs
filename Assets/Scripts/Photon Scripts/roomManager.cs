using Photon.Pun;

// Manages joining and creating Photon Rooms.
public class roomManager : MonoBehaviourPunCallbacks
{

    // Creates the room when the create button is clicked.
    public void createRoom()
    {
        PhotonNetwork.CreateRoom("Test Room");
    }

    // Joins the room when the join button is clicked.
    public void joinRoom()
    {
        PhotonNetwork.JoinRoom("Test Room");
    }

    // Once the client joins a room, load the main scene.
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}