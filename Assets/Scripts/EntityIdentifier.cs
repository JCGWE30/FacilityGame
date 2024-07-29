using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityIdentifier : MonoBehaviour
{
    private static long lastId = -1;

    public long id { get; private set; }
    private bool isSet = false;
    // Start is called before the first frame update
    void Awake()
    {
        if (!isSet)
        {
            isSet = true;
            lastId++;
            id = lastId;
        }
    }

    public static EntityIdentifier fetchObject(long id)
    {
        foreach (EntityIdentifier entity in FindObjectsOfType<EntityIdentifier>())
        {
            if (entity.id == id)
                return entity;
        }
        return null;
    }

    public static T fetchObject<T>(long id)
    {
        foreach(EntityIdentifier entity in FindObjectsOfType<EntityIdentifier>())
        {
            if (entity.id == id)
                return confidentFetch<T>(entity);
        }
        return default(T);
    }

    public static T fetchInstancedObject<T>(long id)
    {
        foreach (EntityIdentifier entity in FindObjectsOfType<EntityIdentifier>())
        {
            if (entity.id == id)
                return confidentFetch<T>(Instantiate(entity));
        }
        return default(T);
    }

    private static T confidentFetch<T>(EntityIdentifier id)
    {
        foreach(Component comp in id.GetComponents<Component>())
        {
            if (comp is T)
                return comp.GetComponent<T>();
        }
        return default(T);
    }
}
