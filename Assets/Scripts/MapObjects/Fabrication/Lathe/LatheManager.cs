using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LatheManager : Interactable
{
    private int maxResource = 100;
    private GameObject panel;
    private bool open;

    public LatheUIManager latheMenu;

    private void Start()
    {
        panel = GameObject.Find("/HUD/LatheMenu");
    }

    public int iron { get; private set; } = 0;
    public int wood { get; private set; } = 0;
    public int plastic { get; private set; } = 0;

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

    public bool CraftItem(Recipe recipe)
    {
        ItemDesc result = Instantiate(recipe.result);
        if (recipe.resultAmount > 1)
        {
            result.GetComponent<StackableChecker>().amount = recipe.resultAmount;
        }
        foreach (var item in GetComponent<LatheContainer>().GetItems())
        {
            if (item.item == null)
            {
                StartCoroutine(Craft(item, result,recipe.craftTime));
                return true;
            }
        }
        return false;
    }

    private IEnumerator Craft(ItemSlot slot, ItemDesc item, float waittime)
    {
        yield return new WaitForSeconds(waittime);
        item.transform.parent = slot.transform;
    }

    protected override void Interact(bool alt)
    {
        if (alt)
        {
            ItemSlot item = EquipmentContainer.instance.GetEquipmentItem(SlotType.Hand);
            if (item.item != null)
            {
                switch (item.item.id)
                {
                    case "plastic":
                        int[] amounts = amountbalance(item.item.GetComponent<StackableChecker>().amount, plastic);
                        item.item.GetComponent<StackableChecker>().amount = amounts[0];
                        plastic = amounts[1];
                        break;
                    case "wood":
                        amounts = amountbalance(item.item.GetComponent<StackableChecker>().amount, wood);
                        item.item.GetComponent<StackableChecker>().amount = amounts[0];
                        wood = amounts[1];
                        break;
                    case "iron":
                        amounts = amountbalance(item.item.GetComponent<StackableChecker>().amount, iron);
                        item.item.GetComponent<StackableChecker>().amount = amounts[0];
                        iron = amounts[1];
                        break;
                }
            }
            else
            {
                InventoryManager.instance.storageSlots.Open(GetComponent<LatheContainer>());
            }
        }
        else
        {
            latheMenu.Open(this);
        }
    }
}
