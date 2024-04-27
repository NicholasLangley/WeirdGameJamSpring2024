using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityMass : MonoBehaviour
{
    [SerializeField]
    GravitySource gSource;
    
    [SerializeField]
    bool isPlayer = false;

    public bool isEnabled = true;

    private void Awake()
    {
        if (isEnabled) { GetComponent<Rigidbody>().useGravity = false; }
        if (isPlayer) { GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation; }
    }

    private void FixedUpdate()
    {
        if (isEnabled && gSource != null) { gSource.affectMass(transform, isPlayer); }
    }

    public void enable()
    {
        isEnabled = true;
        GetComponent<Rigidbody>().useGravity = false;
    }

    public void disable()
    {
        isEnabled = true;
        GetComponent<Rigidbody>().useGravity = true;
    }

}