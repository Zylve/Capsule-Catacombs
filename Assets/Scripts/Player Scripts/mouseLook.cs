using UnityEngine;
using Photon.Pun;

// Essentially a camera controller. Allows you to look around
public class mouseLook : MonoBehaviour
{
    // Mouse sensitivity.
    [Header("Camera Values")]
    public float mouseSensitivity;
    [Header("Debug")]
    // Mouse inputs.
    [SerializeField] private float mouseX;
    [SerializeField] private float mouseY;

    // Variable that stores the required rotation degrees of the camera.
    private float xRotation = 0f;

    // Component References.
    [SerializeField] private Camera cam;
    [SerializeField] private AudioListener aListener;
    public AudioSource aSource;
    private PhotonView view;
    public bool isPaused = false;
    private Transform playerTransform;

    void Start()
    {
        // Locks the cursor.
        Cursor.lockState = CursorLockMode.Locked;

        // Gets the components.
        playerTransform = transform.root.GetComponent<Transform>();

        aSource = GetComponent<AudioSource>();
        view = GetComponent<PhotonView>();

        // Checks if the PhotonView is ours, if so, disable elements from other players.
        if(!view.IsMine)
        {
            cam.enabled = false;
            aListener.enabled = false;
        }
    }

    void Update()
    {
        // Checks if the view is ours, if so, run the playerRotate method if the game is not paused.
        if(view.IsMine == true)
        {
            if(isPaused == false)
            {
                playerRotate();
            }
        }
    }

    private void playerRotate()
    {
        // Get the mouse inputs.
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Gets the rotation amount.
        xRotation -= mouseY;

        // Clamp the rotation.
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Locally rotate the camera.
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // Rotate the entire player on the Y axis.
        playerTransform.Rotate(Vector3.up * mouseX);
    }
}
