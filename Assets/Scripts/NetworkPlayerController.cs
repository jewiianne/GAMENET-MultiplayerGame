using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundGravity = -2;

    private CharacterController characterController;

    public float verticalVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        //checks if the object is not owned by the local player
        if (!IsOwner)
        {
            //stops non owned players from reading local player
            return;
        }

        //reads left and right input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 movementInput = new Vector2(horizontalInput, verticalInput);

        //checks if this instance is the server. the host is also a server
        if(IsServer)
        {
            MovePlayer(movementInput);
        }
        //runs when the owner is a normal client, not the server
        else
        {
            MovePlayerRPC(movementInput);
        }
    }
    //marks the next method as an RPC that runs on the server
    [Rpc(SendTo.Server)]
    //declare the server RPC that receives client movement input
    private void MovePlayerRPC(Vector2 movementInput)
    {
        MovePlayer(movementInput);
    }

    private void MovePlayer(Vector2 movementInput)
    {
        if(characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundGravity;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;

        Vector3 horizontalMovement = moveDirection * moveSpeed;
        Vector3 verticalMovement = Vector3.up * verticalVelocity;

        //combines horizontal and vertical
        Vector3 finalMovement = horizontalMovement + verticalMovement;
        characterController.Move(finalMovement * Time.deltaTime);
    }

}
