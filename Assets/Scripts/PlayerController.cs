using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 10f;
    public float jumpForce = 10f;
    public float jumpDist = 1.1f;

    public float mouseSenseX = 250f;
    public float mouseSenseY = 250f;

    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;

    public Transform cameraTransform;

    [SerializeField]
    float maxVertRotation = 60f;
    [SerializeField]
    float minVertRotation = -60f;
    public float verticalLookRotation = 0;

    Rigidbody rb;
    public bool isGrounded = true;
    public LayerMask groundedMask;

    bool lookEnabled = true;
    bool moveEnabled = true;

    public int health = 100;
    public AudioSource metalPipe;
    public AudioSource metalPipeAtHome;


    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lookEnabled)
        {
            transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSenseX);
            verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSenseY;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, minVertRotation, maxVertRotation);
            cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;
        }

        if (moveEnabled)
        {
            Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            float finalSpeed = isGrounded ? moveSpeed : moveSpeed * 0.8f;
            Vector3 targetMovement = moveDir * finalSpeed;
            moveAmount = Vector3.SmoothDamp(moveAmount, targetMovement, ref smoothMoveVelocity, .15f);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.AddForce(transform.up * jumpForce);
            }

            isGrounded = false;
            Ray[] rays = new Ray[5];
            rays[0] = new Ray(transform.position, -transform.up);
            rays[1] = new Ray(transform.position + 0.03f * transform.forward, -transform.up);
            rays[2] = new Ray(transform.position + 0.03f * -transform.forward, -transform.up);
            rays[3] = new Ray(transform.position + 0.03f * transform.right, -transform.up);
            rays[4] = new Ray(transform.position + 0.03f * -transform.right, -transform.up);

            RaycastHit hit;

            foreach (Ray ray in rays)
            {
                if (Physics.Raycast(ray, out hit, jumpDist, groundedMask))
                {
                    isGrounded = true;
                    break;
                }
            }
        }

    }

    private void FixedUpdate()
    {
        if (moveEnabled)
        {
            rb.MovePosition(transform.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        }
    }

    public void setLookEnabled(bool set)
    {
        lookEnabled = set;
    }

    public void setMoveEnabled(bool set)
    {
        moveEnabled = set;
    }

    public void Damage()
    {
        health--;

        int sound = Random.Range(0, 5);
        if(sound == 0)
        {
            metalPipeAtHome.Play();
        }
        else
        {
            metalPipe.Play();
        }

        if (health <= 0)
        {
            Debug.Log("GameOver!");
        }
    }
}
