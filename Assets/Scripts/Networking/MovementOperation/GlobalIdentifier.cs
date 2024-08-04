using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalIdentifier : NetworkBehaviour
{
    private static long lastId = 0;
    public NetworkVariable<long> id { get; private set; } = new NetworkVariable<long>();

    private bool isSet = false;
    void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        if (!isSet)
        {
            id.Value = lastId;
            lastId++;
        }
        ItemSlot slot;
        if (TryGetComponent(out slot))
            slot.SetId(id.Value);
    }

    public static long InitalizeID(Object obj)
    {
        long id = lastId;
        lastId++;

        GlobalIdentifier slot = obj.GetOrAddComponent<GlobalIdentifier>();
        slot.id.Value = id;
        slot.isSet = true;
        return id;
    }

    public static T FetchObject<T>(long id)
    {
        foreach(GlobalIdentifier gid in FindObjectsOfType<GlobalIdentifier>())
        {
            Debug.Log($"{gid.gameObject} has id of {gid.id.Value}");
            if (gid.id.Value == id)
                return gid.GetComponent<T>();
        }
        return default(T);
    }

    public static GameObject FetchObject(long id)
    {
        foreach (GlobalIdentifier gid in FindObjectsOfType<GlobalIdentifier>())
        {
            if (gid.id.Value == id)
                return gid.gameObject;
        }
        return null;
    }
}
