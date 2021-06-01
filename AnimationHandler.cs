using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : NetworkBehaviour
{

    [SerializeField] Animator anim; 

    void Update()
    {
        //if (!transform.root.GetComponent<NetworkIdentity>().isLocalPlayer) return; 

        //if(Input.GetKey(KeyCode.W)
        //    || Input.GetKey(KeyCode.A)
        //    || Input.GetKey(KeyCode.S)
        //    || Input.GetKey(KeyCode.D))
        //{
        //    anim.SetBool("isRunning", true); 
        //}
        //else
        //{
        //    anim.SetBool("isRunning", false); 
        //}
    }
}
