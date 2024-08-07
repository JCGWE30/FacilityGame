using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDDoor : MonoBehaviour
{
    public string doorName;
    public Access accessneeded;
    public GameObject door;
    private float interacted;
    private bool open;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Interactable>().Setup(SpriteEnum.IDChipIcon, doorName + " (ID Needed)")
            .OnInteract += Interact;
    }

    private void Update()
    {
        if (Time.time - interacted > 5f && open)
        {
            open = false;
            door.GetComponent<Animator>().SetBool("isOpen", false);
        }
    }

    private void Interact()
    {
        ItemSlot handSlot = EquipmentContainer.instance.GetEquipmentItem(SlotType.Hand);
        if (handSlot.item == null)
            return;
        IDCardHandler keycard;
        if (!handSlot.item.TryGetComponent(out keycard))
            return;
        if (keycard.HasAccess(accessneeded))
        {
            open = true;
            door.GetComponent<Animator>().SetBool("isOpen", true);
            interacted = Time.time;
        }
        else
        {

        }
    }
}
