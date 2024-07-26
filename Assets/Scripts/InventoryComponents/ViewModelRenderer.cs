using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewModelRenderer : MonoBehaviour
{
    private ItemSlot handSlot;
    private GameObject viewModel;

    void Update()
    {
        if (GetComponent<EquipmentContainer>().GetEquipmentItem(SlotType.Hand).item == null)
        {
            if (viewModel != null)
                Destroy(viewModel);
        }
        else
        {
            if (viewModel == null)
            {
                GameObject vm = GetComponent<EquipmentContainer>().GetEquipmentItem(SlotType.Hand).item.viewModel;
                viewModel = Instantiate(vm);
                viewModel.transform.parent = GetComponentInChildren<Camera>().transform;
                viewModel.transform.localPosition = vm.transform.localPosition;
                viewModel.transform.localRotation = vm.transform.rotation;
            }
        }
    }
}
