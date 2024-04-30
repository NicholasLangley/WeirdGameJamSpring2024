using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabinet : MonoBehaviour
{
    [SerializeField]
    Snake snakeGame;

    [SerializeField]
    GameObject eye;

    [SerializeField]
    Camera playerCamera;
    [SerializeField]
    GameObject player;

    float time = 0;
    [SerializeField]
    float eyeAppearTime = 1.0f;

    bool rotating = false;
    Quaternion lookRotation;
    [SerializeField]
    float eyeRotateSpeed = 0.0001f;
    Quaternion lerpedRotation;
    float pupilLerpTime = 2.0f;

    bool doneLooking = false;
    [SerializeField]
    Animator eyelids;

    bool done = false;
    [SerializeField]
    float teleportTime = 1.5f;
    [SerializeField]
    float preSwallowTime = 3.0f;

    [SerializeField]
    GameObject arcade;
    [SerializeField]
    GameObject holeCover;

    bool runScript = true;

    public float activationDistance = 2.0f;
    public bool inRange = false;
    bool assuming = false;
    Vector3 playerStart;
    Vector3 playerEnd;
    float assumeTime = 1.0f;

    // Start is called before the first frame update
    void Awake()
    {
        eye.GetComponent<SkinnedMeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!runScript) { return; }
        if (snakeGame.gameComplete)
        {
            time += Time.deltaTime;
            playEye();
        }
        else if (assuming)
        {
            assumeThePosition();
        }
        else
        {
            if((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F)) && !snakeGame.started && !assuming)
            {
                RaycastHit hit;
                inRange = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.TransformDirection(Vector3.forward), out hit, activationDistance);
                if(inRange)
                {
                    player.GetComponent<PlayerController>().setLookEnabled(false);
                    player.GetComponent<PlayerController>().setMoveEnabled(false);

                    playerStart = player.transform.position;

                    playerEnd = transform.position + transform.forward;
                    playerEnd.y = player.transform.position.y;


                    assuming = true;                  
                }
            }
            
        }
    }

    void assumeThePosition()
    {
        time += Time.deltaTime;
        player.transform.position = Vector3.Lerp(playerStart, playerEnd, time / assumeTime);
        player.transform.LookAt(new Vector3 (eye.transform.position.x, player.transform.position.y, eye.transform.position.z));
        playerCamera.transform.LookAt(new Vector3(eye.transform.position.x, eye.transform.position.y, eye.transform.position.z));


        if (time >= assumeTime) { snakeGame.startGame(); time = 0; assuming = false; }
    }

    void playEye()
    {
        if (!rotating)
        {
            eye.GetComponent<SkinnedMeshRenderer>().enabled = true;
            float z = Mathf.Lerp(-0.75f, 0.75f, time / eyeAppearTime);
            eye.transform.localPosition = new Vector3(eye.transform.localPosition.x, eye.transform.localPosition.y, z);

            if (time > eyeAppearTime)
            {
                eye.transform.rotation = new Quaternion(0, 0, 0, 0);
                Vector3 lookDirection = playerCamera.transform.position - eye.transform.position;
                Vector3 eyeForward = eye.transform.TransformDirection(eye.transform.forward);
                lookRotation = Quaternion.FromToRotation(eyeForward, lookDirection);
                lerpedRotation = eye.transform.rotation;

                //eye.transform.LookAt(playerCamera.transform);
                //eye.transform.Rotate(-90, 0, 0);
                rotating = true;
                time = 0f;
            }
        }
        //rotating to look at player
        else if (!doneLooking)
        {
            lerpedRotation = Quaternion.Lerp(lerpedRotation, lookRotation, eyeRotateSpeed);
            eye.transform.rotation = lerpedRotation;
            eye.transform.Rotate(-90, 0, 0);
            eye.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, Mathf.Lerp(0, 100, time / pupilLerpTime));
            eye.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(1, Mathf.Lerp(0, 50, time / pupilLerpTime));
            if (time > pupilLerpTime) { doneLooking = true; eyelids.Play("Blinking"); time = 0; }
        }
        else if (!done)
        {
            if (time >= teleportTime)
            {
                arcade.transform.Rotate(-90f, 0f, 0f);
                player.GetComponent<GravityMass>().enable();
                player.GetComponent<GravityMass>().setSource(eye.GetComponent<GravitySource>());
                player.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                player.GetComponent<PlayerController>().moveSpeed = 0.1f;
                player.GetComponent<PlayerController>().jumpForce = 2.5f;
                player.GetComponent<PlayerController>().jumpDist = 0.011f;
                playerCamera.transform.localEulerAngles = Vector3.left * -80f;

                player.GetComponent<PlayerController>().setLookEnabled(true);
                player.GetComponent<PlayerController>().setMoveEnabled(true);


                eye.transform.localPosition = new Vector3(eye.transform.localPosition.x, eye.transform.localPosition.y, 0.25f);
                player.transform.position = new Vector3(eye.transform.position.x, eye.transform.position.y + 0.25f, eye.transform.position.z);

                GameObject.Destroy(holeCover);

                done = true;
                time = 0;
            }
        }
        else 
        {
            if (time > preSwallowTime)
            {
                eye.transform.LookAt(player.transform);
                eye.transform.Rotate(-90, 0, 0);

                eye.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, Mathf.Lerp(100, 0, (time - preSwallowTime) / pupilLerpTime));
                eye.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(1, Mathf.Lerp(50, 0, (time - preSwallowTime) / pupilLerpTime));

                float playerScaleLerp = Mathf.Lerp(0.01f, 0.001f, (time - preSwallowTime) / pupilLerpTime);
                player.transform.localScale = new Vector3(playerScaleLerp, playerScaleLerp, playerScaleLerp);
            }
            if(time > preSwallowTime + pupilLerpTime)
            {
                eye.layer = LayerMask.NameToLayer("Default");
                eye.GetComponent<SphereCollider>().enabled = false;
                player.GetComponent<GravityMass>().disable();
            }
            if (time > preSwallowTime + pupilLerpTime + 0.5f)
            {
                player.GetComponent<GravityMass>().disable();
                player.transform.rotation = Quaternion.Euler(0f, player.transform.rotation.eulerAngles.y, 0f);
                gameObject.SetActive(false);
                runScript = false;
                GameObject.Destroy(gameObject);
            }
        }
    }
}
