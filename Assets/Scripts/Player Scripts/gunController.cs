using System.Collections;
using UnityEngine;
using Photon.Pun;

// Includes all the necessary code for the gun.
public class gunController : MonoBehaviour
{
    // Statistics such as the firerate and damage.
    [Header("Gun Statistics")]
    public float fireRate;
    public int damage = 10;

    // References to the necessary components.
    private Camera cam;
    private ParticleSystem pSystemShoot;
    private Animator anim;
    private AudioSource aSource;
    private PhotonView view;
    private LineRenderer lRender;
    public AudioClip aClip;

    // Boolean for checking whether the game is paused.
    public bool isPaused = false;
    public bool canControl = false;

    [Header("Debug")]

    // Checks for whether the gun has bean cocked yet.
    [SerializeField] private float nextFireTime = 0f;

    void Start()
    {
        // Gets references to the components.
        anim = gameObject.GetComponent<Animator>();
        cam = transform.parent.GetComponent<Camera>();
        pSystemShoot = GetComponentInChildren<ParticleSystem>();
        lRender = GetComponent<LineRenderer>();
        aSource = GetComponentInParent<mouseLook>().aSource;
        view = GetComponent<PhotonView>();

        // Disables the line renderer.
        lRender.enabled = false;
    }

    void Update()
    {
        // Checks if the PhotonView is ours.
        if(view.IsMine == true && canControl == true)
        {
            // Checks if we have clicked / pressed the fire key.
            if(Input.GetButton("Fire1") && Time.time >= nextFireTime && isPaused == false)
            {
                // Resets the fire time.
                nextFireTime = Time.time + 1f/fireRate;
                
                // Send out a Shoot RPC.
                view.RPC("shoot", RpcTarget.All);

                // Increments shot count.
                transform.root.GetComponent<playerController>().shots++;
            }
        }
    }

    // Shoot RPC.
    [PunRPC]
    private void shoot()
    {
        // Plays some particles.
        pSystemShoot.Play();

        // Plays a co-routine for the recoil animation.
        StartCoroutine(playAnim());

        // Plays the gunshot sound effect.
        aSource.PlayOneShot(aClip, 1);

        // Raycast shooting.
        RaycastHit hit;

        // Checks if the raycast is within 100 metres.
        if(Physics.Raycast(cam.gameObject.transform.position, cam.transform.forward, out hit, 100))
        {
            // Checks if you hit another player. If so, get their playerController script and decrease their health.
            playerController playerController = hit.transform.GetComponent<playerController> ();
            if(playerController != null && playerController != transform.root.GetComponent<playerController>())
            {
                playerController.healthMethod(damage, gameObject);
                transform.root.GetComponent<playerController>().hits++;
            }

            // Starts the co-routine for drawing the line effect
            StartCoroutine(drawLine(hit));
        }
    }

    // Recoil animation co-routine.
    public IEnumerator playAnim()
    {
        anim.SetBool("isRecoiling", true);
        yield return new WaitForSeconds(0.167f);
        anim.SetBool("isRecoiling", false);
        yield break;
    }

    // Line renderer co-routine.
    public IEnumerator drawLine(RaycastHit hit)
    {
        lRender.enabled = true;
        lRender.SetPosition(0, pSystemShoot.transform.position);
        lRender.SetPosition(1, hit.point);
        yield return new WaitForSeconds(0.05f);
        lRender.enabled = false;
        yield break;
    }
}