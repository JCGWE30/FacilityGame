using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TechComputerUI : MonoBehaviour, IBaseInitalizer
{
    public static TechComputerUI instance;

    //UI Elements
    private GameObject standardUI;
    private GameObject researchUI;

    private Button researchTech;
    private Button fabricationTech;
    private Button tfaTech;

    private TMP_Text researchText;
    private TMP_Text fabricationText;
    private TMP_Text tfaText;

    private TMP_Text totalPoints;

    private GameObject techInfo;

    private TMP_Text infoText;
    private TMP_Text pointCost;
    private TMP_Text unlockTime;
    private TMP_Text unlockText;
    private Button unlockButton;

    private TMP_Text researchInfoText;
    private Image researchInfoImage;
    private Button researchInfoButton;

    private ResearchTech selectedTech;
    public void Initalize()
    {
        if (instance == null)
            instance = this;
        standardUI = transform.Find("StandardUI").gameObject;
        researchUI = transform.Find("ResearchUI").gameObject;

        researchTech = transform.Find("StandardUI/ResearchTech").GetComponent<Button>();
        fabricationTech = transform.Find("StandardUI/FabricationTech").GetComponent<Button>();
        tfaTech = transform.Find("StandardUI/TFATech").GetComponent<Button>();

        researchText = transform.Find("StandardUI/ResearchTech/Text").GetComponent<TMP_Text>();
        fabricationText = transform.Find("StandardUI/FabricationTech/Text").GetComponent<TMP_Text>();
        tfaText = transform.Find("StandardUI/TFATech/Text").GetComponent<TMP_Text>();

        totalPoints = transform.Find("StandardUI/PointCount").GetComponent<TMP_Text>();

        techInfo = transform.Find("StandardUI/TechInfo").gameObject;


        infoText = transform.Find("StandardUI/TechInfo/TechText").GetComponent<TMP_Text>();
        pointCost = transform.Find("StandardUI/TechInfo/PointCost").GetComponent<TMP_Text>();
        unlockTime = transform.Find("StandardUI/TechInfo/UnlockTime").GetComponent<TMP_Text>();
        unlockText = transform.Find("StandardUI/TechInfo/UnlockButton/Text").GetComponent<TMP_Text>();
        unlockButton = transform.Find("StandardUI/TechInfo/UnlockButton").GetComponent<Button>();

        researchInfoText = transform.Find("ResearchUI/Text").GetComponent<TMP_Text>();
        researchInfoImage = transform.Find("ResearchUI/Image").GetComponent<Image>();
        researchInfoButton = transform.Find("ResearchUI/Button").GetComponent<Button>();

        researchInfoButton.onClick.AddListener(delegate { ClaimDisk(); });
        researchTech.onClick.AddListener(delegate { SelectTech(0); });
        fabricationTech.onClick.AddListener(delegate { SelectTech(1); });
        tfaTech.onClick.AddListener(delegate { SelectTech(2); });
        unlockButton.onClick.AddListener(delegate { Unlock(); });
    }

    private void Update()
    {
        standardUI.SetActive(TechComputer.instance.computerState == 0);
        researchUI.SetActive(TechComputer.instance.computerState != 0);
        if (TechComputer.instance.computerState == 0)
        {
            totalPoints.text = ResearchServer.instance.researchPoints + " Points";
            if (selectedTech == null)
            {
                techInfo.SetActive(false);
            }
            else
            {
                techInfo.SetActive(true);
                infoText.text = selectedTech.name;
                pointCost.text = selectedTech.cost + " Points";
                int minutes = (int)selectedTech.time / 60;
                int seconds = (int)selectedTech.time % 60;
                unlockTime.text = $"Unlock Time: {minutes:D2}:{seconds:D2}";

                if (ResearchServer.instance.researchPoints >= selectedTech.cost)
                {
                    unlockText.text = "Unlock";
                }
                else
                {
                    unlockText.text = "Not Enough Points";
                }
            }
        }
        else
        {
            if (TechComputer.instance.computerState == 1)
            {
                researchInfoText.text = "Researching " + selectedTech.name;
                researchInfoImage.gameObject.SetActive(true);
                researchInfoButton.gameObject.SetActive(false);
                researchInfoImage.fillAmount = Mathf.Min(1,(Time.time - TechComputer.instance.startTime)/selectedTech.time);
            }
            else
            {
                researchInfoText.text = "Research Complete! Remove disk";
                researchInfoImage.gameObject.SetActive(false);
                researchInfoButton.gameObject.SetActive(true);
            }
        }
    }

    private void ClaimDisk()
    {
        if (TechComputer.instance.computerState == 2)
        {
            if (TechComputer.instance.claimableDisk.GetComponent<MovementChecker>().TryInsert(EquipmentContainer.instance.GetEquipmentItem(SlotType.Hand)))
                TechComputer.instance.ClearDisk();
        }
    }

    private void Unlock()
    {
        if (ResearchServer.instance.researchPoints >= selectedTech.cost)
            TechComputer.instance.StartResearching(selectedTech);
    }

    private void SelectTech(int spot)
    {
        switch (spot)
        {
            case 0:
                selectedTech = ResearchServer.instance.currentResearchTech;
                break;
            case 1:
                selectedTech = ResearchServer.instance.currentFabricationTech;
                break;
            case 2:
                selectedTech = ResearchServer.instance.currentTFATech;
                break;
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
        InputManager.instance.MenuOpen();
        InputManager.instance.setControls(InputManager.ControlType.TechComputer);

        researchText.text = ResearchServer.instance.currentResearchTech.name;
        fabricationText.text = ResearchServer.instance.currentFabricationTech.name;
        tfaText.text = ResearchServer.instance.currentTFATech.name;
    }

    public void Close()
    {
        if(TechComputer.instance.computerState==0)
            selectedTech = null;
        gameObject.SetActive(false);
        InputManager.instance.MenuClose();
        InputManager.instance.setControls(InputManager.ControlType.Basic);
    }
}
