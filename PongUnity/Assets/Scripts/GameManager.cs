using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using TMPro;
using UnityEditor.SearchService;

public class GameManager : MonoBehaviour
{
    public KeyCode startKey;

    public TMP_Text p1ScoreText;
    public TMP_Text p2ScoreText;
    public TMP_Text winText;

    Player[] players;
    Ball ball;

    public int p1Score, p2Score, maxScore;
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
        p1ScoreText.text = "" + p1Score;
        p2ScoreText.text = "" + p2Score;

        if (p1Score == maxScore)
        {
            winText.text = "PLAYER 1 IS\nWINNER\n\nPRESS " + startKey + " TO\nRESTART";
        }
        else if (p2Score == maxScore)
        {
            winText.text = "PLAYER 2 IS\nWINNER\n\nPRESS " + startKey + " TO\nRESTART";
        }

        if (gameOver && Input.GetKeyDown(startKey))
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        p1Score = 0;
        p2Score = 0;
        p1ScoreText.text = "" + p1Score;
        p2ScoreText.text = "" + p2Score;
        winText.text = "";

        ball.StartCoroutine(ball.ResetPosition());
        for (var i = 0; i < players.Length; i++)
        {
            players[i].StartCoroutine(players[i].ResetPosition());
        }

        gameOver = false;
    }
}
