using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarWatcher : MonoBehaviour
{
    public StorageContainer container;
    // Start is called before the first frame update
    private void Awake()
    {
        foreach (Transform item in transform)
        {
            Image image = new GameObject("Image").AddComponent<Image>();
            image.transform.parent = item;
            image.rectTransform.localPosition = Vector2.zero;
            image.rectTransform.sizeDelta = new Vector2(60, 60);
            image.gameObject.SetActive(false);
        }
    }

    public void Click(InventoryManager manager,int click)
    {
        if (container == null)
            return;
        if (container.slotCount <= click)
            return;

        ItemSlot handslot = manager.equipmentSlots.equipmentContainer.GetEquipmentItem(SlotType.Hand);
        ItemSlot hotbarslot = manager.beltSlots.container.GetItem(click);

        if (hotbarslot.item == null && handslot.item == null)
            return;

        if (handslot.item == null)
        {
            MovementChecker checker = hotbarslot.item.GetComponent<MovementChecker>();
            checker.TryMove(hotbarslot, handslot, false);
        }
        else if (handslot.item != null)
        {
            MovementChecker checker = handslot.item.GetComponent<MovementChecker>();
            checker.TryMove(handslot, hotbarslot, false);
        }
    }

    private void Update()
    {
        if (container == null)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        int count = 0;
        foreach (Transform item in transform)
        {
            if (container.slotCount > count)
            {
                item.gameObject.SetActive(true);
                Image image = item.GetChild(1).GetComponent<Image>();
                if (container.GetItem(count).item != null)
                {
                    image.gameObject.SetActive(true);
                    image.sprite = container.GetItem(count).item.sprite;
                }
                else
                    image.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
            count++;
        }
    }
}
