using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField]
    ForkKing James;
    [SerializeField]
    ForkKing Anya;
    [SerializeField]
    ForkKing SanFrancisco;
    [SerializeField]
    ForkKing Arson;

    [SerializeField]
    GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            James.activate();
            Anya.activate();
            SanFrancisco.activate();
            Arson.activate();
        }
    }
}
