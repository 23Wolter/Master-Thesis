using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionCamera : NetworkBehaviour
{
    [SerializeField] Transform cameraPos; 


    void Start()
    {
        if (isLocalPlayer)
        {
            Transform mainCamera = Camera.main.transform;
            mainCamera.parent = cameraPos;
            mainCamera.position = cameraPos.position;
            mainCamera.rotation = cameraPos.rotation;
        }
    }

}
