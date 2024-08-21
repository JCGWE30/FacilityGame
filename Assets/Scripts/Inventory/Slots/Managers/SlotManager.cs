using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class SlotManager : MonoBehaviour, IInatalizer
{
    private List<InventorySlot> slots = new List<InventorySlot>();
    public StorageContainer container;
    // Start is called before the first frame update
    public void Initalize()
    {
        slots = transform.GetComponentsInChildren<InventorySlot>().ToList();
        int count = 0;
        foreach (var item in slots)
        {
            item.id = count;
            count++;
        }
        SubInitalize();
    }

    public InventorySlot GetSlot(int slot)
    {
        return slots[slot];
    }

    // Update is called once per frame
    void Update()
    {
        ItemUpdate(container);
        if (container == null)
        {
            foreach (var item in slots)
            {
                item.gameObject.SetActive(false);
                item.SetItem(null);
            }
            return;
        }
        int count = 0;
        foreach (var item in slots)
        {
            if (container.slotCount > count)
            {
                item.gameObject.SetActive(true);
                item.SetItem(container.GetItems()[count].item);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
            count++;
        }
    }
    protected virtual void ItemUpdate(StorageContainer container)
    {

    }
    //Inheriting classes CANNOT override awake. Thats why this is here
    protected virtual void SubInitalize()
    {

    }
}
