using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DroppedItem : MonoBehaviour
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
    public void RegisterNewItem()
    {
        droppedItem = gameObject.GetComponentInChildren<ItemDesc>();
        if (droppedItem != null)
            initDroppedItem();
    }
    private void initDroppedItem()
    {
        gameObject.layer = 6;
        gameObject.GetOrAddComponent<Interactable>().Setup(droppedItem.sprite, droppedItem.displayName)
            .OnInteract += Interact;

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
    private void Interact()
    {
        if (InputManager.instance.onFoot.AltHeld.IsPressed())
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
        }
    }
}
