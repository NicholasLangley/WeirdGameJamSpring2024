using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySource : MonoBehaviour
{
    public float gravity = -1.0f;

    public void affectMass(Transform mass, bool isPlayer)
    {
        //faces mass upward based on center of gravitational body
        Vector3 targetOrientation = (mass.position - transform.position).normalized;
        Vector3 currentOrientation = mass.up;

        if (isPlayer) { mass.rotation = Quaternion.FromToRotation(currentOrientation, targetOrientation) * mass.rotation; }
        mass.GetComponent<Rigidbody>().AddForce(targetOrientation * gravity);
    }

}
