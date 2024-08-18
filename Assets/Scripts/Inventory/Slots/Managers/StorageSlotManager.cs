using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class StorageSlotManager : SlotManager
{
    public TMP_Text text;
    public Image inventorypanel;
    public bool isOpen { get; private set; } = false;

    public void Open(StorageContainer container)
    {
        InputManager input = InputManager.instance;
        input.MenuOpen();
        input.setControls(InputManager.ControlType.Inventory);
        inventorypanel.transform.localPosition = new Vector3(-467,0,0);
        inventorypanel.gameObject.SetActive(true);
        isOpen = true;
        text.text = container.GetComponent<ItemDesc>()?.displayName ?? "Container";
        gameObject.transform.parent.gameObject.SetActive(true);
        this.container = container;
    }

    public void Close()
    {
        InputManager input = InputManager.instance;
        input.MenuClose();
        input.setControls(InputManager.ControlType.Basic);
        inventorypanel.rectTransform.localPosition = Vector3.zero;
        inventorypanel.gameObject.SetActive(false);
        gameObject.transform.parent.gameObject.SetActive(false);
        isOpen = false;
        this.container = null;
    }
}
