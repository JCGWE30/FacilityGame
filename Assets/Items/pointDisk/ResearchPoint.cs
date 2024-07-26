using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchPoint : MonoBehaviour
{
    public int pointValue;
    public string testName;

    private ItemDesc desc;

    void Start()
    {
        desc = GetComponent<ItemDesc>();
        desc.displayName = testName + " Results Disk (" + pointValue + ") points";
    }
}
