using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementChecker : MonoBehaviour
{
    public bool CanMove(ItemSlot from, ItemSlot to, bool shifting)
    {
        return GetCanMove(from, to, shifting);
    }
    protected virtual bool GetCanMove(ItemSlot from, ItemSlot to, bool shifting)
    {
        return false;
    }

    public bool TryMove(ItemSlot from, ItemSlot to, bool shifting)
    {
        return GetTryMove(from, to, shifting);
    }
    protected virtual bool GetTryMove(ItemSlot from, ItemSlot to, bool shifting)
    {
        to.GetComponentInParent<StorageContainer>().SetItem(to.id, GetComponent<ItemDesc>());
        return true;
    }

    public bool TryInsert(ItemSlot to)
    {
        return GetTryInsert(to);
    }
    protected virtual bool GetTryInsert(ItemSlot to)
    {
        to.GetComponentInParent<StorageContainer>().SetItem(to.id, GetComponent<ItemDesc>());
        return true;
    }
    public ItemDesc TryDrop(ItemSlot from, bool shifting)
    {
        return GetTryDrop(from, shifting);
    }
    protected virtual ItemDesc GetTryDrop(ItemSlot from,bool shifting)
    {
        return GetComponent<ItemDesc>();
    }
}
