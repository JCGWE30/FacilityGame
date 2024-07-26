using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworker : NetworkBehaviour
{
    [SerializeField]
    private Transform playerMarker;

    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            gameObject.name = "LocalClientS";
        }
        else
        {
            Transform marker = Instantiate(playerMarker);
            marker.parent = gameObject.transform;
            gameObject.name = $"RemoteClient ({OwnerClientId})";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
            return;
        if (player == null)
            player = InventoryManager.instance.transform;
        transform.SetPositionAndRotation(player.position, player.rotation);
    }
}
