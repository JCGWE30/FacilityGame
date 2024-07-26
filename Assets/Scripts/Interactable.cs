using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public Sprite InfoImage;
    public string InfoText;
    public void BaseInteract(bool alt)
    {
        Interact(alt);
    }
    protected virtual void Interact(bool alt)
    {

    }
}
