using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FacilityNetworking : MonoBehaviour
{
    [SerializeField]
    private Transform otherPlayer;

    private NetworkManager network;
    // Start is called before the first frame update
    void Start()
    {
        network = NetworkManager.Singleton;
        if (!Application.isEditor)
        {
            network.StartHost();
            HostStart();
        }
        else
        {
            network.StartClient();
            ClientStart();
        }

        return;
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
