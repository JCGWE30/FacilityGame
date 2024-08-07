using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateContainer : StorageContainer
{
    protected override void SlotInitalize(int id, ItemSlot slot)
    {
        slot.slotType = SlotType.Hand;
    }
}
