using System.Runtime.CompilerServices;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class gunController : MonoBehaviour
{

    [Header("Gun Statistics")]
    public float fireRate;
    public int damage = 10;
    private Camera cam;
    private ParticleSystem pSystemShoot;
    private Animator anim;
    private AudioSource aSource;
    private PhotonView view;
    private LineRenderer lRender;
    public bool isPaused = false;
    public AudioClip aClip;

    [Header("Debug")]
    [SerializeField] private float nextFireTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        cam = transform.parent.GetComponent<Camera>();
        pSystemShoot = GetComponentInChildren<ParticleSystem>();
        lRender = GetComponent<LineRenderer>();
        aSource = GetComponentInParent<mouseLook>().aSource;
        view = GetComponent<PhotonView>();

        lRender.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(view.IsMine == true)
        {
            if(Input.GetButton("Fire1") && Time.time >= nextFireTime && isPaused == false)
            {
                nextFireTime = Time.time + 1f/fireRate;
                view.RPC("shoot", RpcTarget.All);
            }
        }
    }
    [PunRPC]
    private void shoot()
    {
        pSystemShoot.Play();
        StartCoroutine(playAnim());
        aSource.PlayOneShot(aClip, 1);
        RaycastHit hit;
        if(Physics.Raycast(cam.gameObject.transform.position, cam.transform.forward, out hit, 100))
        {
            playerController playerController = hit.transform.GetComponent<playerController> ();
            if(playerController != null && playerController != GetComponentInParent<playerController>())
            {
                playerController.healthMethod(damage, gameObject, false);
            }
            StartCoroutine(drawLine(hit));
        }
    }
    public IEnumerator playAnim()
    {
        anim.SetBool("isRecoiling", true);
        yield return new WaitForSeconds(0.167f);
        anim.SetBool("isRecoiling", false);
        yield break;
    }
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