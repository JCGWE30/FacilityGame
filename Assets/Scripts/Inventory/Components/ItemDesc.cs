using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.SceneManagement;
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
    [SerializeField]
    public string id;

    public MovementChecker checker { get 
        {
            return GetComponent<MovementChecker>();
        } }

    public string displayName;
    public GameObject worldModel;
    public GameObject viewModel;
    public string spriteName;
    private Sprite _sprite = null;
    public Sprite sprite
    { 
        get 
        {
            if (_sprite == null)
                _sprite = SpriteFinder.Find(spriteName);
            return _sprite;
        } 
    }
    public Slot itemType;

    public bool Match(ItemDesc desc)
    {
        return desc.id == id;
    }

    public ItemSlot GetSlot()
    {
        return GetComponentInParent<ItemSlot>();
    }

    public void SetSprite(Sprite sprite)
    {
        _sprite = sprite;
    }
}
