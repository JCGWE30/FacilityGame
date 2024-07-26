using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Sighting : Interactable
{
    public ItemDesc material;

    private bool started;
    private bool holding;
    private float holdTime = 2f;
    private float startedHolding;
    private float endtime;
    private Vector3 position;
    private GameObject harvestBar;

    private static Sighting sighting;
    public static Sighting getSighting()
    {
        return sighting;
    }
    private void Start()
    {
        sighting = this;
        harvestBar = GameObject.Find("HUD/HarvestPanel");
    }

    private void Update()
    {
        if (holding)
        {
            float percent = (Time.time - startedHolding) / holdTime;
            harvestBar.transform.Find("Percent").GetComponent<Image>().fillAmount = 1-percent;
            if (percent >= 1)
            {
                holding = false;
            }
            harvestBar.SetActive(true);
        }
        else
        {
            harvestBar.SetActive(false);
        }
        if (endtime < Time.time)
        {
            started = false;
            gameObject.SetActive(false);
        }
    }

    public void StartSighting(float time)
    {
        gameObject.SetActive(true);
        started = true;
        endtime = Time.time + time;
    }

    protected override void Interact(bool alt)
    {
        if (!holding)
        {
            startedHolding = Time.time;
            ItemDesc giveItem = Instantiate(material);
            giveItem.GetComponent<StackableChecker>().amount = 1;
            if (InventoryManager.instance.ArmItem(giveItem))
                holding = true;
            else
                Destroy(giveItem);
        }
    }
}
