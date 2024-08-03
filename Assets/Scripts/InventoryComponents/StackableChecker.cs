using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct StackableCheckerData : INetworkSerializable
{
    public int amount;
    public int maxStackSize;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref amount);
        serializer.SerializeValue(ref maxStackSize);
    }
}

public class StackableChecker : MovementChecker, ISerializableComponent
{
    public int amount;
    [SerializeField]
    private int maxStackSize;
    private void Start()
    {
        if(amount==0)
            amount = 1;
    }
    private void Update()
    {
        if (amount == 0)
            Destroy(gameObject);
    }
    protected override bool GetCanMove(ItemSlot from, ItemSlot to, bool shifting)
    {
        if (to.item == null && InventorySlot.CanItemFit(to.slotType, from.item.itemType))
            return true;
        if (to.item == null)
            return false;
        if (to.GetInstanceID() == from.GetInstanceID())
            return false;
        if (!to.item.Match(GetComponent<ItemDesc>()))
            return false;
        int toamount = to.item.GetComponent<StackableChecker>().amount;
        if (toamount < maxStackSize)
            return true;
        return false;
    }

    private int[] amountbalance(int donor, int recepient)
    {

        if (maxStackSize - recepient < donor)
        {
            recepient = maxStackSize;
            donor -= maxStackSize - recepient;
        }
        else
        {
            recepient += donor;
            donor = 0;
        }

        return new int[] { donor, recepient };
    }

    protected override bool GetTryMove(ItemSlot from, ItemSlot to, bool shifting)
    {
        if (!CanMove(from, to, shifting))
            return false;
        if (to.item == null)
        {
            if (shifting)
            {
                ItemDesc newitem = Instantiate(gameObject).GetComponent<ItemDesc>();
                to.GetComponentInParent<StorageContainer>().SetItem(to.id, newitem);
                newitem.GetComponent<StackableChecker>().amount = (int)Mathf.Ceil((float)amount / 2);
                amount = (int)Mathf.Floor((float)amount / 2);
            }
            else
            {
                to.GetComponentInParent<StorageContainer>().SetItem(to.id, GetComponent<ItemDesc>());
            }
        }
        else
        {
            if (shifting)
            {
                int curamount = amount;
                amount = (int)Mathf.Floor((float)curamount / 2);
                curamount = (int)Mathf.Ceil((float)curamount / 2);
                StackableChecker tostack = to.item.GetComponent<StackableChecker>();
                int[] amounts = amountbalance(curamount, tostack.amount);
                amount += amounts[0];
                tostack.amount = amounts[1];
                return amounts[0] == 0;
            }
            else
            {
                StackableChecker tostack = to.item.GetComponent<StackableChecker>();
                int[] amounts = amountbalance(amount, tostack.amount);
                amount = amounts[0];
                tostack.amount = amounts[1];
                return amounts[0] == 0;
            }
        }
        return true;
    }

    protected override bool GetTryInsert(ItemSlot to)
    {
        Debug.Log("Try insert called");
        if (to.item == null)
        {
            to.GetComponentInParent<StorageContainer>().SetItem(to.id, GetComponent<ItemDesc>());
            return true;
        }
        if (!to.item.Match(GetComponent<ItemDesc>()))
            return false;
        StackableChecker recipient = to.item.GetComponent<StackableChecker>();
        int[] amounts = amountbalance(amount,recipient.amount);
        amount = amounts[0];
        recipient.amount = amounts[1];
        return amounts[0]==0;
    }

    protected override ItemDesc GetTryDrop(ItemSlot from,bool shifting)
    {
        if (shifting)
        {
            ItemDesc newitem = Instantiate(gameObject).GetComponent<ItemDesc>();
            newitem.GetComponent<StackableChecker>().amount = (int)Mathf.Ceil((float)amount / 2);
            amount = (int)Mathf.Floor((float)amount / 2);
            return newitem;
        }
        else
        {
            return GetComponent<ItemDesc>();
        }
    }

    public INetworkSerializable SerializeComponent()
    {
        return new StackableCheckerData
        {
            amount = amount,
            maxStackSize = maxStackSize
        };
    }

    public void DeserializeComponent(INetworkSerializable serializedComponent)
    {
        StackableCheckerData? data = serializedComponent as StackableCheckerData?;
        if (data.HasValue)
        {
            amount = data.Value.amount;
            maxStackSize = data.Value.maxStackSize;
        }
    }
}
