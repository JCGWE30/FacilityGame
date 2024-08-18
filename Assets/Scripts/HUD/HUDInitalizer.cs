using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInatalizer
{
    public void Initalize();
}
public class HUDInitalizer : MonoBehaviour
{
    private void Awake()
    {
        IInatalizer[] objects = gameObject.GetComponentsInChildren<IInatalizer>(true);

        foreach (var item in objects)
        {
            item.Initalize();
        }
    }
}
