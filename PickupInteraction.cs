using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteraction : NetworkBehaviour
{
    [SerializeField] string nucleonType; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                other.GetComponent<PlayerAction>().PickupItem(nucleonType, this.GetComponent<NetworkIdentity>().netId); 
            }
        }
    }
}
