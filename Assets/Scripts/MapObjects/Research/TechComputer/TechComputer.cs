using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechComputer : MonoBehaviour
{
    public static TechComputer instance;

    public TechDisk techDisk;

    public ItemDesc claimableDisk { get; private set; }
    public int computerState { get; private set; } = 0;
    public float startTime { get; private set; }
    private ResearchTech selectedTech;
    public void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        GetComponent<Interactable>().Setup(SpriteEnum.ResearchBackground, "Open Tech Computer")
            .OnInteract += Interact;
    }

    public void StartResearching(ResearchTech tech)
    {
        if (computerState == 0)
        {
            selectedTech = tech;
            computerState = 1;
            startTime = Time.time;
        }
    }

    public void ClearDisk()
    {
        claimableDisk = null;
        computerState = 0;
    }

    private void Update()
    {
        if (computerState == 1)
        {
            float timePassed = Time.time - startTime;

            if (timePassed > selectedTech.time)
            {
                ResearchServer.instance.PickNewTech(0);
                ResearchServer.instance.PickNewTech(1);
                ResearchServer.instance.PickNewTech(2);
                computerState = 2;
                TechDisk disk = Instantiate(techDisk);
                disk.tech = selectedTech;
                claimableDisk = disk.GetComponent<ItemDesc>();
            }
        }
    }

    private void Interact()
    {
        TechComputerUI.instance.Open();
    }
}
