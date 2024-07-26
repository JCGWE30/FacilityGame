using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyLogUI : MonoBehaviour, IBaseInitalizer
{
    public static DailyLogUI instance;

    //UI Elements
    private GameObject diskList;
    private TMP_Text title;
    private TMP_Text diskCount;
    private TMP_Text pointCount;
    private TMP_Text techCount;
    private TMP_Text commitText;
    private Button commitButton;

    public void Initalize()
    {
        if (instance == null)
            instance = this;

        diskList = transform.Find("DiskList").gameObject;
        title = transform.Find("TitleText").GetComponent<TMP_Text>();
        diskCount = transform.Find("DiskCount").GetComponent<TMP_Text>();
        pointCount = transform.Find("NewPoints").GetComponent<TMP_Text>();
        techCount = transform.Find("NewTechs").GetComponent<TMP_Text>();
        commitButton = transform.Find("CommitButton").GetComponent<Button>();
        commitText = transform.Find("CommitButton/CommitText").GetComponent<TMP_Text>();

        commitButton.onClick.AddListener(delegate { CommitClick(); });
    }

    private void CommitClick()
    {
        if (DailyLog.instance.GetCommitTimer() == 0)
        {
            DailyLog.instance.Commit();
            Close();
        }
    }

    private void Update()
    {
        float time = DailyLog.instance.GetCommitTimer();

        if (time == 0)
        {
            commitText.text = "Commit Disks";
        }
        else
        {
            int minutes = (int) time / 60;
            int seconds = (int)time % 60;
            commitText.text = $"Cooldown ({minutes:D2}:{seconds:D2})";
        }
    }

    public void Open()
    {
        int pointamount = 0;
        gameObject.SetActive(true);
        InputManager.instance.MenuOpen();
        InputManager.instance.setControls(InputManager.ControlType.DailyLog);

        foreach (var item in DailyLog.instance.GetComponentsInChildren<ResearchPoint>())
        {
            pointamount += item.pointValue;
        }
        pointCount.text = "Point Value: " + pointamount;
        techCount.text = "New Techs: " + DailyLog.instance.GetComponentsInChildren<TechDisk>().Length;
        diskCount.text = DailyLog.instance.transform.childCount + "/10 disks";
        foreach (Transform item in DailyLog.instance.transform)
        {
            GameObject disk = new GameObject("Disk");
            disk.transform.parent = diskList.transform;
            disk.AddComponent<Image>().color = new Color32(255,255,255,30);
            Image previewImage = new GameObject("DiskImage").AddComponent<Image>();

            previewImage.transform.parent = disk.transform;
            previewImage.rectTransform.position = new Vector2(-201, 0);
            previewImage.sprite = item.GetComponent<ItemDesc>().sprite;

            TMP_Text previewText = new GameObject("DiskText").AddComponent<TextMeshProUGUI>();
            previewText.transform.parent = disk.transform;
            previewText.rectTransform.position = new Vector2(51, 0);
            previewText.rectTransform.sizeDelta = new Vector2(400, 100);
            previewText.alignment = TextAlignmentOptions.Center;
            previewText.text = item.GetComponent<ItemDesc>().displayName;
            previewText.fontSize = 30;
        }
    }

    public void Close()
    {
        foreach (Transform item in diskList.transform)
        {
            Destroy(item.gameObject);
        }
        gameObject.SetActive(false);
        InputManager.instance.MenuClose();
        InputManager.instance.setControls(InputManager.ControlType.Basic);
    }

    
}
