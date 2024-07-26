using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InitilizerManager : MonoBehaviour
{
    void Start()
    {
        foreach (IBaseInitalizer item in FindObjectsOfType<MonoBehaviour>(true).OfType<IBaseInitalizer>())
        {
            item.Initalize();
        }
    }
}
