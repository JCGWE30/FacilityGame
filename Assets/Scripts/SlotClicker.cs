using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotClicker : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent callEvent;
    private Button button;
    // Start is called before the first frame update
    public void OnPointerDown(PointerEventData eventData)
    {
        callEvent.Invoke();
    }
}
