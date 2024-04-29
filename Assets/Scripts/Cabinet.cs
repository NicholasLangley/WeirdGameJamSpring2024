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

    float time = 0;
    [SerializeField]
    float eyeAppearTime = 1.0f;

    bool rotating = false;
    Quaternion lookRotation;
    [SerializeField]
    float eyeRotateSpeed = 0.0001f;
    Quaternion lerpedRotation;
    float pupilLerpTime = 2.0f;

    // Start is called before the first frame update
    void Awake()
    {
        eye.GetComponent<SkinnedMeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (snakeGame.gameComplete)
        {
            time += Time.deltaTime;
            playEye();
        }
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
        else
        {
            lerpedRotation = Quaternion.Lerp(lerpedRotation, lookRotation, eyeRotateSpeed);
            eye.transform.rotation = lerpedRotation;
            eye.transform.Rotate(-90, 0, 0);
            eye.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, Mathf.Lerp(0, 100, time / pupilLerpTime));
            eye.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(1, Mathf.Lerp(0, 50, time / pupilLerpTime));
        }
    }
}
