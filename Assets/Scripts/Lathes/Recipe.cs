using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Recipe : MonoBehaviour
{
    public int woodCost;
    public int plasticCost;
    public int ironCost;
    public int resultAmount;

    public ItemDesc result;
    public float craftTime;
    private GameObject infoPanel;
    void Start()
    {
        infoPanel = transform.parent.parent.Find("InfoPanel").gameObject;

        if (woodCost == 0)
        {
            transform.Find("Cost/Wood").gameObject.SetActive(false);
        }
        else
        {
            transform.Find("Cost/Wood/Count").gameObject.GetComponent<TMP_Text>().text = woodCost + "";
        }

        if (plasticCost == 0)
        {
            transform.Find("Cost/Plastic").gameObject.SetActive(false);
        }
        else
        {
            transform.Find("Cost/Plastic/Count").gameObject.GetComponent<TMP_Text>().text = plasticCost + "";
        }

        if (ironCost == 0)
        {
            transform.Find("Cost/Iron").gameObject.SetActive(false);
        }
        else
        {
            transform.Find("Cost/Iron/Count").gameObject.GetComponent<TMP_Text>().text = ironCost + "";
        }

        transform.Find("Result").GetComponent<Image>().sprite = result.sprite;
        transform.Find("Result/Count").GetComponent<TMP_Text>().text = resultAmount + "";

        transform.Find("ButtonBlanket").GetComponent<Button>().onClick.AddListener(delegate { Click(); });
    }

    private void Click()
    {
        transform.parent.GetComponentInParent<LatheUIManager>().SetRecipe(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
