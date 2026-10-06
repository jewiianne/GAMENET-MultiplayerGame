using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP; //required for the unity transport
using Unity.Networking.Transport.Relay; //required for the allocation utils
using Unity.Services.Relay; //required for relay operations
using Unity.Services.Relay.Models; //required for relay allocations
using Unity.Services.Core; //required for initialize unity services
using Unity.Services.Authentication; // required for players to sigh in anonymously
using System.Threading.Tasks; // required for async tasks (await)

public class MultiplayerMenu : NetworkBehaviour
{
    //public static MultiplayerMenu Instance;

    //hide ui
    public GameObject menuUI;
    //store clients input field
    [SerializeField] private TMP_InputField joinCodeInput; // Where the Client types the code
    [SerializeField] private TMP_Text joinCodeText; // Where the Host sees their generated code
    [SerializeField] private TMP_Text statusText; // Shows connection messages

    //webgl
    private const string WebGLConnectionType = "wss";

    //void Awake()
    //{
    //    Instance = this;
    //}

    private async void Start()
    {
        // 1. Initialize Unity Gaming Services
        await UnityServices.InitializeAsync();

        // 2. Sign the player in anonymously (Relay requires authentication)
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            statusText.text = "Signed in locally. Ready to connect.";
        }
    }

    // Must be 'async' because reaching out to Unity's internet servers takes time
    public async void StartHost()
    {
        try
        {
            statusText.text = "Creating Relay Session...";

            // 1. Ask Unity Relay for a server allocation (Max 4 players)
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

            // 2. Ask Relay for the specific Join Code attached to our allocation
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // 3. Display the Join Code on the Host's screen so they can share it
            joinCodeText.text = "Join Code: " + joinCode;

            // 4. Grab the network transport component
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            // 5. Force WebSockets (vital for WebGL)
            transport.UseWebSockets = true;

            // 6. Pass the Relay allocation data into the transport
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, WebGLConnectionType));

            // 7. Finally, start the Host
            NetworkManager.Singleton.StartHost();

            statusText.text = "Host started successfully!";
            // Note: We do not hide the menuUI entirely for the Host yet, so they can still see the Join Code text on screen.
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            statusText.text = "Failed to start Host.";
        }
    }

    // Must be 'async' because joining takes time
    public async void StartClient()
    {
        try
        {
            statusText.text = "Joining Session...";

            // 1. Get the text the Client typed and ask Relay to join that allocation
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCodeInput.text);

            // 2. Grab the network transport component
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            // 3. Force WebSockets for the Client
            transport.UseWebSockets = true;

            // 4. Pass the specific join allocation data into the transport
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, WebGLConnectionType));

            // 5. Start the Client
            NetworkManager.Singleton.StartClient();

            // 6. Hide the main menu since the Client successfully joined
            if (menuUI != null) menuUI.SetActive(false);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            statusText.text = "Invalid Join Code.";
        }
    }

    //public void StartHost()
    //{
    //    //grabs the unity transport component attached to the network manager
    //    UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    //    //forces websockets in the transport. if the transport uses websockets but relay data uses udp, connection fails
    //    transport.UseWebSockets = true;
    //    NetworkManager.Singleton.StartHost();
    //}

    //public void StartClient()
    //{
    //    //grabs the unity transport component attached to the network manager
    //    UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    //    //forces websockets in the transport. if the transport uses websockets but relay data uses udp, connection fails
    //    transport.UseWebSockets = true;
    //    NetworkManager.Singleton.StartClient();
    //}

    //public void StartServer()
    //{
    //    //grabs the unity transport component attached to the network manager
    //    UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    //    //forces websockets in the transport. if the transport uses websockets but relay data uses udp, connection fails
    //    transport.UseWebSockets = true;
    //    NetworkManager.Singleton.StartServer();
    //}
}
