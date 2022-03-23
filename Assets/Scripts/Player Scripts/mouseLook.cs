using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class mouseLook : MonoBehaviour
{
    [Header("Camera Values")]
    public float mouseSensitivity;
    [Header("Debug")]
    [SerializeField] private float mouseX;
    [SerializeField] private float mouseY;
    private float xRotation = 0f;
    [SerializeField] private Camera cam;
    [SerializeField] private AudioListener aListener;
    private PhotonView view;
    public bool isPaused = false;
    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerTransform = transform.root.GetComponent<Transform>();
        view = GetComponent<PhotonView>();
        if(!view.IsMine)
        {
            cam.enabled = false;
            aListener.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
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
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerTransform.Rotate(Vector3.up * mouseX);
    }
}
