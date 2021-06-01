using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStarter : NetworkBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerBody; 
    [SerializeField] Transform mainGameCameraPos;
    [SerializeField] GameObject characterSelection;
    [SerializeField] GameObject mainGameUI;
    [SerializeField] GameObject miniMapUI;
    [SerializeField] InputField displayNameInput;
    [SerializeField] DisplayName displayNameScript;
    [SerializeField] AudioSource clickSound; 

    [SyncVar(hook = nameof(SetPlayerReady))] 
    public bool ready = false;
    
    
    public void EnterWaitArea()
    {
        if (!isLocalPlayer) return;

        clickSound.Play(); 

        displayNameScript.SetDisplayName(displayNameInput.text);

        CmdPlayerReady();

        Destroy(characterSelection);
        mainGameUI.SetActive(true);
        miniMapUI.SetActive(true);

        PositionPlayerInWaitArea();
        PositionCamera();
        ActivatePlayerMovement(); 
    }

    private void SetPlayerReady(bool oldValue, bool newValue)
    {
        GameObject gameManager = GameObject.Find("GameManager");
        gameManager.GetComponent<GameManager>().GetPlayersReady();
    }

    [Command]
    private void CmdPlayerReady()
    {
        ready = true; 
    }

    private void PositionPlayerInWaitArea()
    {
        Transform waitAreaSpawnPoint = GameObject.FindGameObjectWithTag("WaitAreaSpawnPoint").transform; 
        player.transform.rotation = waitAreaSpawnPoint.rotation;
        player.transform.position = waitAreaSpawnPoint.position;
    }


    private void PositionCamera()
    {
        Transform mainCamera = Camera.main.transform;

        mainCamera.parent = mainGameCameraPos;
        mainCamera.position = mainGameCameraPos.position;
        mainCamera.rotation = mainGameCameraPos.rotation;
    }


    private void ActivatePlayerMovement()
    {
        PlayerMovement playerMovement = player.AddComponent<PlayerMovement>();
        playerMovement.SetMovementSpeed(8);

        PlayerRotation playerRotation = player.AddComponent<PlayerRotation>();
        playerRotation.SetPlayerBody(playerBody.transform); 
    }
}
