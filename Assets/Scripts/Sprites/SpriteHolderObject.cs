using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHolderObject : MonoBehaviour
{
    [SerializeField]
    public SpriteHolder holder;
    public static SpriteHolderObject instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public SpriteHolder get()
    {
        return holder;
    }

}
