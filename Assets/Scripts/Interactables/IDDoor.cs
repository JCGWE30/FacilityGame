using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDDoor : Interactable
{
    public string doorName;
    public Access accessneeded;
    public GameObject door;
    private float interacted;
    private bool open;

    // Start is called before the first frame update
    void Start()
    {
        InfoText = doorName + " (ID Needed)";
    }

    private void Update()
    {
        if (Time.time - interacted > 5f && open)
        {
            open = false;
            door.GetComponent<Animator>().SetBool("isOpen", false);
        }
    }

    protected override void Interact(bool alt)
    {
        ItemSlot handSlot = EquipmentContainer.instance.GetEquipmentItem(SlotType.Hand);
        if (handSlot.item == null)
            return;
        IDCardHandler keycard;
        if (!handSlot.item.TryGetComponent(out keycard))
            return;
        if (keycard.hasAccess(accessneeded))
        {
            open = true;
            door.GetComponent<Animator>().SetBool("isOpen", true);
            interacted = Time.time;
        }
        else
        {

        }
        base.Interact(alt);
    }
}
