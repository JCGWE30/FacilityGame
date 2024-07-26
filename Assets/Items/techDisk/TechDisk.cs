using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechDisk : MonoBehaviour
{
    public ResearchTech tech;

    private ItemDesc desc;

    void Start()
    {
        desc = GetComponent<ItemDesc>();
        if (tech == null)
            desc.displayName = "Empty Tech Disk";
        else
            desc.displayName = tech.id+" Tech Disk";
    }
}
