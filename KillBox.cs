using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    [SerializeField] Transform respawn; 

    public void RespawnObject(GameObject obj)
    {
        obj.transform.position = respawn.position; 
    }
}
