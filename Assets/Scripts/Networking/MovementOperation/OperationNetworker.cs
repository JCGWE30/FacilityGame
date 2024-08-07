using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public enum OperationResult
{
    Success, // Operation has succeded
    Cancelled, // Operation could not be completed
    Failure // Operation is invalid (ie source item does not exist)
}

public struct QueuedItem
{
    public GameObject item;
    public long id;
}

public class OperationNetworker : NetworkBehaviour
{
    public List<QueuedItem> queuedItems = new List<QueuedItem>();
    public static OperationNetworker instance;

    void Start()
    {
        if (instance == null && IsOwner)
            instance = this;
    }

    [Rpc(SendTo.Server)]
    public void SendMovementOperationRpc(MovementOperation.MovementOperationData data)
    {
        if (GlobalIdentifier.FetchObject(data.sourceId)?.transform.childCount == 0)
        {
            SendResults(OperationResult.Failure, data.operationId);
        }

        DroppedItem sourceItem;
        ItemSlot source;
        ItemSlot destination;
        bool success;

        switch (data.type)
        {
            case OperationType.Pickup:
                sourceItem = GlobalIdentifier.FetchObject<DroppedItem>(data.sourceId);
                destination = GlobalIdentifier.FetchObject<ItemSlot>(data.destinationId);
                success = sourceItem.droppedItem.checker.TryInsert(destination);
                if (success)
                {
                    Destroy(sourceItem.gameObject);
                }
                SendResults(success ? OperationResult.Success : OperationResult.Cancelled, data.operationId);
                break;
            case OperationType.Move:
                source = GlobalIdentifier.FetchObject<ItemSlot>(data.sourceId);
                destination = GlobalIdentifier.FetchObject<ItemSlot>(data.destinationId);
                success = source.item.checker.TryMove(source, destination, data.shifting);
                SendResults(success ? OperationResult.Success : OperationResult.Cancelled, data.operationId);
                break;
            case OperationType.Drop:
                source = GlobalIdentifier.FetchObject<ItemSlot>(data.sourceId);
                ItemDesc dropItem = source.item.checker.TryDrop(source, data.shifting);
                success = dropItem != null;
                if (success)
                {
                    DroppedItem itemToDrop = Instantiate(FacilityNetworking.instance.droppedItem);
                    itemToDrop.GetComponent<NetworkObject>().Spawn(true);
                    dropItem.transform.parent = itemToDrop.transform;
                    itemToDrop.transform.position = NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.transform.position;
                    SpawnDroppedItem(dropItem, GlobalIdentifier.InitalizeID(itemToDrop));
                }
                SendResults(success ? OperationResult.Success : OperationResult.Cancelled, data.operationId);
                break;
        }
    }
    private void SendResults(OperationResult result,long id)
    {
        RpcParams rpcParams = RpcTarget.Single(OwnerClientId, RpcTargetUse.Temp);
        SendOperationResultsRpc(id, result, rpcParams);
    }

    private void SpawnDroppedItem(ItemDesc item, long id)
    {
        SerializedGameObject sgo = ItemSerializer.serializeGameObject(item.gameObject);
        SpawnDroppedItemRpc(id, sgo);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void SendOperationResultsRpc(long id, OperationResult result,RpcParams rpcParams)
    {
        MovementOperation.ProcessResult(id, result);
    }

    [Rpc(SendTo.NotServer)]
    public void SpawnDroppedItemRpc(long id,SerializedGameObject serializedObject)
    {
        DroppedItem droppedItem = GlobalIdentifier.FetchObject<DroppedItem>(id);
        GameObject item = ItemSerializer.deserializeGameObject(serializedObject);
        if (droppedItem == null)
        {
            instance.queuedItems.Add(new QueuedItem { item = item, id=id});
            return;
        }
        item.transform.parent = droppedItem.transform;
    }
}
