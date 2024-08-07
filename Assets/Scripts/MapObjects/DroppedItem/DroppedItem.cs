using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DroppedItem : Interactable
{
    public ItemDesc droppedItem { get; private set; }
    public LayerMask mask;
    private InventoryManager manager;
    void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkObject obj;
            if (TryGetComponent(out obj))
            {
                if (!obj.IsSpawned)
                    obj.Spawn(true);
            }
        }
        manager = InventoryManager.instance;
        mask = LayerMask.GetMask("Floor");

        droppedItem = gameObject.GetComponentInChildren<ItemDesc>();
        if (droppedItem == null)
        {
            GlobalIdentifier gid = GetComponent<GlobalIdentifier>();
            gid.id.OnValueChanged = (long pval, long nval) =>
            {
                var dd = OperationNetworker.instance.queuedItems;

                foreach (var item in OperationNetworker.instance.queuedItems)
                {
                    if (item.id == nval)
                    {
                        GameObject newItem = item.item;
                        newItem.transform.parent = transform;
                        droppedItem = newItem.GetComponent<ItemDesc>();
                        initDroppedItem();
                    }
                }
            };
            return;
        }
        initDroppedItem();
    }
    private void initDroppedItem()
    {
        gameObject.layer = 6;
        InfoImage = droppedItem.sprite;
        InfoText = "Pickup " + droppedItem.displayName;

        gameObject.transform.localScale = droppedItem.worldModel.transform.lossyScale;
        gameObject.transform.rotation = droppedItem.worldModel.transform.rotation;

        gameObject.AddComponent<MeshFilter>().sharedMesh = droppedItem.worldModel.GetComponent<MeshFilter>().sharedMesh;
        gameObject.AddComponent<MeshRenderer>().material = droppedItem.worldModel.GetComponent<MeshRenderer>().sharedMaterial;

        BoxCollider col = gameObject.AddComponent<BoxCollider>();
        col.size = droppedItem.worldModel.GetComponent<BoxCollider>().size;
        col.center = droppedItem.worldModel.GetComponent<BoxCollider>().center;

        Ray ray = new Ray(gameObject.transform.position, Vector2.down);
        RaycastHit hitinfo;

        if (Physics.Raycast(ray, out hitinfo, 30f, mask))
        {
            gameObject.transform.position = hitinfo.point;
        }
    }
    protected override void Interact(bool alt)
    {
        if (alt)
        {
            StorageContainer itemContainer;
            if (!droppedItem.TryGetComponent(out itemContainer))
                return;
            manager.storageSlots.Open(itemContainer);
        }
        else
        {
            if (manager.ArmItem(Instantiate(droppedItem)))
            {
                ItemSlot handSlot = EquipmentContainer.instance.GetEquipmentItem(SlotType.Hand);
                MovementOperation operation = new MovementOperation(GetComponent<GlobalIdentifier>().id.Value, handSlot.id, false);
                operation.OnOperationCanceled += () =>
                {
                    handSlot.Clear();
                };
                operation.ProcessMove();
            }
                //PlayerNetworker.localInstance.TryItemPickupRpc(GetComponent<EntityIdentifier>().id, NetworkManager.Singleton.LocalClientId);
        }
    }
}
