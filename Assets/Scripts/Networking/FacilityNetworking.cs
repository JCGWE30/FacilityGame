using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FacilityNetworking : MonoBehaviour
{
    [SerializeField]
    private Transform otherPlayer;
    // Start is called before the first frame update
    void Start()
    {
        if (Application.isEditor)
        {
            NetworkManager.Singleton.StartHost();
            ServerStart();
            ClientStart();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
            ClientStart();
        }

        return;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ClientStart()
    {

    }

    private void ServerStart()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (obj) =>
        {
            Debug.Log(obj + " has Connected");
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (obj) =>
        {
            Debug.Log(obj + " has Disconnected");
        };
    }
}
