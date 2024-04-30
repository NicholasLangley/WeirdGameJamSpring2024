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
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            active = true;
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
        float x = transform.position.x + Random.Range(200f, 400f);
        float z = transform.position.z + Random.Range(200f, 400f);

        int neg = Random.Range(0, 2);
        x *= -1 * neg;

        neg = Random.Range(0, 2);
        z *= -1 * neg;

        transform.position = new Vector3(x, transform.position.y, z);
    }
}
