using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class playerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Player Values")]
    public float moveSpeed;
    public float jumpHeight;
    public int health = 100;
    public int score = 0;

    [Header("Debug")]
    [SerializeField] private float xInput;
    [SerializeField] private float zInput;
    [SerializeField] Vector3 velocity;
    [SerializeField] float groundDistance;
    [SerializeField] LayerMask groundMask;
    [SerializeField] bool isGrounded;
    private CharacterController cController;
    private Vector3 movementVector;
    private Transform groundCheck;
    private PhotonView view;
    public bool isPaused = false;

    [Header("UI & Micellaneous")]
    public TextMeshProUGUI textHealth;
    public TextMeshProUGUI textScore;
    public GameObject pauseMenu;
    private Canvas canvas;
    private Renderer[] visuals;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(score);
        }
        else
        {
            health = (int)stream.ReceiveNext();
            score = (int)stream.ReceiveNext();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        groundCheck = transform.Find("Ground Check").transform;
        pauseMenu.SetActive(false);

        view = GetComponent<PhotonView>();

        canvas = GetComponentInChildren<Canvas>();
        

        cController = GetComponent<CharacterController>();
        visuals = GetComponentsInChildren<Renderer>();

        if(!view.IsMine)
        {
            canvas.enabled = false;
        }

        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(view.IsMine == true)
        {
            movePlayer();
        }
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            isPaused = !isPaused;
            if(isPaused)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;

                pauseMenu.SetActive(true);

                GetComponentInChildren<mouseLook>().isPaused = isPaused;
                GetComponentInChildren<gunController>().isPaused = isPaused;
            }else{
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;

                pauseMenu.SetActive(false);

                GetComponentInChildren<mouseLook>().isPaused = isPaused;
                GetComponentInChildren<gunController>().isPaused = isPaused;

            }
        }
    }
    private void movePlayer()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        xInput = 0;
        zInput = 0;
        if(isGrounded == true && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if(isPaused == false)
        {
            if(Input.GetButtonDown("Jump") && isGrounded == true)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * -9.81f);
            }

            xInput = Input.GetAxisRaw("Horizontal");
            zInput = Input.GetAxisRaw("Vertical");
        }
        movementVector = transform.right * xInput + transform.forward * zInput;
        cController.Move(movementVector.normalized * moveSpeed * Time.deltaTime);

        velocity.y += -9.81f * Time.deltaTime;

        cController.Move(velocity * Time.deltaTime);
    }

    private void setRenderers(bool state)
    {
        foreach(Renderer renderer in visuals)
        {
            renderer.enabled = state;
        }
    }

    IEnumerator respawn()
    {
        setRenderers(false);
        GetComponent<CharacterController>().enabled = false;
        transform.position = new Vector3(Random.Range(-20, 20), 5, Random.Range(-20, 20));
        yield return new WaitForSeconds(1f);
        health = 100;
        textHealth.text = ($"Health: {health}%");
        GetComponent<CharacterController>().enabled = true;
        setRenderers(true);
    }

    public void giveScore()
    {
        score++;
        textScore.text = ($"Score: {score}");
    }

    public void healthMethod(int damage, GameObject gameObj, bool addsHealth)
    {
        health += damage;
        textHealth.text = ($"Health: {health}");
        if(health <= 0)
        {
            gameObj.transform.root.GetComponent<playerController>().giveScore();
            StartCoroutine(respawn());
        }
    }
}