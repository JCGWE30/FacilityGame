using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Sighting : NetworkBehaviour
{

    public ItemDesc material;

    private NetworkVariable<bool> active = new NetworkVariable<bool>(true);

    private bool holding;
    private float holdTime = 2f;
    private float startedHolding;
    private float endtime;
    private Vector3 position;
    private GameObject harvestBar;

    public static Sighting sighting; //Have a better way of getting the sighting
    private void Start()
    {
        sighting = this;
        harvestBar = GameObject.Find("HUD/HarvestPanel");
        GetComponent<Interactable>().Setup(SpriteEnum.AnomolusMaterialItem, "Harvest Material")
            .OnInteract += Interact;
        active.OnValueChanged += ToggleActive;
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
        if (!IsServer)
            return;

        if (endtime < Time.time)
        {
            active.Value = false;
        }
    }

    public void StartSighting(float time)
    {
        active.Value = true;
        gameObject.SetActive(true);
        endtime = Time.time + time;
    }

    private void ToggleActive(bool _, bool state)
    {
        gameObject.SetActive(state);
    }

    private void Interact()
    {
        if (!holding)
        {
            startedHolding = Time.time;
            //ItemDesc giveItem = Instantiate(material);
            //giveItem.GetComponent<StackableChecker>().amount = 1;
            //if (InventoryManager.instance.ArmItem(giveItem))
            //    holding = true;
            //else
            //    Destroy(giveItem);
        }
    }

    [Rpc(SendTo.Server)]
    public void InteractRpc(ulong id)
    {

    }

    [Rpc(SendTo.Server)]
    public void StartSightingRpc()
    {
        StartSighting(10f);
    }
}
