using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : NetworkBehaviour
{
    private Camera playerCam;
    private Transform playerBody; 
    private RaycastHit hit;
    private bool canRotate = true; 


    private void Start()
    {
        playerCam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if(canRotate) Rotate(); 
    }


    private void Rotate()
    {
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 target = new Vector3(hit.point.x, playerBody.position.y, hit.point.z);
            playerBody.LookAt(target);
        }
    }


    public void SetPlayerBody(Transform body)
    {
        playerBody = body; 
    }

    public void SetCanRotate(bool rotate)
    {
        canRotate = rotate; 
    }
}
