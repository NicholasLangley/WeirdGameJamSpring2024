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

    private void Awake()
    {
        GetComponent<Rigidbody>().useGravity = false;
        if (isPlayer) { GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation; }
    }

    private void FixedUpdate()
    {
        gSource.affectMass(transform, isPlayer);
    }

    private void Update()
    {
        
    }

}