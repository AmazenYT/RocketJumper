using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TimerController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private int minutes = 1;
    [SerializeField] private int seconds = 0;
    [SerializeField] private float timeCounter;

    private void Update()
    {
        timeCounter += Time.deltaTime;
        minutes = Mathf.FloorToInt(timeCounter / 60f);
        seconds = Mathf.FloorToInt(timeCounter - minutes * 60f);  
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

    }

}
