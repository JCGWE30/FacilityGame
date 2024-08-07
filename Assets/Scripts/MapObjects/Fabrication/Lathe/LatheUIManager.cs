using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LatheUIManager : MonoBehaviour
{
    private LatheManager manager;
    public InputManager playerInput;
    private Recipe selectedRecipe;
    private bool fabricating;
    private float fabricateStart;

    //UI Elements
    private Button infoButton;
    private GameObject fabricatingPanel;
    private Image fabricatingImage;

    private GameObject infoPanel;
    private TMP_Text infoText;
    private Image infoImage;

    private TMP_Text ironCount;
    private TMP_Text plasticCount;
    private TMP_Text woodCount;


    private void Awake()
    {
        infoButton = transform.Find("InfoPanel/Button").GetComponent<Button>();
        fabricatingPanel = transform.Find("Fabricating").gameObject;
        fabricatingImage = transform.Find("Fabricating/Image").GetComponent<Image>();

        infoPanel = transform.Find("InfoPanel").gameObject;
        infoText = transform.Find("InfoPanel/InfoText").GetComponent<TMP_Text>();
        infoImage = transform.Find("InfoPanel/ImagePanel/Image").GetComponent<Image>();

        ironCount = transform.Find("ResourceCount/Iron/Count").GetComponent<TMP_Text>();
        plasticCount = transform.Find("ResourceCount/Plastic/Count").GetComponent<TMP_Text>();
        woodCount = transform.Find("ResourceCount/Wood/Count").GetComponent<TMP_Text>();


        infoButton.onClick.AddListener(delegate { Fabricate(); });
    }

    private void Fabricate()
    {
        if (selectedRecipe == null)
            return;
        if (fabricating)
            return;
        if (!manager.CraftItem(selectedRecipe))
            return;
        fabricateStart = Time.time;
        fabricating = true;

        fabricatingPanel.SetActive(true);
        fabricatingImage.GetComponent<Image>().sprite = selectedRecipe.result.sprite;
        infoPanel.SetActive(false);
    }

    private void Update()
    {
        if (manager != null)
        {
            ironCount.text = manager.iron + "";
            plasticCount.text = manager.plastic + "";
            woodCount.text = manager.wood + "";
        }
        if (fabricating)
        {
            float timeElapsed = Time.time - fabricateStart;
            float percentage = timeElapsed/selectedRecipe.craftTime;
            fabricatingImage.GetComponent<Image>().fillAmount = percentage;

            if (percentage >= 1)
            {
                selectedRecipe = null;
                fabricating = false;
                fabricatingPanel.SetActive(false);
            }
        }
    }

    public void Open(LatheManager lathe)
    {
        foreach(Transform item in lathe.transform.Find("Recipes"))
        {
            GameObject newRecipe = Instantiate(item.gameObject);
            newRecipe.transform.parent = transform.Find("Recipes");
            newRecipe.SetActive(true);
        }
        playerInput.MenuOpen();
        playerInput.setControls(InputManager.ControlType.Lathe);
        manager = lathe;
        gameObject.SetActive(true);
    }
    public void Close()
    {
        foreach (Transform item in transform.Find("Recipes"))
        {
            Destroy(item.gameObject);
        }
        if (!fabricating)
            selectedRecipe = null;
        infoPanel.SetActive(false);
        playerInput.MenuClose();
        playerInput.setControls(InputManager.ControlType.Basic);
        gameObject.SetActive(false);
        manager = null;
    }

    public void SetRecipe(Recipe recipe)
    {
        if (fabricating)
            return;
        if (recipe == null)
        {
            selectedRecipe = null;
            infoPanel.gameObject.SetActive(false);
        }
        else
        {
            selectedRecipe = recipe;
            infoImage.sprite = selectedRecipe.result.sprite;
            infoText.text = selectedRecipe.result.displayName + " x" + selectedRecipe.resultAmount;
            infoPanel.SetActive(true);
        }
    }
}
