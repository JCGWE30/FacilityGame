using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct ItemSlotData : INetworkSerializable
{
    public int id;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
    }
}
public class ItemSlot : MonoBehaviour,ISerializableComponent
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

    public INetworkSerializable SerializeComponent()
    {
        return new ItemSlotData
        {
            id = id
        };
    }

    public void DeserializeComponent(INetworkSerializable serializedComponent)
    {
        ItemSlotData? data = serializedComponent as ItemSlotData?;
        if (data.HasValue)
        {
            id = data.Value.id;
        }
    }
}
