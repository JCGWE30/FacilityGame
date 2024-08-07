using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchTest
{
    public string name { get; private set; }
    public string shortName { get; private set; }
    public int successRate { get; private set; }
    public int materialCost { get; private set; }
    public int pointMin { get; private set; }
    public int pointMax { get; private set; }
    public float time { get; private set; }

    public ResearchTest(string name, string shortName, int successRate, int materialCost, int pointMin, int pointMax, float time)
    {
        this.name = name;
        this.shortName = shortName;
        this.successRate = successRate;
        this.materialCost = materialCost;
        this.pointMin = pointMin;
        this.pointMax = pointMax;
        this.time = time;
    }
}

public class ResearchTable : Interactable
{
    public List<ResearchTest> tests = new List<ResearchTest>();

    public GameObject techDisk;

    private ResearchPoint printedDisk;

    public ResearchTest selectedTest { get; private set; }
    public float startTime;

    //0 - no test selected
    //1 - still testing
    //2 - test failed
    //3 - test succeded
    public int testState { get; private set; } = 0;
    public ItemDesc claimableDisk { get; private set; }
    void Awake()
    {
        tests = new List<ResearchTest>(new[] { 
        new ResearchTest("Test test","Test",100,0,5,20,10),
        new ResearchTest("Risky Test","Risky",10,0,5,5000,10)
        });
    }

    private void Update()
    {
        if (testState == 1)
        {
            float timepassed = Time.time - startTime;
            if (timepassed > selectedTest.time)
            {
                float checknumber = (float) selectedTest.successRate/100;
                float val = Random.value;
                if (checknumber >= val)
                {
                    testState = 2;
                    claimableDisk = Instantiate(techDisk).GetComponent<ItemDesc>();
                    printedDisk = claimableDisk.GetComponent<ResearchPoint>();
                    printedDisk.testName = selectedTest.shortName;
                    printedDisk.pointValue = Random.Range(selectedTest.pointMin, selectedTest.pointMax + 1);
                }
                else
                {
                    testState = 3;
                }
            }
        }
    }

    public void ContinueFromTest()
    {
        if (testState == 2)
        {
            if (printedDisk.desc.checker.TryInsert(EquipmentContainer.instance.GetEquipmentItem(SlotType.Hand)))
            {
                testState = 0;
                selectedTest = null;
            }
        }
        else
        {
            testState = 0;
            selectedTest = null;
        }
    }

    public void StartTest(ResearchTest test)
    {
        EquipmentContainer.instance.GetEquipmentItem(SlotType.Hand).item.GetComponent<StackableChecker>().amount -= test.materialCost;
        testState = 1;
        startTime = Time.time;
        selectedTest = test;
    }
    protected override void Interact(bool alt)
    {
        ResearchTableUI.instance.Open(this); ;
    }
}
