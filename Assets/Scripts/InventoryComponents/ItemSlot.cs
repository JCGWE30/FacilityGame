using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public ItemDesc item { get; private set; }
    public SlotType slotType;
    public int id { get; private set; }

    private bool clearNext = false;

    public void SetId(int id)
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
        Debug.Log("ok");
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
