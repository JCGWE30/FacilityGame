using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class SlotInfo
{
    public ItemDesc item { get; private set; }
    public SlotType slot { get; private set; }
    public SlotInfo(ItemDesc item,SlotType slot)
    {
        this.item = item;
        this.slot = slot;
    }
}

public abstract class StorageContainer : MonoBehaviour
{
    public delegate void FinishHandler(List<GameObject> slots);

    public event FinishHandler OnSlotsInitalized;
    public int slotCount;
    private GameObject slotHolder;
    private List<GameObject> slots = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        if (transform.Find("Inventory"))
        {
            foreach(Transform child in transform.Find("Inventory"))
            {
                slots.Add(child.gameObject);
            }
            slotCount = slots.Count;
            return;
        }
        slotHolder = new GameObject("Inventory");
        slotHolder.transform.parent = transform;
        for (int i = 0; i < slotCount; i++)
        {
            var slot = new GameObject("Slot" + i);
            slot.AddComponent<ItemSlot>().SetId(i);
            SlotInitalize(i, slot.GetComponent<ItemSlot>());
            slots.Add(slot);
            slot.transform.parent = slotHolder.transform;
        }
        OnSlotsInitalized?.Invoke(slots);
    }
    protected virtual void SlotInitalize(int id, ItemSlot slot)
    {

    }
    public ItemSlot GetItemByID(int slot)
    {
        return idToSlot(slot);
    }
    public ItemSlot GetItem(int slot)
    {
        return slots[slot].GetComponent<ItemSlot>();
    }
    public ItemSlot[] GetItems()
    {
        return GetComponentsInChildren<ItemSlot>();
    }
    public void SetItem(long slot,ItemDesc item)
    {
        item.gameObject.transform.parent = idToSlot(slot).transform;
    }
    private ItemSlot idToSlot(long id) 
    {
        foreach(var slot in slots)
        {
            if (slot.GetComponent<ItemSlot>().id == id)
                return slot.GetComponent<ItemSlot>();
        }
        return null;
    }
}
