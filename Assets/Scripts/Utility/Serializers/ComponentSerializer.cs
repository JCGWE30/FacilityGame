using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public interface ISerializableComponent
{
    //Something should be done with these 2 methods to allow making new components easier
    public ComponentValues SerializeComponent();
    public void DeserializeComponent(ComponentValues serializedComponent);
}

public struct SerializedValue : INetworkSerializable
{
    public string name;

    public int intValue;
    public FixedString4096Bytes stringValue;
    public long longValue;
    public ulong ulongValue;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref intValue);
        serializer.SerializeValue(ref stringValue);
        serializer.SerializeValue(ref longValue);
        serializer.SerializeValue(ref ulongValue);
    }
}

public class ComponentValues
{
    public List<SerializedValue> values = new List<SerializedValue>();


    public ComponentValues() { }

    public ComponentValues(SerializedValue[] vals) { values = vals.ToList(); }

    public ComponentValues AddValue(string name, int value)
    {
        values.Add(new SerializedValue { name = name, intValue = value });
        return this;
    }
    public ComponentValues AddValue(string name, string value)
    {
        values.Add(new SerializedValue { name = name, stringValue = value });
        return this;
    }
    public ComponentValues AddValue(string name, long value)
    {
        values.Add(new SerializedValue { name = name, longValue = value });
        return this;
    }
    public ComponentValues AddValue(string name, ulong value)
    {
        values.Add(new SerializedValue { name = name, ulongValue = value });
        return this;
    }
    public ComponentValues GetValue(string name, ref int setValue)
    {
        foreach (var item in values)
        {
            if (item.name == name)
            {
                setValue = item.intValue;
                return this;
            }
        }
        return this;
    }
    public ComponentValues GetValue(string name, ref string setValue)
    {
        foreach (var item in values)
        {
            if (item.name == name)
            {
                setValue = item.stringValue.ToString();
                return this;
            }
        }
        return this;
    }
    public ComponentValues GetValue(string name, ref long setValue)
    {
        foreach (var item in values)
        {
            if (item.name == name)
            {
                setValue = item.longValue;
                return this;
            }
        }
        return this;
    }
}