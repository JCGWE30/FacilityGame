using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFinder : MonoBehaviour
{
    public SpriteHolder spriteHolder;

    private static SpriteFinder instance;

    private void Start()
    {
        if (instance == null)
            instance = this;
    }

    public static Sprite GetSprite(SpriteEnum spriteValue)
    {
        foreach(Sprite sprite in instance.spriteHolder.sprites)
        {
            if (sprite.name == spriteValue.ToString())
                return sprite;
        }
        return null;
    }

    public static Sprite GetSprite(string spriteValue)
    {
        foreach (Sprite sprite in instance.spriteHolder.sprites)
        {
            if (sprite.name == spriteValue)
                return sprite;
        }
        return null;
    }
}
