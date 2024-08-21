using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrateContainer : StorageContainer
{
    protected override void SlotInitalize(int id, ItemSlot slot)
    {
        slot.AddComponent<GlobalIdentifier>();
        slot.slotType = SlotType.Hand;
    }
}
