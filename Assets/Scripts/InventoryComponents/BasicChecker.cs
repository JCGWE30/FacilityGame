using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicChecker : MovementChecker
{
    protected override bool GetCanMove(ItemSlot from, ItemSlot to, bool shifting)
    {
        if (to.item == null && InventorySlot.CanItemFit(to.slotType, from.item.itemType))
            return true;
        return false;
    }

    protected override bool GetTryMove(ItemSlot from, ItemSlot to, bool shifting)
    {
        if (!CanMove(from, to, shifting))
            return false;
        return base.GetTryMove(from, to, shifting);
    }

    protected override bool GetTryInsert(ItemSlot to)
    {
        if (to.item != null)
            return false;
        return base.GetTryInsert(to);
    }
}
