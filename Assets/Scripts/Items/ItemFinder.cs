using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemFinder : MonoBehaviour
{
    private static ItemFinder instance;
    public ItemList itemList;

    void Start()
    {
        if (instance == null)
            instance = this;
    }

    public static ItemDesc Find(string id)
    {
        foreach(GameObject obj in instance.itemList.items)
        {
            if (obj.GetComponent<ItemDesc>().id == id)
                return obj.GetComponent<ItemDesc>();
        }
        return null;
    }

    public static ItemDesc FindInstanced(string id)
    {
        foreach (GameObject obj in instance.itemList.items)
        {
            if (obj.GetComponent<ItemDesc>().id == id)
                return Instantiate(obj.GetComponent<ItemDesc>());
        }
        return null;
    }
}
