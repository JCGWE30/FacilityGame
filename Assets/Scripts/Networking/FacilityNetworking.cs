using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FacilityNetworking : MonoBehaviour
{
    public static FacilityNetworking instance;

    public DroppedItem droppedItem;

    private NetworkManager network;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        network = NetworkManager.Singleton;
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
            Debug.Log(obj + " has Disconnected"); //ND
        };
    }

    private void ConnectPlayer(ulong id)
    {
        network.ConnectedClients[id].PlayerObject.GetComponent<PlayerNetworker>().playerState.Value = new PlayerNetworker.PlayerState
        {
            name = "potatoes"
        };
        Debug.Log(id + " has Connected"); //ND
    }
}
