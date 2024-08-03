using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemHolderObject")]
public class ItemList : ScriptableObject
{
    public GameObject[] items;
}
