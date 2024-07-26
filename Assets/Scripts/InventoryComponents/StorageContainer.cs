using System.Collections;
using System.Collections.Generic;
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
    public int slotCount;
    private GameObject slotHolder;
    private List<SlotManager> managers = new List<SlotManager>();
    private List<GameObject> slots = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
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
    }
    protected virtual void SlotInitalize(int id, ItemSlot slot)
    {

    }
    public ItemSlot GetItem(int slot)
    {
        GameObject obj = slots[slot];

        return obj.GetComponent<ItemSlot>();
    }
    public ItemSlot[] GetItems()
    {
        return GetComponentsInChildren<ItemSlot>();
    }
    public void SetItem(int slot,ItemDesc item)
    {
        item.gameObject.transform.parent = slots[slot].transform;
    }
}
