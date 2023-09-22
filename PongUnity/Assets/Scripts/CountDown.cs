using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountDown : MonoBehaviour
{
    float currentTime = 0f;
    float startingTime = 5f;

    public TMP_Text countdownText;

    void Start()
    {
        currentTime = startingTime;
    }

    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        countdownText.text = currentTime.ToString("0");    

        if(currentTime <= 0)
        {
            currentTime = 0;
        }
    }
}
