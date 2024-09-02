using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SightingCooldown : MonoBehaviour
{
    public static SightingCooldown instance;

    public static bool canHold { get { return instance.endtime < Time.time; } }

    [SerializeField]
    private GameObject cooldown;
    [SerializeField]
    private Image cooldownBar;

    private float endtime;
    private float starttime;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        cooldown.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (endtime < Time.time)
        {
            gameObject.SetActive(false);
        }
        else
        {
            float percent = (Time.time-starttime) / (endtime-starttime);

            if (percent < 1)
                cooldownBar.fillAmount = percent;

        }
    }

    public void StartCooldown(float endTime)
    {
        starttime = Time.time;
        endtime = endTime;
        gameObject.SetActive(true);
    }
}
