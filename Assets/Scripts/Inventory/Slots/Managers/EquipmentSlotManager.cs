using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class EquipmentSlotManager : SlotManager
{
    private InventoryManager manager;
    private SlotManager backpackSlots;
    private SlotManager beltSlots;
    private HotbarWatcher hotbar;
    public EquipmentContainer equipmentContainer { get; private set; }

    protected override void Initalize()
    {
        equipmentContainer = container.GetComponent<EquipmentContainer>();
        manager = InventoryManager.instance;
        hotbar = manager.hotbar;
        backpackSlots = manager.backpackSlots;
        beltSlots = manager.beltSlots;
    }

    public InventorySlot GetEquipmentSlot(SlotType type)
    {
        foreach (Transform item in transform)
        {
            InventorySlot slot = item.GetComponent<InventorySlot>();
            if (slot.slotType == type)
                return slot;
        }
        //This should never happen
        return null;
    }

    private void LateUpdate()
    {
    }

    protected override void ItemUpdate(StorageContainer container)
    {
        foreach (var item in container.GetItems())
        {
            //If an item is put onto the back
            if (item.slotType == SlotType.Back && item.item != null)
            {
                StorageContainer itemContainer;
                if (item.item.TryGetComponent(out itemContainer))
                    backpackSlots.container = itemContainer;
                else
                    backpackSlots.container = null;
            }
            //If an item is taken off the back
            else if (item.slotType == SlotType.Back)
                backpackSlots.container = null;

            //Same but for the belt
            if (item.slotType == SlotType.Belt && item.item != null)
            {
                StorageContainer itemContainer;
                if (item.item.TryGetComponent(out itemContainer))
                {
                    hotbar.container = itemContainer;
                    beltSlots.container = itemContainer;
                }
                else
                {
                    hotbar.container = null;
                    beltSlots.container = null;
                }
            }
            else if (item.slotType == SlotType.Belt)
            {
                hotbar.container = null;
                beltSlots.container = null;
            }
        }
    }
}
