using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] float movementSpeed;

    private CharacterController characterController;
    

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return; 

        Move();
    }

    private void Move()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        characterController.SimpleMove(moveDirection * movementSpeed);
    }


    public void SetMovementSpeed(float speed)
    {
        movementSpeed = speed; 
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("KillBox"))
        {
            other.GetComponent<KillBox>().RespawnObject(gameObject); 
        }
    }
}
