using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);

        other.gameObject.GetComponent<PlayerController>().setLookEnabled(true);
        other.gameObject.GetComponent<PlayerController>().setMoveEnabled(true);
        other.gameObject.GetComponent<PlayerController>().moveSpeed = 10f;
        other.gameObject.GetComponent<PlayerController>().jumpForce = 250f;
        other.gameObject.GetComponent<PlayerController>().jumpDist = 1.1f;
    }
}
