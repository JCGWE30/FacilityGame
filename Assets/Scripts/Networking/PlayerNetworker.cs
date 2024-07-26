using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworker : NetworkBehaviour
{
    [SerializeField]
    private Transform playerMarker;

    private Transform player;

    public NetworkVariable<PlayerState> playerState = new NetworkVariable<PlayerState>(
        new PlayerState
        {
            name = "test"
        },NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

    public struct PlayerState : INetworkSerializable
    {
        public FixedString32Bytes name;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref name);
        }
    }

    private TMP_Text username;
    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            gameObject.name = "LocalClient";
        }
        else
        {
            Transform marker = Instantiate(playerMarker);
            username = marker.GetComponentInChildren<TMP_Text>();
            username.transform.localEulerAngles = new Vector3(0, 180, 0);
            marker.parent = gameObject.transform;
            marker.localPosition = Vector3.zero;
            gameObject.name = $"RemoteClient ({OwnerClientId})";
        }

        playerState.OnValueChanged += (PlayerState prev, PlayerState next) =>
        {
            Debug.Log("We got a value change " + next.name);
        };
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(playerState.Value.name+" "+OwnerClientId);
        if (player == null)
            player = InventoryManager.instance.transform;
        if (IsOwner)
        {
            transform.SetPositionAndRotation(player.position, player.rotation);
        }
        else
        {
            username.text = playerState.Value.name.ToString();
            username.transform.LookAt(Camera.main.transform);
        }
    }
}