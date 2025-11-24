using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TimerController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float countTimer = 120f;
    [SerializeField] private int minutes = 1;
    [SerializeField] private int seconds = 0;
    [SerializeField] private bool isCounting;
    [SerializeField] private float timeCounter;

    private void Update()
    {
        //if (isCounting && countTimer > 0)
        //{
        //timeCounter -= Time.deltaTime;
        //minutes = Mathf.FloorToInt(countTimer / 60f);
        //seconds = Mathf.FloorToInt(countTimer - minutes * 60f);  
        //}
        if (!isCounting)
        {
            timeCounter += Time.deltaTime;
            minutes = Mathf.FloorToInt(timeCounter / 60f);
            seconds = Mathf.FloorToInt(timeCounter - minutes * 60f);  
        }
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

    }

}
