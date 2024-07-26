using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchTech
{
    public string id {get; private set; }
    public string name { get; private set; }
    public int cost { get; private set; }
    public float time { get; private set; }
    public float tier { get; private set; }
    public ResearchTech(string id, string name, int cost, float time, int tier)
    {
        this.id = id;
        this.name = name;
        this.cost = cost;
        this.time = time;
        this.tier = tier;
    }
}
public class ResearchServer : MonoBehaviour, IBaseInitalizer
{
    private List<ResearchTech> researchTech = new List<ResearchTech>();
    private List<ResearchTech> fabricationTech = new List<ResearchTech>();
    private List<ResearchTech> tfaTech = new List<ResearchTech>();

    private List<ResearchTech> unlockedTechs = new List<ResearchTech>();

    public ResearchTech currentResearchTech { get; private set; }
    public ResearchTech currentFabricationTech { get; private set; }
    public ResearchTech currentTFATech { get; private set; }

    public static ResearchServer instance;

    private float lastProcess;
    public int researchPoints { get; private set; }
    public void Initalize()
    {
        if (instance == null)
            instance = this;


        researchTech.Add(new ResearchTech("testResearch", "20% Bungalo", 2, 5, 1));
        fabricationTech.Add(new ResearchTech("testResearch", "20% Fabolo", 2, 5, 1));
        tfaTech.Add(new ResearchTech("testResearch", "20% Tfaolo", 2, 5, 1));

        PickNewTech(0);
        PickNewTech(1);
        PickNewTech(2);
    }

    public void PickNewTech(int department)
    {
        switch (department)
        {
            case 0:
                currentResearchTech = researchTech[Random.Range(0, researchTech.Count-1)];
                break;
            case 1:
                currentFabricationTech = fabricationTech[Random.Range(0, fabricationTech.Count - 1)];
                break;
            case 2:
                currentTFATech = tfaTech[Random.Range(0, tfaTech.Count - 1)];
                break;
        }
    }

    public void CommitDisks(ItemDesc[] items)
    {
        foreach (ItemDesc item in items)
        {
            item.transform.parent = transform;
        }
    }

    private void Update()
    {
        if (transform.childCount > 0)
        {
            if (Time.time - lastProcess > 10)
            {
                GameObject procces = transform.GetChild(0).gameObject;

                ResearchPoint point;
                TechDisk disk;
                if (procces.TryGetComponent(out point))
                {
                    researchPoints += point.pointValue;
                }
                if (procces.TryGetComponent(out disk))
                {
                    unlockedTechs.Add(disk.tech);
                }

                Destroy(procces);
                lastProcess = Time.time;
            }
        }
    }

}
