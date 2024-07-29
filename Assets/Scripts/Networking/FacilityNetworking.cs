using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FacilityNetworking : MonoBehaviour
{
    public static FacilityNetworking instance;

    [SerializeField]
    private Transform otherPlayer;

    private NetworkManager network;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        network = NetworkManager.Singleton;
        Debug.Log("I AM THE NETWORKER YES THAT IS ME");
#if UNITY_EDITOR
        if (ParrelSync.ClonesManager.IsClone())
        {
            network.StartClient();
            ClientStart();
        }
        else
        {
            network.StartHost();
            HostStart();
        }
#else
        network.StartClient();
        ClientStart();
#endif

        Debug.Log("Host Status: " + network.IsHost);
        Debug.Log("Client Status: " + network.IsClient);
        Debug.Log("Server Status: " + network.IsServer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HostStart()
    {
        ClientStart();
        ServerStart();
        ConnectPlayer(NetworkManager.Singleton.LocalClientId);
    }

    private void ClientStart()
    {

    }

    private void ServerStart()
    {
        network.OnClientConnectedCallback += ConnectPlayer;
        network.OnClientDisconnectCallback += (obj) =>
        {
            Debug.Log(obj + " has Disconnected");
        };
    }

    private void ConnectPlayer(ulong id)
    {
        network.ConnectedClients[id].PlayerObject.GetComponent<PlayerNetworker>().playerState.Value = new PlayerNetworker.PlayerState
        {
            name = "potatoes"
        };
        Debug.Log(id + " has Connected");
    }
}
