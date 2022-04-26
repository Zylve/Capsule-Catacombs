using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

// Manages joining and creating Photon Rooms.
public class roomManager : MonoBehaviourPunCallbacks
{
    // References the error text.
    public GameObject roomCodeError;

    // References the Room Code Input Field.
    public TextMeshProUGUI roomCode;
    
    // Initializes a string to store the Room Code.
    public string code;

    // References the Lobby creation and In Room gameobjects.
    public GameObject creationScene;
    public GameObject lobbyScene;

    // References UI components
    [Header("UI components")]
    public TextMeshProUGUI playerCount; 
    public TextMeshProUGUI roomName;
    public GameObject startGame;

    // Creates the room when the create button is clicked.
    public void createRoom()
    {
        if(getRoomCodeInput() == null)
        {

        }else{
            PhotonNetwork.CreateRoom(getRoomCodeInput());
        }
    }

    // Joins the room when the join button is clicked.
    public void joinRoom()
    {
        if(getRoomCodeInput() == null)
        {

        }else{
            PhotonNetwork.JoinRoom(getRoomCodeInput());
        }
    }

    public void loadMap()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }

    // Enters the lobby.
    public override void OnJoinedRoom()
    {
        // Swap lobby/room screens.
        creationScene.SetActive(false);
        lobbyScene.SetActive(true);

        // Set the room name.
        roomName.text = $"Room Name: {PhotonNetwork.CurrentRoom.Name}";

        // Updates the player count.
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();

        // Hides the "Start Game" button if the player is not the Master Client
        if(!PhotonNetwork.IsMasterClient)
        {
            startGame.SetActive(false);
        }
    }

    // Update the player count when someone joins or leaves the room. Written this way to avoid spamming the server with unnecessary requests.
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }

    // Leave the room when the button is clicked.
    public void leaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    // Swaps the UI elements once the room is left.
    public override void OnLeftRoom()
    {
        creationScene.SetActive(true);
        lobbyScene.SetActive(false);
    }

    // Rejoins the lobby so that you are able to reconnect to other rooms.
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    // Gets the value of the input field. If it is not null, return the string in lowercase.
    private string getRoomCodeInput()
    {
        code = roomCode.text;
        if(code == null)
        {
            displayError();
            return null;
        }else{
            return code.ToLower();
        }
    }

    // Displays an error that tells the player that the Code is invalid
    private void displayError()
    {
        roomCodeError.SetActive(true);
    }

    // Start method. Anything that needs to be done upon loading the scene will be put here
    public void Start()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }
}