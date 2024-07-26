using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public ItemDesc item { get; private set; }
    public SlotType slotType;
    public int id { get; private set; }

    public void SetId(int id)
    {
        this.id = id;
    }

    private void Update()
    {
        item = transform.GetComponentInChildren<ItemDesc>();
    }
}
