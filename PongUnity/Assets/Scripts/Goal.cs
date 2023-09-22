using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class Goal : MonoBehaviour
{
    Ball ball;
    ScoreLine scoreLine;

    private void Start()
    {
        scoreLine = FindObjectOfType<ScoreLine>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>())
        {
            // setting this variable because every if-statement will be a mile long otherwise
            ball = collision.gameObject.GetComponent<Ball>();


            // declare the ball as "scored" if the ball is owned by a team, was spiked, and has collided with this object
            if (ball && !ball.isScored && ball.ownedBy != -1 && ball.isSpiked)
            {
                ball.isScored = true;

                if (ball.ownedBy == 0)
                {
                    GameManager.Instance.homeScore += 1;

                    if (GameManager.Instance.homeScore == GameManager.Instance.maxScore)
                    {
                        GameManager.Instance.gameOver = true;
                    }
                }

                if (ball.ownedBy == 1)
                {
                    GameManager.Instance.awayScore += 1;

                    if (GameManager.Instance.awayScore == GameManager.Instance.maxScore)
                    {
                        GameManager.Instance.gameOver = true;
                    }
                }

                if (!GameManager.Instance.gameOver)
                {
                    StartCoroutine(ball.ResetPosition());

                    foreach (Player obj in FindObjectsOfType<Player>())
                    {
                        StartCoroutine(obj.ResetPosition());
                    }
                }
            }

            // give point to team Y if the ball is owned by team X, was NOT spiked (or was spiked below the score line), and has collided with this object
            else if (ball && !ball.isScored && ball.ownedBy != -1 && (!ball.isSpiked || ball.transform.position.y < scoreLine.transform.position.y))
            {
                ball.isScored = true;

                if (ball.ownedBy == 0)
                {
                    GameManager.Instance.awayScore += 1;

                    if (GameManager.Instance.awayScore == GameManager.Instance.maxScore)
                    {
                        GameManager.Instance.gameOver = true;
                    }
                }

                if (ball.ownedBy == 1)
                {
                    GameManager.Instance.homeScore += 1;

                    if (GameManager.Instance.homeScore == GameManager.Instance.maxScore)
                    {
                        GameManager.Instance.gameOver = true;
                    }
                }

                if (!GameManager.Instance.gameOver)
                {
                    StartCoroutine(ball.ResetPosition());

                    foreach (Player obj in FindObjectsOfType<Player>())
                    {
                        StartCoroutine(obj.ResetPosition());
                    }
                }
            }
        }
    }
}
