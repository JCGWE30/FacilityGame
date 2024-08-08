using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CommandManager : MonoBehaviour
{
    private Transform player;
    private InputManager manager;
    private InventoryManager inven;
    public TMP_InputField CommandBox;
    public LayerMask floormask;

    public GameObject crate;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Transform>();
        manager = GetComponent<InputManager>();
        inven = GetComponent<InventoryManager>();
    }

    public void OpenCommandBox()
    {
        CommandBox.text = string.Empty;
        manager.setControls(InputManager.ControlType.CommandBox);
        manager.MenuOpen();
        CommandBox.gameObject.SetActive(true);
        CommandBox.ActivateInputField();
    }

    public void SendCommand()
    {
        string[] command = CommandBox.text.Split(" ");
        ExecuteCommand(command[0], command.Skip(1).ToArray());
        manager.setControls(InputManager.ControlType.Basic);
        manager.MenuClose();
        CommandBox.gameObject.SetActive(false);
    }

    private void ExecuteCommand(string command, string[] args)
    {
        if (command == "giveItem")
        {
            if (args.Length == 0)
                return;
            ItemDesc item = ItemFinder.FindInstanced(args[0]);
            if (item != null)
            {
                foreach(var slot in EquipmentContainer.instance.GetItems())
                {
                    if (item.checker.TryInsert(slot))
                    {
                        MovementOperation operation = new MovementOperation(args[0], slot.id, false);
                        operation.OnOperationFail += () => { Destroy(item); };
                        operation.OnOperationCanceled += () => { Destroy(item); };
                        operation.ProcessMove();
                        return;
                    }
                }
            }
        }
    }

    public void ExitCommand()
    {
        manager.setControls(InputManager.ControlType.Basic);
        manager.MenuClose();
        CommandBox.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
