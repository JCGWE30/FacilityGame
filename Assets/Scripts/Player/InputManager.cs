using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private PlayerInput playerInput;

    public PlayerInput.OnFootActions onFoot;
    public PlayerInput.CommandBoxActions commandBox;
    public PlayerInput.InventoryActions inventoryButtons;
    public PlayerInput.LatheMenuActions latheButtons;
    public PlayerInput.ResearchTableActions researchButtons;
    public PlayerInput.DailyLogActions dailyLogButtons;
    public PlayerInput.TechComputerActions techButtons;
    public bool shiftHeld { get; private set; }

    public GameObject crosshair;

    public LatheUIManager latheManager;
    private PlayerMotion motion;
    private PlayerLook look;
    private InventoryManager inven;
    private CommandManager commands;

    public bool isMenuOpen { get; private set; } = false;
    // Start is called before the first frame update
    public enum ControlType
    {
        Basic,
        Inventory,
        CommandBox,
        Lathe,
        ResearchTable,
        DailyLog,
        TechComputer
    }
    void Awake()
    {
        if (instance == null)
            instance = this;
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        commandBox = playerInput.CommandBox;
        inventoryButtons = playerInput.Inventory;
        latheButtons = playerInput.LatheMenu;
        researchButtons = playerInput.ResearchTable;
        dailyLogButtons = playerInput.DailyLog;
        techButtons = playerInput.TechComputer;
        motion = GetComponent<PlayerMotion>();
        look = GetComponent<PlayerLook>();
        commands = GetComponent<CommandManager>();
        look.LockMouse();
        inven = GetComponent<InventoryManager>();
        onFoot.Command.performed += ctx => commands.OpenCommandBox();

        onFoot.Inventory.performed += ctx => inven.OpenInventory();

        onFoot.Slot1.performed += ctx => inven.HotbarClick(0);
        onFoot.Slot2.performed += ctx => inven.HotbarClick(1);
        onFoot.Slot3.performed += ctx => inven.HotbarClick(2);
        onFoot.Slot4.performed += ctx => inven.HotbarClick(3);
        onFoot.Slot5.performed += ctx => inven.HotbarClick(4);

        inventoryButtons.Shift.performed += ctx => shiftStart();
        inventoryButtons.Shift.canceled += ctx => shiftEnd();
        inventoryButtons.Close.canceled += ctx => inven.CloseInventory();

        latheButtons.Close.performed += ctx => latheManager.Close();

        researchButtons.Close.performed += ctx => ResearchTableUI.instance.Close();

        dailyLogButtons.Close.performed += ctx => DailyLogUI.instance.Close();

        techButtons.Close.performed += ctx => TechComputerUI.instance.Close();

        commandBox.SendCommand.performed += ctx => commands.SendCommand();
        commandBox.Exit.performed += ctx => commands.ExitCommand();
    }

    private void shiftStart()
    {
        shiftHeld = true;
    }
    private void shiftEnd()
    {
        shiftHeld = false;
    }

    public PlayerInput getInput()
    {
        return playerInput;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isMenuOpen)
            motion.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    public void setControls(ControlType type)
    {
        disableAll();
        switch (type)
        {
            case ControlType.Basic:
                onFoot.Enable();
                break;
            case ControlType.CommandBox:
                commandBox.Enable();
                break;
            case ControlType.Inventory:
                inventoryButtons.Enable();
                break;
            case ControlType.Lathe:
                latheButtons.Enable();
                break;
            case ControlType.ResearchTable:
                researchButtons.Enable();
                break;
            case ControlType.DailyLog:
                dailyLogButtons.Enable();
                break;
            case ControlType.TechComputer:
                techButtons.Enable();
                break;
            default:
                onFoot.Enable();
                break;
        }
    }

    private void disableAll()
    {
        onFoot.Disable();
        commandBox.Disable();
        inventoryButtons.Disable();
        latheButtons.Disable();
        researchButtons.Disable();
        dailyLogButtons.Disable();
        techButtons.Disable();
    }

    public void MenuOpen()
    {
        inven.hotbar.gameObject.SetActive(false);
        crosshair.SetActive(false);
        isMenuOpen = true;
        look.UnlockMouse();
    }
    public void MenuClose()
    {
        inven.hotbar.gameObject.SetActive(true);
        crosshair.SetActive(true);
        isMenuOpen = false;
        look.LockMouse();
    }

    private void LateUpdate()
    {
        if(!isMenuOpen)
            look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }
    private void OnDisable()
    {
        disableAll();
    }
}
