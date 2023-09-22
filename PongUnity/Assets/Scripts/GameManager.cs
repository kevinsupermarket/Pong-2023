using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using TMPro;
using UnityEditor.SearchService;

public class GameManager : MonoBehaviour
{
    public KeyCode startKey;

    public TMP_Text homeScoreText;
    public TMP_Text awayScoreText;
    public TMP_Text winText;

    Player[] players;
    Ball ball;

    public GameObject scoreLine;

    public int homeScore, awayScore, maxScore;
    public bool gameOver;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        players = FindObjectsOfType<Player>();
        ball = FindObjectOfType<Ball>();
        scoreLine = FindObjectOfType<ScoreLine>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        homeScoreText.text = "" + homeScore;
        awayScoreText.text = "" + awayScore;

        if (homeScore == maxScore)
        {
            winText.text = "HOME TEAM IS\nWINNER\n\nPRESS " + startKey + " TO\nRESTART";
        }
        else if (awayScore == maxScore)
        {
            winText.text = "AWAY TEAM IS\nWINNER\n\nPRESS " + startKey + " TO\nRESTART";
        }

        if (gameOver && Input.GetKeyDown(startKey))
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        homeScore = 0;
        awayScore = 0;
        homeScoreText.text = "" + homeScore;
        awayScoreText.text = "" + awayScore;
        winText.text = "";

        ball.StartCoroutine(ball.ResetPosition());
        for (var i = 0; i < players.Length; i++)
        {
            players[i].StartCoroutine(players[i].ResetPosition());
        }

        gameOver = false;
    }
}
