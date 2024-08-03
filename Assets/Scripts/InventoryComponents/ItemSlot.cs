using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public ItemDesc item { get; private set; }
    public SlotType slotType;
    public long id { get; private set; }

    private bool clearNext = false;

    public void SetId(long id)
    {
        this.id = id;
    }

    private void Update()
    {
        item = transform.GetComponentInChildren<ItemDesc>();
        if (item != null && clearNext)
        {
            Destroy(item);
            clearNext = false;
        }
    }

    public void Clear()
    {
        Destroy(item);
    }

    public void ClearNextItem()
    {
        clearNext = true;
    }
}
