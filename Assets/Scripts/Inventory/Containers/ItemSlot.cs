using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemSlot : MonoBehaviour,ISerializableComponent
{
    public ItemDesc item { get; private set; }
    public SlotType slotType;
    public long id;

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

    public ComponentValues SerializeComponent()
    {
        return new ComponentValues()
            .AddValue("id", id);
    }

    public void DeserializeComponent(ComponentValues serializedComponent)
    {
        serializedComponent
            .GetValue("id", ref id);
    }
}
