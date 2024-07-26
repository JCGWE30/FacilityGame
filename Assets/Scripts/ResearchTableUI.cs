using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchTableUI : MonoBehaviour, IBaseInitalizer
{
    public static ResearchTableUI instance;

    private ResearchTable table;

    private ResearchTest selectedTest;

    //UI Elements
    private GameObject standardUI;
    private GameObject testingUI;

    private GameObject testingProcedures;
    private GameObject activeProcedure;

    private TMP_Text testTitle;
    private TMP_Text success;
    private TMP_Text cost;
    private TMP_Text yield;
    private TMP_Text time;
    private TMP_Text startText;
    private Button startButton;

    private TMP_Text testStatus;
    private Image progressImage;
    private TMP_Text clearText;
    private Button clearButton;

    public ResearchTableUI()
    {
    }
    public void Initalize()
    {
        if (instance == null)
            instance = this;

        standardUI = transform.Find("StandardUI").gameObject;
        testingUI = transform.Find("TestingUI").gameObject;

        testingProcedures = transform.Find("StandardUI/TestingProcedures").gameObject;
        activeProcedure = transform.Find("StandardUI/ActiveProcedure").gameObject;

        testTitle = transform.Find("StandardUI/ActiveProcedure/TestTitle").GetComponent<TMP_Text>();
        success = transform.Find("StandardUI/ActiveProcedure/Success").GetComponent<TMP_Text>();
        cost = transform.Find("StandardUI/ActiveProcedure/Cost").GetComponent<TMP_Text>();
        yield = transform.Find("StandardUI/ActiveProcedure/Yield").GetComponent<TMP_Text>();
        time = transform.Find("StandardUI/ActiveProcedure/Time").GetComponent<TMP_Text>();
        startText = transform.Find("StandardUI/ActiveProcedure/Start/Text").GetComponent<TMP_Text>();
        startButton = transform.Find("StandardUI/ActiveProcedure/Start").GetComponent<Button>();

        testStatus = transform.Find("TestingUI/Status").GetComponent<TMP_Text>();
        progressImage = transform.Find("TestingUI/ProgressImage").GetComponent<Image>();
        clearText = transform.Find("TestingUI/ClearButton/ButtonText").GetComponent<TMP_Text>();
        clearButton = transform.Find("TestingUI/ClearButton").GetComponent<Button>();

        startButton.onClick.AddListener(delegate { StartTest(); });
        clearButton.onClick.AddListener(delegate { ClearClick(); });
        activeProcedure.SetActive(false);
    }

    private void StartTest()
    {
        if (canStart(selectedTest))
            table.StartTest(selectedTest);
    }

    private void ClearClick()
    {
        table.ContinueFromTest();
    }

    private void Update()
    {
        if (table == null)
            return;
        testingUI.SetActive(table.testState != 0);
        standardUI.SetActive(table.testState == 0);
        switch (table.testState)
        {
            case 0:
                if (selectedTest == null)
                {
                    activeProcedure.SetActive(false);
                }
                else
                {
                    activeProcedure.SetActive(true);
                    testTitle.text = selectedTest.name;
                    success.text = "Success Rate: " + selectedTest.successRate + "%";
                    cost.text = "Material Cost: " + selectedTest.materialCost;
                    yield.text = "Point Yield: " + selectedTest.pointMin + "-" + selectedTest.pointMax;
                    time.text = "Test time: " + fancyTimeText(selectedTest.time);

                    if (canStart(selectedTest))
                        startText.text = "Start Test";
                    else
                        startText.text = "Not Enough Material";
                }
                break;
            case 1:
                testStatus.text = "Testing In Progress";
                progressImage.gameObject.SetActive(true);
                progressImage.fillAmount = (Time.time - table.startTime) / table.selectedTest.time;
                clearButton.gameObject.SetActive(false);
                break;
            case 2:
                testStatus.text = "Test Complete!";
                progressImage.gameObject.SetActive(false);
                clearText.text = "Take Results Disk";
                clearButton.gameObject.SetActive(true);
                break;
            case 3:
                testStatus.text = "Test Failed";
                progressImage.gameObject.SetActive(false);
                clearText.text = "Continue to tests";
                clearButton.gameObject.SetActive(true);
                break;

        }
        
    }

    private string fancyTimeText(float time)
    {
        int seconds = (int) time % 60;
        int minutes = (int)time / 60;

        if (minutes==0)
            return $"{seconds} seconds";
        else if(seconds==0)
            return $"{minutes} minutes";
        else
            return $"{minutes} minutes {seconds} seconds";
    }

    private bool canStart(ResearchTest test)
    {
        ItemDesc item = EquipmentContainer.instance.GetEquipmentItem(SlotType.Hand).item;
        if (item == null)
            return false;
        if (item.id == "anomolusMaterial")
            return item.GetComponent<StackableChecker>().amount >= test.materialCost;
        return false;
    }

    public void Open(ResearchTable table)
    {
        gameObject.SetActive(true);
        this.table = table;
        InputManager.instance.MenuOpen();
        InputManager.instance.setControls(InputManager.ControlType.ResearchTable);

        foreach (var item in table.tests)
        {
            GameObject rest = new GameObject("Rest");
            rest.transform.parent = testingProcedures.transform;
            rest.AddComponent<Image>().rectTransform.sizeDelta = new Vector2(500,100);
            rest.AddComponent<Button>().onClick.AddListener(delegate { SelectTest(item); });

            TMP_Text text = new GameObject("Text").AddComponent<TextMeshProUGUI>();
            text.color = Color.black;
            text.transform.parent = rest.transform;
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 30;
            text.text = item.name;
        }
    }

    private void SelectTest(ResearchTest test)
    {
        selectedTest = test;
    }

    public void Close()
    {
        selectedTest = null;
        foreach(Transform item in testingProcedures.transform)
        {
            Destroy(item.gameObject);
        }
        gameObject.SetActive(false);
        InputManager.instance.MenuClose();
        InputManager.instance.setControls(InputManager.ControlType.Basic);
    }
}

