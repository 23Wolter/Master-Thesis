using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalJump : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Portal"))
        {
            PortalManager portal = other.GetComponent<PortalManager>();
            transform.position = portal.GetDestination().position;
            GetComponent<PlayerAction>().enabled = true; 
        }
    }
}
