using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatheContainer : StorageContainer
{
    protected override void SlotInitalize(int id, ItemSlot slot)
    {
        slot.slotType = SlotType.None;
    }
}
