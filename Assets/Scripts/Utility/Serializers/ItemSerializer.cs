using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public struct SerializedComponent : INetworkSerializable
{
    public INetworkSerializable data;
    public string componentName;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        data.NetworkSerialize(serializer);
        serializer.SerializeValue(ref componentName);
    }
}
public struct SerializedGameObject : INetworkSerializable
{
    public string name;
    public FixedString128Bytes itemId;
    public SerializedComponent[] components;
    public SerializedGameObject[] children;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref itemId);
        serializer.SerializeValue(ref components);
        serializer.SerializeValue(ref children);
    }
}
public class ItemSerializer
{
    public static SerializedGameObject serializeGameObject(GameObject gameObject)
    {
        List<SerializedComponent> components = new List<SerializedComponent>();
        List<SerializedGameObject> children = new List<SerializedGameObject>();

        foreach(Component component in gameObject.GetComponents<Component>())
        {
            if (component as ISerializableComponent == null)
                continue;
            components.Add(new SerializedComponent
            {
                data = (component as ISerializableComponent).SerializeComponent(),
                componentName = component.GetType().AssemblyQualifiedName
            });
        }

        foreach(Transform child in gameObject.transform)
        {
            children.Add(serializeGameObject(child.gameObject));
        }

        return new SerializedGameObject
        {
            name = gameObject.name,
            itemId = gameObject.GetComponent<ItemDesc>()?.id ?? string.Empty,
            components = components.ToArray(),
            children = children.ToArray()
        };
    }

    public static GameObject deserializeGameObject(SerializedGameObject serializedObject)
    {
        GameObject newObject;
        if (serializedObject.itemId.Length>0)
        {
            newObject = Object.Instantiate(ItemFinder.getItemByID(serializedObject.itemId.ToString()));
            newObject.name = serializedObject.name;
        }
        else
        {
            newObject = new GameObject(serializedObject.name);
        }
        foreach(SerializedComponent component in serializedObject.components)
        {
            System.Type componentType = System.Type.GetType($"{component.componentName},Assembly-CSharp");
            ISerializableComponent newComponent = newObject.AddComponent(componentType) as ISerializableComponent;
            newComponent.DeserializeComponent(component.data);
        }
        foreach(SerializedGameObject child in serializedObject.children)
        {
            deserializeGameObject(child).transform.parent = newObject.transform;
        }
        return newObject;
    }
}
