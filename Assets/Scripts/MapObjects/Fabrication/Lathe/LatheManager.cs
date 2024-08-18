using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class LatheRecipe
{
    public int woodCost;
    public int plasticCost;
    public int ironCost;

    public int resultAmount;
    public string resultItem;

    public long craftTime;

    public LatheRecipe(int wood, int plastic, int iron, int result, string item, long craft)
    {
        woodCost = wood;
        plasticCost = plastic;
        ironCost = iron;

        resultAmount = result;
        resultItem = item;
        craftTime = craft;
    }
}

public class LatheManager : NetworkBehaviour
{
    private int maxResource = 100;

    private LatheUIManager latheMenu;
    public List<LatheRecipe> recipes = new List<LatheRecipe>();  

    private void Start()
    {
        latheMenu = LatheUIManager.instance;
        recipes = new List<LatheRecipe>(new[]
        {
            new LatheRecipe(1,1,1,1,"itemCrate",0)
        });
        GetComponent<Interactable>().Setup(SpriteEnum.FabricationIcon, "Open Lathe")
            .OnInteract += Interact;
    }

    public NetworkVariable<int> selectedRecipe = new NetworkVariable<int>(0);
    public LatheRecipe currentRecipe { get { return recipes[selectedRecipe.Value]; } }
    public NetworkVariable<float> craftingFinish = new NetworkVariable<float>(0);
    public NetworkVariable<bool> isCrafting = new NetworkVariable<bool>(false);

    public NetworkVariable<int> _iron = new NetworkVariable<int>(0);
    public NetworkVariable<int> _wood = new NetworkVariable<int>(0);
    public NetworkVariable<int> _plastic = new NetworkVariable<int>(0);

    public int iron{ get { return _iron.Value; }}
    public int wood { get { return _wood.Value; }}
    public int plastic { get { return _plastic.Value; }}

    private int[] amountbalance(int donor, int recepient)
    {

        if (maxResource - recepient < donor)
        {
            recepient = maxResource;
            donor -= maxResource - recepient;
        }
        else
        {
            recepient += donor;
            donor = 0;
        }

        return new int[] { donor, recepient };
    }

    public void CraftItem(LatheRecipe recipe)
    {
        ItemDesc result = ItemFinder.Find(recipe.resultItem);
        if (result == null)
            return;
        if (isCrafting.Value == true)
            return;

        StartCraftRpc(recipes.IndexOf(recipe));
    }

    private void Interact()
    {
        if (InputManager.instance.onFoot.AltHeld.IsPressed())
        {
            ItemSlot item = EquipmentContainer.instance.GetEquipmentItem(SlotType.Hand);
            if (item.item != null)
            {
                AddMaterialToLatheRpc(item.id, NetworkManager.Singleton.LocalClientId);
            }
            else
            {
                InventoryManager.instance.storageSlots.Open(GetComponent<LatheContainer>());
            }
        }
        else
        {
            LatheUIManager.instance.Open(this);
        }
    }

    [Rpc(SendTo.Server)]
    private void StartCraftRpc(int recId)
    {
        LatheRecipe recipe = recipes[recId];
        if (isCrafting.Value)
            return;
        ItemDesc result = ItemFinder.Find(recipe.resultItem);
        if (result == null)
            return;
        bool fits = false;
        foreach(ItemSlot slot in GetComponent<StorageContainer>().GetItems())
        {
            if (result.checker.CanFit(slot))
            {
                fits = true;
                break;
            }
        }

        if (!fits)
            return;

        isCrafting.Value = true;
        craftingFinish.Value = NetworkManager.Singleton.ServerTime.TimeAsFloat + recipe.craftTime;
        selectedRecipe.Value = recId;
    }

    [Rpc(SendTo.Server)]
    public void AddMaterialToLatheRpc(long id,ulong sender)
    {
        ItemSlot slot = GlobalIdentifier.FetchObject<ItemSlot>(id);
        ItemDesc item = slot.item;

        if (item == null) return;

        int finalAmount = 0;
        switch (item.id)
        {
            case "plastic":
                int[] amounts = amountbalance(item.GetComponent<StackableChecker>().amount, plastic);
                item.GetComponent<StackableChecker>().amount = amounts[0];
                finalAmount = amounts[0];
                _plastic.Value = amounts[1];
                break;
            case "wood":
                amounts = amountbalance(item.GetComponent<StackableChecker>().amount, wood);
                item.GetComponent<StackableChecker>().amount = amounts[0];
                finalAmount = amounts[0];
                _wood.Value = amounts[1];
                break;
            case "iron":
                amounts = amountbalance(item.GetComponent<StackableChecker>().amount, iron);
                item.GetComponent<StackableChecker>().amount = amounts[0];
                finalAmount = amounts[0];
                _iron.Value = amounts[1];
                break;
            default:
                return;
        }
        FinalizeMaterialAmountRpc(id, finalAmount, RpcTarget.Single(sender, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void FinalizeMaterialAmountRpc(long slotId, int amount, RpcParams rpcParams)
    {
        ItemSlot slot = EquipmentContainer.instance.GetItemByID(slotId);
        ItemDesc item = slot.item;

        if (item == null) return;

        item.GetComponent<StackableChecker>().amount = amount;
    }

    [Rpc(SendTo.NotServer)]
    public void AddItemToLatheRpc(SerializedGameObject serializedObject, long slotId)
    {
        ItemDesc item = ItemSerializer.deserializeGameObject(serializedObject).GetComponent<ItemDesc>();
        if (item == null)
            return;
        ItemSlot slot = GlobalIdentifier.FetchObject<ItemSlot>(slotId);
        item.checker.TryInsert(slot);
    }

    private void Update()
    {
        if (!IsServer)
            return;
        if (!isCrafting.Value)
            return;
        if (craftingFinish.Value >= NetworkManager.Singleton.ServerTime.TimeAsFloat)
            return;

        LatheRecipe recipe = currentRecipe;
        isCrafting.Value = false;
        ItemDesc craftedItem = ItemFinder.FindInstanced(recipe.resultItem);

        if (recipe.resultAmount > 1)
            (craftedItem.checker as StackableChecker).amount = recipe.resultAmount;

        foreach(ItemSlot slot in GetComponent<StorageContainer>().GetItems())
        {
            if (craftedItem.checker.TryInsert(slot))
            {
                AddItemToLatheRpc(ItemSerializer.serializeGameObject(craftedItem.gameObject), slot.id);
                return;
            }
        }
    }
}
