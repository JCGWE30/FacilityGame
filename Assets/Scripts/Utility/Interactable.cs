using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public delegate void InteractionHandler();

    public event InteractionHandler OnInteract;

    public Sprite infoImage { get; private set; }
    public string infoText { get; private set; }

    private void Awake()
    {
        gameObject.layer = 6;
    }

    public void Call()
    {
        OnInteract?.Invoke();
    }

    public Interactable Setup(Sprite sprite, string text)
    {
        infoImage = sprite;
        infoText = text;
        return this;
    }

    public Interactable Setup(SpriteEnum sprite, string text)
    {
        infoImage = SpriteFinder.Find(sprite);
        infoText = text;
        return this;
    }
}
