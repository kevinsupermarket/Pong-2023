using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public KeyCode startKey;

    public TMP_Text homeScoreText;
    public TMP_Text awayScoreText;
    public TMP_Text winText;

    public AudioSource pointScoredSound;

    Player[] players;
    Ball ball;

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
    }

    // Update is called once per frame
    void Update()
    {
        homeScoreText.text = "" + homeScore;
        awayScoreText.text = "" + awayScore;

        if (homeScore == maxScore)
        {
            winText.text = "HOME TEAM WINS\nPRESS " + startKey + " TO\nRESTART";
        }
        else if (awayScore == maxScore)
        {
            winText.text = "AWAY TEAM WINS\nPRESS " + startKey + " TO\nRESTART";
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
