using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

// Player controller. This controls the player. (Obviously)
public class playerController : MonoBehaviourPunCallbacks, IPunObservable
{

    // Player statistics, controls different variables of the player.
    [Header("Player Values")]
    public float moveSpeed;
    public float jumpHeight;
    public int health = 100;
    public int score = 0;
    public int shots = 0;
    public int hits = 0;

    // Debug values, nothing more than just current positions and velocities.
    [Header("Debug")]
    [SerializeField] private float xInput;
    [SerializeField] private float zInput;
    [SerializeField] Vector3 velocity;
    [SerializeField] float groundDistance;
    [SerializeField] LayerMask groundMask;
    [SerializeField] bool isGrounded;
    public float timeUntilEnd = 10;

    // Component References.
    public CharacterController cController;
    private Vector3 movementVector;
    private Transform groundCheck;
    private PhotonView view;

    // Boolean for checking whether the game is paused.
    public bool isPaused = false;
    public bool canControl = true;

    // User Interface and other component references.
    [Header("UI & Micellaneous")]
    public TextMeshProUGUI textHealth;
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textNickname;
    public GameObject pauseMenu;
    public Camera miniMapCam;
    public TextMeshProUGUI matchTimer;
    public GameObject endGameBackground;
    private Canvas canvas;
    private Renderer[] visuals;
    private Light playerLight;

    // Writes and recieves the health and score values to and from the cloud.
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(score);
            stream.SendNext(shots);
            stream.SendNext(hits);
            if(PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(timeUntilEnd);
            }
        }
        else
        {
            health = (int)stream.ReceiveNext();
            score = (int)stream.ReceiveNext();
            shots = (int)stream.ReceiveNext();
            hits = (int)stream.ReceiveNext();
            timeUntilEnd = (float)stream.ReceiveNext();
        }
    }


    void Start()
    {
        // Creates sphere for gravity / ground checking
        groundCheck = transform.Find("Ground Check").transform;

        // Deactivates the pause menu
        pauseMenu.SetActive(false);

        // Gets references to the components.
        view = GetComponent<PhotonView>();

        playerLight = GetComponentInChildren<Light>();
        canvas = GetComponentInChildren<Canvas>();
        visuals = GetComponentsInChildren<Renderer>();

        textNickname.text = PhotonNetwork.NickName;

        // Checks if the PhotonView is ours, if it isn't, disable elements from other player.s
        if(!view.IsMine)
        {
            miniMapCam.enabled = false;
            canvas.enabled = false;
            playerLight.enabled = false;
        }else{
            transform.Find("Nickname canvas").Find("Nickname").gameObject.SetActive(false);
        }

        // Resets the score.
        score = 0;

        // Starts the match timer.
        StartCoroutine(decrementTimer());
    }

    void Update()
    {
        // Checks if the score is ours, if so, then call the movePlayer method.
        if(view.IsMine == true && canControl == true)
        {
            movePlayer();
        }
        pauseGame();
    }

    // Checks for the escape key.
    private void pauseGame()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && canControl == true)
        {
            // Flips the isPaused boolean.
            isPaused = !isPaused;

            // If it is paused, show the pause menu.
            if(isPaused)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;

                pauseMenu.SetActive(true);

                GetComponentInChildren<mouseLook>().isPaused = isPaused;
                GetComponentInChildren<gunController>().isPaused = isPaused;

            // If it is not paused, hide it.
            }else{
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;

                pauseMenu.SetActive(false);

                GetComponentInChildren<mouseLook>().isPaused = isPaused;
                GetComponentInChildren<gunController>().isPaused = isPaused;
            }
        }
    }

    // Moves the player.
    private void movePlayer()
    {
        // Checks if the player should be able to jump and whether gravity should take place.
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Resets the input variables.
        xInput = 0;
        zInput = 0;

        // Applies a small force if the player is grounded. This tends to make the phyiscs perform slightly better.
        if(isGrounded == true && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // If the game is not paused, run this code.
        if(isPaused == false)
        {
            // Checks for the jump key.
            if(Input.GetButtonDown("Jump") && isGrounded == true)
            {
                // Applies physics calculations to get the desired jump height.
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * -9.81f);
            }

            // Gets the player input.
            xInput = Input.GetAxisRaw("Horizontal");
            zInput = Input.GetAxisRaw("Vertical");
        }

        // Moves the player.
        movementVector = transform.right * xInput + transform.forward * zInput;
        cController.Move(movementVector.normalized * moveSpeed * Time.deltaTime);

        // Gravity formula.
        velocity.y += -9.81f * Time.deltaTime;

        // Applis gravity to the player.
        cController.Move(velocity * Time.deltaTime);
    }

    // Enables / disables the renderers for respawning.
    private void setRenderers(bool state)
    {
        foreach(Renderer renderer in visuals)
        {
            renderer.enabled = state;
        }
    }

    // Respawn co-routine.
    IEnumerator respawn()
    {
        // Makes the player invisible.
        setRenderers(false);

        // Disables the character controller.
        GetComponent<CharacterController>().enabled = false;

        // Sets the transform to a random location slightly above the ground.
        transform.position = new Vector3(Random.Range(-3.5f, 95), 5, Random.Range(-20, 77));

        yield return new WaitForSeconds(1f);

        // Resets the player's health.
        health = 100;
        textHealth.text = ($"Health: {health}");

        // Reenables the character controller.
        GetComponent<CharacterController>().enabled = true;

        // Makes the player visible again.
        setRenderers(true);
    }

    // Decrements the timer every second and prevents control after it has run out.
    IEnumerator decrementTimer()
    {
        canControl = true;
        while(timeUntilEnd != 0)
        {
            yield return new WaitForSecondsRealtime(1);
            timeUntilEnd--;
            matchTimer.text = $"Time left: {timeUntilEnd}";
        }
        canControl = false;
        GetComponentInChildren<gunController>().canControl = false;
        GetComponentInChildren<mouseLook>().canControl = false;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        endGameBackground.SetActive(true);
        endGameBackground.transform.Find("score").GetComponent<TextMeshProUGUI>().text = $"Score: {score}";
        yield break;
    }

    // Gives score to the player who killed you.
    public void giveScore()
    {
        score++;
        textScore.text = ($"Score: {score}");
    }

    // Regulates the player health
    public void healthMethod(int damage, GameObject gameObj)
    {
        // Subtracts health if shot.
        health -= damage;
        textHealth.text = ($"Health: {health}");

        // If the player has 0 health, add a point to the killer and run the respawn co-routine.
        if(health <= 0)
        {
            gameObj.transform.root.GetComponent<playerController>().giveScore();
            StartCoroutine(respawn());
        }
    }

    // Returns to the Main Menu
    public void returnToMenu()
    {
        SceneManager.LoadScene("Start Scene");
        PhotonNetwork.Disconnect();
    }
}