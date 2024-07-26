using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class EquipmentContainer : StorageContainer
{
    public static EquipmentContainer instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    protected override void SlotInitalize(int id, ItemSlot slot)
    {
        switch (id)
        {
            case 0:
                slot.slotType = SlotType.Hand;
                break;
            case 1:
                slot.slotType = SlotType.Belt;
                break;
            case 2:
                slot.slotType = SlotType.Back;
                break;
            case 3:
                slot.slotType = SlotType.Uniform;
                break;
            case 4:
                slot.slotType = SlotType.Vest;
                break;
            case 5:
                slot.slotType = SlotType.Helmet;
                break;
        }
    }

    public bool HasItem(SlotType type)
    {
        return GetEquipmentItem(type).item != null;
    }
    public ItemSlot GetEquipmentItem(SlotType type)
    {
        switch (type)
        {
            case SlotType.Hand:
                return GetItem(0);
            case SlotType.Belt:
                return GetItem(1);
            case SlotType.Back:
                return GetItem(2);
            case SlotType.Uniform:
                return GetItem(3);
            case SlotType.Vest:
                return GetItem(4);
            case SlotType.Helmet:
                return GetItem(5);
        }
        return null;
    }
}
