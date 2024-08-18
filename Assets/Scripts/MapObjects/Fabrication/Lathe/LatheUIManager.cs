using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LatheUIManager : MonoBehaviour, IInatalizer
{
    [SerializeField]
    private GameObject recipePrefab;

    public static LatheUIManager instance;

    private LatheManager manager;
    public InputManager playerInput;
    private LatheRecipe selectedRecipe;
    private bool shouldFabricate { get { return manager?.isCrafting.Value ?? false;} }
    private bool fabricationMode = false;

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


    public void Initalize()
    {
        if (instance == null)
            instance = this;

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
        manager.CraftItem(selectedRecipe);
    }

    private void Update()
    {
        if (manager == null)
            return;

        ironCount.text = manager.iron + "";
        plasticCount.text = manager.plastic + "";
        woodCount.text = manager.wood + "";

        if (fabricationMode != shouldFabricate)
        {
            if (shouldFabricate)
            {
                fabricatingPanel.SetActive(true);
                fabricatingImage.GetComponent<Image>().sprite = ItemFinder.Find(manager.currentRecipe.resultItem).sprite;
                infoPanel.SetActive(false);
            }
            else
            {
                fabricatingPanel.SetActive(false);
                infoPanel.SetActive(selectedRecipe!=null);
            }
            fabricationMode = shouldFabricate;
        }

        if(fabricationMode)
        {
            float timeLeft = manager.craftingFinish.Value-NetworkManager.Singleton.ServerTime.TimeAsFloat;
            float percentage = timeLeft/manager.currentRecipe.craftTime;
            fabricatingImage.GetComponent<Image>().fillAmount = 1-percentage;

            if (percentage <= 0)
            {
                selectedRecipe = null;
                fabricatingPanel.SetActive(false);
            }
        }
    }

    public void Open(LatheManager lathe)
    {
        foreach(LatheRecipe recipe in lathe.recipes)
        {
            LatheUIObject newRecipe = Instantiate(recipePrefab).GetComponent<LatheUIObject>();

            newRecipe.transform.parent = transform.Find("Recipes");


            newRecipe.ironCost.text = recipe.ironCost.ToString();
            newRecipe.ironImage.gameObject.SetActive(recipe.ironCost > 0);

            newRecipe.plasticCost.text = recipe.plasticCost.ToString();
            newRecipe.plasticImage.gameObject.SetActive(recipe.plasticCost > 0);

            newRecipe.woodCost.text = recipe.woodCost.ToString();
            newRecipe.woodImage.gameObject.SetActive(recipe.woodCost > 0);

            ItemDesc item = ItemFinder.Find(recipe.resultItem);
            newRecipe.result.sprite = item.sprite;
            newRecipe.resultAmount.text = recipe.resultAmount.ToString();


            newRecipe.selectButton.onClick.AddListener(() => { SetRecipe(recipe); });
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
        selectedRecipe = null;
        infoPanel.SetActive(false);
        playerInput.MenuClose();
        playerInput.setControls(InputManager.ControlType.Basic);
        gameObject.SetActive(false);
        manager = null;
    }

    public void SetRecipe(LatheRecipe recipe)
    {
        if (fabricationMode)
            return;
        if (recipe == null)
        {
            selectedRecipe = null;
            infoPanel.gameObject.SetActive(false);
        }
        else
        {
            ItemDesc resultItem = ItemFinder.Find(recipe.resultItem);
            selectedRecipe = recipe;
            infoImage.sprite = resultItem.sprite;
            infoText.text = resultItem.displayName + " x" + selectedRecipe.resultAmount;
            infoPanel.SetActive(true);
        }
    }
}
