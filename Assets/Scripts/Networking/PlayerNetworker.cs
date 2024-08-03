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


    //Should be removed very soon

    //[Rpc(SendTo.Server)]
    //public void TryItemPickupRpc(long id,ulong playerId)
    //{
    //    NetworkObject player = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject;
    //    EquipmentContainer equipment = player.GetComponent<EquipmentContainer>();
    //    DroppedItem item = EntityIdentifier.fetchObject<DroppedItem>(id);
    //    if (item == null)
    //    {
    //        CancelItemPickupRpc(RpcTarget.Single(playerId, RpcTargetUse.Temp));
    //        return;
    //    }
    //    bool success = item.droppedItem.GetComponent<MovementChecker>().TryInsert(equipment.GetEquipmentItem(SlotType.Hand));

    //    if (!success)
    //    {
    //        CancelItemPickupRpc(RpcTarget.Single(playerId, RpcTargetUse.Temp));
    //    }
    //}

    //[Rpc(SendTo.SpecifiedInParams)]
    //public void CancelItemPickupRpc(RpcParams rpcParams)
    //{
    //    EquipmentContainer contaner = EquipmentContainer.instance;
    //    ItemSlot slot = contaner.GetEquipmentItem(SlotType.Hand);
    //    slot.ClearNextItem();
    //}

    

    //[Rpc(SendTo.Server)]
    //public void TryItemDropRpc(long id, ulong playerId, bool shifting)
    //{
    //    NetworkObject player = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject;
    //    //EquipmentContainer equipment = player.GetComponent<EquipmentContainer>();
    //    //StorageContainer containerToSearch = containerId switch
    //    //{
    //    //    0 => equipment, //Equipment
    //    //    1 => equipment.GetEquipmentItem(SlotType.Belt).item?.GetComponent<StorageContainer>(), //Belt
    //    //    2 => equipment.GetEquipmentItem(SlotType.Back).item?.GetComponent<StorageContainer>(), //Backpack
    //    //    3 => equipment.GetEquipmentItem(SlotType.Vest).item?.GetComponent<StorageContainer>(), //Vest
    //    //};

    //    //if (containerToSearch == null)
    //    //    return;

    //    //ItemSlot slot = containerToSearch.GetItem(slotId);
    //    ItemDesc item = EntityIdentifier.fetchInstancedObject<ItemDesc>(id);
    //    if (item == null)
    //        return;
    //    ItemDesc itemToDrop = item.getChecker().TryDrop(item.GetSlot(), shifting);
    //    if (itemToDrop != null)
    //    {
    //        GameObject droppedItem = Instantiate(droppedItemPrefab);
    //        droppedItem.transform.position = player.transform.position;
    //        itemToDrop.transform.parent = droppedItem.transform;    
    //        droppedItem.GetComponent<NetworkObject>().Spawn(true);
    //        CompleteDropItemRpc(id, shifting,RpcTarget.Single(playerId, RpcTargetUse.Temp));
    //    }
    //}

    //[Rpc(SendTo.SpecifiedInParams)]
    //public void CompleteDropItemRpc(long id,bool shifting,RpcParams rpcParams)
    //{
    //    ItemDesc item = EntityIdentifier.fetchObject<ItemDesc>(id);
    //    if (item == null)
    //        return;
    //    Destroy(item.getChecker().TryDrop(item.GetSlot(), shifting));
    //}
}