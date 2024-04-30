using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkKing : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    
    bool active = false;

    public float speed = 1.0f;

    // Start is called before the first frame update
    void Awake()
    {
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(active)
        {
            transform.LookAt(player.transform.position);
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed);
            transform.Rotate(0, 90, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            Teleport();
            other.gameObject.GetComponent<PlayerController>().Damage();
        }
    }

    void Teleport()
    {
        float x = transform.position.x + Random.Range(200f, 500f);
        float z = transform.position.z + Random.Range(200f, 500f);

        int neg = Random.Range(0, 2);
        if(neg == 0)
        {
            x *= -1f;
        }

        neg = Random.Range(0, 2);
        if (neg == 0)
        {
            z *= -1f;
        }

        transform.position = new Vector3(x, transform.position.y, z);
    }

    public void activate()
    {
        active = true;
    }
}
