using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyLog : Interactable, IBaseInitalizer
{
    public static DailyLog instance;

    private int diskCount;

    private float lastComitted = 0;

    public void Initalize()
    {
        if (instance == null)
            instance = this; 
    }

    private void Update()
    {
        diskCount = transform.childCount;
    }

    public void Commit()
    {
        lastComitted = Time.time;
        ResearchServer.instance.CommitDisks(GetComponentsInChildren<ItemDesc>());
    }

    public float GetCommitTimer()
    {
        return Mathf.Max(0, Mathf.Min(lastComitted*int.MaxValue, 30 - (Time.time - lastComitted)));
    }

    protected override void Interact(bool alt)
    {
        if (alt)
        {
            if (diskCount >= 10)
                return;
            if (EquipmentContainer.instance.HasItem(SlotType.Hand))
            {
                ItemDesc item = EquipmentContainer.instance.GetEquipmentItem(SlotType.Hand).item;
                if (item.id == "pointDisk")
                {
                    diskCount++;
                    item.transform.parent = transform;
                }
                if(item.id == "techDisk")
                {
                    diskCount++;
                    item.transform.parent = transform;
                }
            }
        }
        else
        {
            DailyLogUI.instance.Open();
        }
    }
}
