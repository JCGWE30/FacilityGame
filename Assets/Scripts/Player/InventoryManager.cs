using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public LayerMask floorMask;

    public GameObject inventory;
    public EquipmentSlotManager equipmentSlots;
    public SlotManager backpackSlots;
    public SlotManager beltSlots;
    public HotbarWatcher hotbar;
    public StorageSlotManager storageSlots;

    public int tweakx = 0;
    public int tweaky = 0;

    private InventorySlot cursorSlot;
    private bool shifting;
    public Image cursorImage;
    private InventorySlot hoveredSlot;
    public TMP_Text infoText;

    private int idcount = 0;

    private bool isOpen;

    private InputManager playerInput;

    void Awake()
    {
        if (instance == null)
            instance = this;
        equipmentSlots.InitManager();
        backpackSlots.InitManager();
        beltSlots.InitManager();
        storageSlots.InitManager();
        hotbar.Init();
        playerInput = GetComponent<InputManager>();
    }

    public int GetNewId()
    {
        idcount += 1;
        return idcount-1;
    }

    public void HotbarClick(int slot)
    {
        hotbar.Click(this, slot);
    }

    public void OpenInventory()
    {
        inventory.SetActive(true);
        playerInput.MenuOpen();
        playerInput.setControls(InputManager.ControlType.Inventory);
    }

    public void CloseInventory()
    {
        if (storageSlots.isOpen)
        {
            storageSlots.Close();
            return;
        }
        inventory.SetActive(false);
        playerInput.MenuClose();
        playerInput.setControls(InputManager.ControlType.Basic);
    }

    public void SlotClick(SlotManager manager, int id)
    {
        shifting = false;
        if (manager.container.GetItem(id).item == null)
            return;
        if (playerInput.inventoryButtons.Shift.IsPressed())
        {
            StartCoroutine(ShiftClickCheck(manager,id));
            return;
        }
        else
        {
            cursorSlot = manager.GetSlot(id);
        }

    }

    public IEnumerator ShiftClickCheck(SlotManager manager, int id)
    {
        yield return new WaitForSeconds(0.1f);
        if (!playerInput.inventoryButtons.MouseClick.IsPressed())
        {
            ShiftClick(SlotToItem(manager.GetSlot(id)));
        }
        else
        {
            shifting = true;
            cursorSlot = manager.GetSlot(id);
        }
    }

    private void ShiftClick(ItemSlot slot)
    {
        List<SlotManager> managers = new List<SlotManager>();
        managers.Add(equipmentSlots);
        if(backpackSlots.gameObject.activeSelf)
            managers.Add(backpackSlots);
        if (beltSlots.gameObject.activeSelf)
            managers.Add(beltSlots);
        if (storageSlots.container == slot.GetComponentInParent<StorageContainer>())
        {
            managers.Add(storageSlots);
        }
        else
        {
            managers.Insert(0, storageSlots);
        }

        foreach (var slotm in managers)
        {
            if (slotm.container == null)
                continue;
            foreach (var item in slotm.container.GetItems())
            {
                if (slot.item.GetComponent<MovementChecker>().TryMove(slot, item, false))
                    return;
            }
        }
    }

    private InventorySlot GetHoveredSlot()
    {
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        foreach (var result in results)
        {
            InventorySlot slot;
            if (result.gameObject.TryGetComponent(out slot))
                return slot;
        }
        return null;
    }

    private bool OnInventory()
    {
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        List<GameObject> finalObjects = new List<GameObject>();
        foreach (var result in results)
        {
            if (result.gameObject.name == "InventoryPanel" || result.gameObject.name == "StoragePanel")
                return true;
        }
        return false;
    }

    public bool ArmItem(ItemDesc item)
    {
        MovementChecker checker = item.GetComponent<MovementChecker>();
        return checker.TryInsert(equipmentSlots.equipmentContainer.GetEquipmentItem(SlotType.Hand));
    }

    public void Disarm()
    {
        ItemSlot item = EquipmentContainer.instance.GetEquipmentItem(SlotType.Hand);

        Debug.Log(item.item);

        Destroy(item.item);
    }

    private ItemSlot SlotToItem(InventorySlot slot)
    {
        return slot.slotManager.container.GetItem(slot.id);
    }

    private void DropItemOnCursor()
    {
        ItemSlot slot = SlotToItem(cursorSlot);
        //ItemDesc dropItem =
        //    slot.item.GetComponent<MovementChecker>()
        //    .TryDrop(SlotToItem(cursorSlot), shifting);
        PlayerNetworker.localInstance.TryItemDropRpc(slot.item.getIdentifier(), NetworkManager.Singleton.LocalClientId, shifting);
        //if (dropItem != null)
        //{
        //    GameObject droppedItem = new GameObject("DroppedItem");
        //    droppedItem.transform.position = gameObject.transform.position;
        //    dropItem.transform.parent = droppedItem.transform;
        //    droppedItem.AddComponent<DroppedItem>();
        //}
    }

    void Update()
    {
        Vector2 oldpos = playerInput.inventoryButtons.MousePosition.ReadValue<Vector2>();
        infoText.transform.position = new Vector2(oldpos.x+tweakx,oldpos.y+tweaky);
        if (cursorSlot != null)
        {
            cursorImage.gameObject.SetActive(true);
            if (OnInventory())
                cursorImage.sprite = cursorSlot.itemSprite;
            else
                cursorImage.sprite = SpriteHolderObject.instance.get().Drop;
            cursorImage.transform.position = playerInput.inventoryButtons.MousePosition.ReadValue<Vector2>();

            if (hoveredSlot != null)
                hoveredSlot.isHovered = false;
            hoveredSlot = GetHoveredSlot();
            if (hoveredSlot != null &&
                SlotToItem(cursorSlot).item.GetComponent<MovementChecker>().CanMove(SlotToItem(cursorSlot), SlotToItem(hoveredSlot), shifting))
                hoveredSlot.isHovered = true;

            if (!playerInput.inventoryButtons.MouseClick.IsPressed())
            {
                if (!OnInventory())
                {
                    DropItemOnCursor();
                }
                else if (hoveredSlot != null)
                {
                    hoveredSlot.isHovered = false;
                    SlotToItem(cursorSlot).item.GetComponent<MovementChecker>().TryMove(SlotToItem(cursorSlot), SlotToItem(hoveredSlot), shifting);
                }
                cursorSlot = null;
            }
        }
        else
        {
            if (GetHoveredSlot() != null && SlotToItem(GetHoveredSlot()).item!=null)
            {
                infoText.text = SlotToItem(GetHoveredSlot()).item.displayName;
            }
            else
            {
                infoText.text = string.Empty;
            }
            cursorImage.gameObject.SetActive(false);
        }
    }
}