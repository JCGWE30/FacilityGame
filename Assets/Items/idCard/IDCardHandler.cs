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
    public Sprite adminSprite;
    public Sprite defaultSprite;

    public AccessPreset preset;
    public bool testAccess;
    public bool HasAccess(Access access)
    {
        switch (access)
        {
            case Access.Test:
                return testAccess;
        }
        return false;
    }
    // Start is called before the first frame update
    private void Start()
    {
        ApplyAccessPreset(AccessPreset.Admin);
    }
    public void ApplyAccessPreset(AccessPreset access)
    {
        preset = access;
        testAccess = false;
        GetComponent<ItemDesc>().displayName = preset + " ID";
        GetComponent<ItemDesc>().sprite = adminSprite;
        switch (access)
        {
            case AccessPreset.Admin:
                testAccess = true;
                break;
        }
    }
}
