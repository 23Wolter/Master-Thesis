using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject lootBoxPrefab;
    [SerializeField] GameObject protonPrefab;
    [SerializeField] GameObject neutronPrefab;

    [Header("Level")]
    [SerializeField] Transform[] lootBoxSpawnPoints;

    [Header("SFX")]
    [SerializeField] AudioSource lootboxOpeningSound; 


    [Server]
    public void SpawnLootBoxes()
    {
        foreach(Transform spawnPoint in lootBoxSpawnPoints)
        {
            GameObject lootBox = Instantiate(lootBoxPrefab);
            NetworkServer.Spawn(lootBox);
            lootBox.transform.position = spawnPoint.position;
            lootBox.transform.rotation = spawnPoint.rotation;
        }
    }

    [Server]
    public void SpawnPickups(Vector3[] pickupSpawnPoints, uint netId)
    {
        RpcPlaySFX(); 

        foreach (Vector3 spawnPoint in pickupSpawnPoints)
        {
            GameObject pickup;
            int randNum = Random.Range(0, 2);
            if (randNum == 0)
            {
                pickup = Instantiate(protonPrefab);
            }
            else
            {
                pickup = Instantiate(neutronPrefab);
            }

            NetworkServer.Spawn(pickup);
            pickup.transform.position = spawnPoint;
        }

        GameObject lootBox = NetworkIdentity.spawned[netId].gameObject;
        NetworkServer.Destroy(lootBox); 
    }

    [ClientRpc]
    private void RpcPlaySFX()
    {
        lootboxOpeningSound.Play(); 
    }

    [Server]
    public void DropPickup(bool proton, Transform playerPosition, Transform playerBody)
    {
        GameObject pickup;
        if (proton) pickup = Instantiate(protonPrefab);
        else pickup = Instantiate(neutronPrefab);

        NetworkServer.Spawn(pickup);
        pickup.transform.position = playerPosition.position + playerBody.forward * 4;
    }
}
