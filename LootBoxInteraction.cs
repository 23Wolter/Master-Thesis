using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBoxInteraction : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject interactionButton;

    [Header("Prefabs")]
    [SerializeField] GameObject protonPrefab;
    [SerializeField] GameObject neutronPrefab;

    [Header("Level")]
    [SerializeField] Transform[] pickupSpawnPoints;



    public bool openBox = false;
    private GameObject playerEntering = null;
    private Vector3[] spawnPoints;

    private void Start()
    {
        spawnPoints = new Vector3[pickupSpawnPoints.Length]; 

        for(int i=0; i<pickupSpawnPoints.Length; i++)
        {
            spawnPoints[i] = pickupSpawnPoints[i].position; 
        }
    }


    private void Update()
    {
        OpenLootBox(); 
    }

    private void OpenLootBox()
    {
        if(openBox)
        {
            // CODE TO DO...
            // Input could also be from a controller
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!playerEntering) return; 
                playerEntering.GetComponent<PlayerAction>().OpenLootBox(spawnPoints, this.GetComponent<NetworkIdentity>().netId);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (other.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                interactionButton.SetActive(true);
                playerEntering = other.gameObject; 
                openBox = true; 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (other.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                interactionButton.SetActive(false);
                playerEntering = null; 
                openBox = false; 
            }
        }
    }
}
