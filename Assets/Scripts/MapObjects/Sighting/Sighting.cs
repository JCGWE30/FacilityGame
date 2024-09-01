using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Sighting : NetworkBehaviour
{
    private static readonly float HOLD_TIME = 2f;

    public ItemDesc material;

    private NetworkVariable<bool> active = new NetworkVariable<bool>(false);

    private bool holding;
    private float startedHolding;
    private float endtime;
    private Vector3 position;
    private GameObject harvestBar;

    private Dictionary<ulong, float> serverHoldTimes = new Dictionary<ulong, float>();

    public static Sighting sighting; //Have a better way of getting the sighting
    private void Start()
    {
        sighting = this;
        harvestBar = GameObject.Find("HUD/HarvestPanel");
        GetComponent<Interactable>().Setup(SpriteEnum.AnomolusMaterialItem, "Harvest Material")
            .OnInteract += Interact;
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("Setting toggle active");
        active.OnValueChanged += ToggleActive;
        gameObject.SetActive(active.Value);
    }

    private void Update()
    {
        if (holding)
        {
            float percent = (Time.time - startedHolding) / HOLD_TIME;
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
        Debug.Log("Active state " + state);
        gameObject.SetActive(state);
    }

    private void Interact()
    {
        ItemSlot slot = EquipmentContainer.instance.GetEquipmentItem(SlotType.Hand);
        string heldType = slot.item?.id ?? "anomolusMaterial";

        if (heldType != "anomolusMaterial")
            return;

        if (!holding)
        {
            startedHolding = Time.time;
            holding = true;
            InteractRpc(NetworkManager.Singleton.LocalClientId);
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
        EquipmentContainer container = NetworkManager.Singleton.ConnectedClients[id].PlayerObject.GetComponent<EquipmentContainer>();

        ItemSlot slot = container.GetEquipmentItem(SlotType.Hand);

        string heldType = slot.item?.id ?? "anomolusMaterial";

        if (heldType != "anomolusMaterial")
            return;

        float time = serverHoldTimes.TryGetValue(id, out var result) ? result : 0f;
        if (time + HOLD_TIME < Time.time)
        {
            ItemDesc item = ItemFinder.FindInstanced("anomolusMaterial");
            (item.checker as StackableChecker).amount = 1;

            item.checker.TryInsert(slot);

            if (item.transform.parent == null)
                Destroy(item.gameObject);

            PickupMaterialRpc(RpcTarget.Single(id, RpcTargetUse.Temp));
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void PickupMaterialRpc(RpcParams rpcParams)
    {
        ItemSlot slot = EquipmentContainer.instance.GetEquipmentItem(SlotType.Hand);
        ItemDesc item = ItemFinder.FindInstanced("anomolusMaterial");
        (item.checker as StackableChecker).amount = 1;

        item.checker.TryInsert(slot);

        if (item.transform.parent==null)
            Destroy(item.gameObject);
    }

    [Rpc(SendTo.Server)]
    public void StartSightingRpc()
    {
        StartSighting(10f);
    }
}
