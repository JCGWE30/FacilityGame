using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Slot
{
    None,
    Back,
    Belt,
    Vest,
    Uniform,
    Helmet,
    Weapon,
    Hold
}
public class ItemDesc : MonoBehaviour
{
    public string displayName;
    public GameObject worldModel;
    public GameObject viewModel;
    public Sprite sprite;
    public Slot itemType;
    [SerializeField]
    public string id;

    public bool Match(ItemDesc desc)
    {
        Debug.Log(id);
        Debug.Log(desc.id);
        return desc.id == id;
    }
}
