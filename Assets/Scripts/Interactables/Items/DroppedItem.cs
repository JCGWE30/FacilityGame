using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DroppedItem : Interactable
{
    private ItemDesc droppedItem;
    public LayerMask mask;
    private InventoryManager manager;
    void Start()
    {
        manager = InventoryManager.instance;
        mask = LayerMask.GetMask("Floor");

        droppedItem = gameObject.transform.GetChild(0).gameObject.GetComponent<ItemDesc>();

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
            if (manager.ArmItem(droppedItem))
                Destroy(gameObject);
        }
    }
}
