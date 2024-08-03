using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BasicContainer : StorageContainer
{
    protected override void SlotInitalize(int id, ItemSlot slot)
    {
        slot.AddComponent<GlobalIdentifier>();
    }
}
