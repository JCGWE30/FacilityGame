using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AccessPreset
{
    Blank,
    Admin
}
public enum Access
{
    Test
}

public class IDCardHandler : MonoBehaviour
{
    public AccessPreset preset;
    public bool testAccess;

    private Sprite normalSprite;
    private Sprite adminSprite;

    private void Start()
    {
        normalSprite = SpriteFinder.GetSprite(SpriteEnum.IDItem);
        adminSprite = SpriteFinder.GetSprite(SpriteEnum.AdminIDItem);
        ApplyAccessPreset(AccessPreset.Admin);
    }
    public bool HasAccess(Access access)
    {
        switch (access)
        {
            case Access.Test:
                return testAccess;
        }
        return false;
    }
    public void ApplyAccessPreset(AccessPreset access)
    {
        preset = access;
        testAccess = false;
        GetComponent<ItemDesc>().displayName = preset + " ID";
        GetComponent<ItemDesc>().SetSprite(adminSprite);
        switch (access)
        {
            case AccessPreset.Admin:
                testAccess = true;
                break;
        }
    }
}
