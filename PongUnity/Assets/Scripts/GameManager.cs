using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text p1ScoreText;
    public TMP_Text p2ScoreText;
    public int p1Score, p2Score;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        p1ScoreText.text = "" + p1Score;
        p2ScoreText.text = "" + p2Score;
    }
}
