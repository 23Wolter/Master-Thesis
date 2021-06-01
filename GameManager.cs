using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour 
{
    [Header("Script References")]
    [SerializeField] SpawnManager spawnManager;
    [SerializeField] PortalManager[] portals; 

    [Header("Main Map Attributes")]
    [SerializeField] GameObject countDownCanvas; 
    [SerializeField] Text countDownTimer;
    [SerializeField] Transform mainSpawnPoint;
    
    [Header("Player Action Attributes")]
    [SerializeField] GameObject bulletPrefab;
    private Transform weaponMuzzle;
    [SerializeField] float shootingSpeed;
    [SerializeField] float bulletLifetime;

    [Header("UI")]
    [SerializeField] Text gameOverText;

    [Header("SFX")]
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] AudioSource gameoverMusic; 

    [Header("Player Count")]
    [SyncVar]
    public int playersOnline = 0;

    [SyncVar(hook = nameof(UpdatePlayersReady))]
    public int playersReady = 0;

    private GameObject[] activePlayers;



    private void Start()
    {
        spawnManager.SpawnLootBoxes(); 
    }


    private void Update()
    {
        if(isServer) UpdatePlayersOnline();
    }


    private void UpdatePlayersOnline()
    {
        playersOnline = NetworkServer.connections.Count; 
    }


    public void GetPlayersReady()
    {
        playersReady = 0; 
        activePlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in activePlayers)
        {
            if (player.GetComponent<GameStarter>().ready) playersReady++;

            if (playersReady == playersOnline) ShowCountDown();
        }
    }


    private void UpdatePlayersReady(int oldPlayersReady, int newPlayersReady)
    {
        ShowPlayersLeft(newPlayersReady);
    }


    [Server]
    private void ShowPlayersLeft(int playersReady) 
    {
        RpcShowPlayersLeft(playersReady); 
    }


    [ClientRpc]
    private void RpcShowPlayersLeft(int playersReady)
    {
        activePlayers = GameObject.FindGameObjectsWithTag("Player");
        
        foreach(GameObject player in activePlayers)
        {
            player.GetComponent<PlayerAction>().UpdatePlayersLeft(playersReady, playersOnline); 
        }
    }



    [Server]
    private void ShowCountDown()
    {
        RpcShowCountDown();
    }


    [ClientRpc]
    private void RpcShowCountDown()
    {
        countDownCanvas.SetActive(true);

        StartCoroutine(BeginCountDown()); 
    }


    private IEnumerator BeginCountDown()
    {
        for(int i=10; i>=0; i--)
        {
            countDownTimer.text = "Portaler aktiverer om " + i.ToString();
            yield return new WaitForSeconds(1); 
        }

        if(isServer) RpcMovePlayersToSpawn(); 
    }

    
    [ClientRpc]
    private void RpcMovePlayersToSpawn()
    {
        countDownCanvas.SetActive(false);
        this.GetComponent<SetMouseCursor>().setCursor = true;

        foreach(PortalManager portal in portals)
        {
            portal.Activate(); 
        }

        foreach (GameObject player in activePlayers)
        {
            //player.transform.position = mainSpawnPoint.position;
            //PlayerAction pAction = player.GetComponent<PlayerAction>(); 
            //pAction.enabled = true;

            Destroy(player.GetComponent<CharacterSelectionManager>());
            Destroy(player.GetComponent<CharacterSelectionCamera>());
            Destroy(player.GetComponent<GameStarter>()); 
        }
    } 



    public void RemovePlayer()
    {
        playersReady--;
        ShowPlayersLeft(playersReady);

        if(playersReady == 1)
        {
            ShowWinner();
        }
    }

    
    [Server]
    private void ShowWinner()
    {
        RpcShowWinner(); 
    }

    [ClientRpc]
    private void RpcShowWinner()
    {
        backgroundMusic.Stop();
        gameoverMusic.Play();  

        foreach (GameObject player in activePlayers)
        {
            if (player.CompareTag("Player"))
            {
                string name = player.GetComponent<DisplayName>().GetDisplayName();
                gameOverText.text = name + " VANDT KAMPEN!";
            }
        }
    }
}
