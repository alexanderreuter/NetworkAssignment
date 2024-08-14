using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float timeToSpawn = 3f;
    [SerializeField] private GameObject playerPrefab;
    
    public void OnPlayerDeath(ulong clientId)
    {
        StartCoroutine(RespawnPlayer(clientId));
    }

    private IEnumerator RespawnPlayer(ulong clientId)
    {
        yield return new WaitForSeconds(timeToSpawn);
        
        GameObject newPlayer = Instantiate(playerPrefab, GetRandomSpawnPoint().position, Quaternion.identity);
        newPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }

    private Transform GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
