using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializeTest : MonoBehaviour
{
    public bool shouldSerialize = false;
    void Update()
    {
        if (shouldSerialize)
        {
            shouldSerialize = false;
            SerializedGameObject obj = ItemSerializer.serializeGameObject(gameObject);
            Debug.Log(obj);
            GameObject dobj = ItemSerializer.deserializeGameObject(obj);
            Debug.Log(dobj);
        }
    }
}
