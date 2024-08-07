using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworker : NetworkBehaviour
{
    public static PlayerNetworker localInstance;

    [SerializeField]
    private Transform playerMarker;

    [SerializeField]
    private GameObject droppedItemPrefab;

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
        if (IsServer)
        {
            EquipmentContainer newContainer = gameObject.AddComponent<EquipmentContainer>();
            newContainer.OnSlotsInitalized += (slots) =>
            {
                List<long> longs = new List<long>();

                foreach(var slot in slots)
                {
                    longs.Add(GlobalIdentifier.InitalizeID(slot));
                }

                SetEquipmentIdsRpc(longs.ToArray(), RpcTarget.Single(OwnerClientId, RpcTargetUse.Temp));
            };
        }
        if (IsOwner)
        {
            localInstance = this;
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
    }

    // Update is called once per frame
    void Update()
    {
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

    [Rpc(SendTo.SpecifiedInParams)]
    public void SetEquipmentIdsRpc(long[] ids, RpcParams rpcParams)
    {
        int runner = 0;
        foreach(long id in ids)
        {
            EquipmentContainer.instance.GetItem(runner).SetId(id);
            runner++;
        }
    }
}