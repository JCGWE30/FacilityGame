using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotType
{
    Item,
    Hand,
    Back,
    Belt,
    Uniform,
    Vest,
    Helmet,
    Weapon,
    None
}

public class InventorySlot : MonoBehaviour, IPointerDownHandler
{
    public Sprite defualtImage;
    public Sprite itemSprite;
    public SlotType slotType;
    public SpriteHolder sprites;

    public SlotManager slotManager { get; private set; }

    private ItemDesc heldItem;

    private InventoryManager manager;
    private Image imageObject;
    private TextMeshProUGUI stackSize;
    private Dictionary<SlotType, Sprite> slotsprites = new Dictionary<SlotType,Sprite>();
    public bool overrideHover;
    public bool isHovered;
    public int id;
    public bool locked;

    public static bool CanItemFit(SlotType ty, Slot slot)
    {
        switch (ty)
        {
            case SlotType.Hand:
                return true;
            case SlotType.Item:
                return slot == Slot.None;
            case SlotType.Back:
                return slot == Slot.Back || slot == Slot.Weapon;
            case SlotType.Belt:
                return slot == Slot.Belt;
            case SlotType.Uniform:
                return slot == Slot.Uniform;
            case SlotType.Vest:
                return slot == Slot.Vest;
            case SlotType.Helmet:
                return slot == Slot.Helmet;
            case SlotType.Weapon:
                return slot == Slot.Weapon || slot == Slot.None;
            case SlotType.None:
                return false;

        }
        return false;
    }

    private void Awake()
    {
        slotManager = GetComponentInParent<SlotManager>();
        manager = InventoryManager.instance;

        slotsprites.Add(SlotType.Item, SpriteFinder.Find(SpriteEnum.ItemSlot));
        slotsprites.Add(SlotType.Hand, SpriteFinder.Find(SpriteEnum.HandSlot));
        slotsprites.Add(SlotType.Back, SpriteFinder.Find(SpriteEnum.BackSlot));
        slotsprites.Add(SlotType.Belt, SpriteFinder.Find(SpriteEnum.BeltSlot));
        slotsprites.Add(SlotType.Uniform, SpriteFinder.Find(SpriteEnum.UniformSlot));
        slotsprites.Add(SlotType.Vest, SpriteFinder.Find(SpriteEnum.VestSlot));
        slotsprites.Add(SlotType.Helmet, SpriteFinder.Find(SpriteEnum.HelmetSlot));
        slotsprites.Add(SlotType.Weapon, SpriteFinder.Find(SpriteEnum.WeaponSlot));
        slotsprites.Add(SlotType.None, SpriteFinder.Find(SpriteEnum.EmptySlot));

        imageObject = new GameObject("Image").AddComponent<Image>();
        imageObject.rectTransform.sizeDelta = new Vector2(150, 150);
        imageObject.sprite = defualtImage;
        imageObject.transform.parent = gameObject.transform;
        imageObject.transform.localScale = new Vector3(1, 1, 1);
        imageObject.transform.localPosition = Vector3.zero;


        stackSize = new GameObject("StackSize").AddComponent<TextMeshProUGUI>();
        stackSize.rectTransform.sizeDelta = new Vector2(90, 50);
        stackSize.alignment = TextAlignmentOptions.Right;
        stackSize.color = Color.black;
        stackSize.transform.parent = gameObject.transform;
        stackSize.transform.localPosition = new Vector3(23,-58,0);
    }

    public void SetItem(ItemDesc item)
    {
        heldItem = item;
    }
    private void Update()
    {
        if (heldItem == null)
        {
            itemSprite = null;
            imageObject.sprite = slotsprites[slotType];
        }
        else
        {
            itemSprite = heldItem.sprite;
            imageObject.sprite = heldItem.sprite;
        }
        if (!isHovered)
            overrideHover = false;
        StackableChecker checker;
        if (heldItem!=null && heldItem.TryGetComponent(out checker))
        {
            stackSize.text = "x"+checker.amount;
        }
        else
        {
            stackSize.text = string.Empty;
        }

        if (isHovered)
            imageObject.color = Color.yellow;
        else
            imageObject.color = new Color(56, 56, 56, 199);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        manager.SlotClick(GetComponentInParent<SlotManager>(),id);
    }
}
