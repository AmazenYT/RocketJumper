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

}
