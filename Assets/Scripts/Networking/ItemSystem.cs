using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemSystem : MonoBehaviour
{
    public static ItemSystem instance;

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;
        if (instance == null)
            instance = this;
    }

    public void DropItem(GameObject item)
    {

    }
}
