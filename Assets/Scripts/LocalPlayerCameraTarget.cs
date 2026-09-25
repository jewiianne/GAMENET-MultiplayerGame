using UnityEngine;
using Unity.Netcode; // Required for network variables
public class LocalPlayerCameraTarget : NetworkBehaviour
{
    private TopDownCameraFollow cameraFollow;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return; // Only the local player should set the camera target
        cameraFollow = Camera.main.GetComponent<TopDownCameraFollow>();
        cameraFollow.SetTarget(transform); // Set the camera to follow this player
    }
}
