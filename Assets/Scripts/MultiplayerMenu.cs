using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class MultiplayerMenu : NetworkBehaviour
{
    //hide ui
    public GameObject menuUI;
    //store clients input field
    [SerializeField] TMP_InputField joinCodeText;
    [SerializeField] TMP_Text statusText;
    private const string WebGLConnectionType = "wss";

    public static MultiplayerMenu Instance;

    void Awake()
    {
        Instance = this;
    }
    public void StartHost()
    {

        //grabs the unity transport component attached to the network manager
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        //forces websockets in the transport. if the transport uses websockets but relay data uses udp, connection fails
        transport.UseWebSockets = true;
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        //grabs the unity transport component attached to the network manager
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        //forces websockets in the transport. if the transport uses websockets but relay data uses udp, connection fails
        transport.UseWebSockets = true;
        NetworkManager.Singleton.StartClient();
    }

    public void StartServer()
    {
        //grabs the unity transport component attached to the network manager
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        //forces websockets in the transport. if the transport uses websockets but relay data uses udp, connection fails
        transport.UseWebSockets = true;
        NetworkManager.Singleton.StartServer();
    }
}
