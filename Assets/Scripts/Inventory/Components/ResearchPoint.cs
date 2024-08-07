using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchPoint : MonoBehaviour
{
    public int pointValue;
    public string testName;

    public ItemDesc desc { get; private set; }

    void Start()
    {
        desc = GetComponent<ItemDesc>();
        desc.displayName = testName + " Results Disk (" + pointValue + ") points";
    }
}
