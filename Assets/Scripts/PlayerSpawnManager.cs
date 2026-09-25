using UnityEngine;
using Unity.Netcode;

public class PlayerSpawnManager : NetworkBehaviour
{
    //stores spawn points
    //static = means all player objects shares this value
    private static int nextSpawnIndex;
    
    //runs when the player object spawns by netcode
    public override void OnNetworkSpawn()
    {
        if (IsOwner && MultiplayerMenu.Instance != null)
        {
            MultiplayerMenu.Instance.menuUI.SetActive(false);
        }
        
        //check if this instance is not the server
        if(!IsServer)
        {
            return;
        }
        //find all spawnpoint
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");

        //selects next spawnpoint
        Transform selectedSpawnPoint = spawnPointObjects[nextSpawnIndex].transform;

        //gets character controller
        CharacterController characterController = GetComponent<CharacterController>();

        //temporary disables the character controller
        characterController.enabled = false;

        //moves the player to the spawnpoint
        transform.position = selectedSpawnPoint.position;
        characterController.enabled = true;
        nextSpawnIndex++;

        if(nextSpawnIndex >= spawnPointObjects.Length)
        {
            nextSpawnIndex = 0;
        }
    }
}
