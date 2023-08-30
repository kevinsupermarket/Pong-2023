using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class Goal : MonoBehaviour
{
    public int playerAssignedTo;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>() && !collision.gameObject.GetComponent<Ball>().isScored)
        {
            collision.gameObject.GetComponent<Ball>().isScored = true;

            if (playerAssignedTo == 0)
            {
                GameManager.Instance.p1Score += 1;

                if (GameManager.Instance.p1Score == GameManager.Instance.maxScore)
                {
                    GameManager.Instance.gameOver = true;
                }
            }

            if (playerAssignedTo == 1)
            {
                GameManager.Instance.p2Score += 1;

                if (GameManager.Instance.p2Score == GameManager.Instance.maxScore)
                {
                    GameManager.Instance.gameOver = true;
                }
            }

            if (!GameManager.Instance.gameOver)
            {
                StartCoroutine(collision.gameObject.GetComponent<Ball>().ResetPosition());

                foreach (Paddle obj in FindObjectsOfType<Paddle>())
                {
                    StartCoroutine(obj.ResetPosition());
                }

                foreach (Player obj in FindObjectsOfType<Player>())
                {
                    StartCoroutine(obj.ResetPosition());
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>() && !collision.gameObject.GetComponent<Ball>().isScored)
        {
            collision.gameObject.GetComponent<Ball>().isScored = true;

            if (playerAssignedTo == 0)
            {
                GameManager.Instance.p1Score += 1;

                if (GameManager.Instance.p1Score == GameManager.Instance.maxScore)
                {
                    GameManager.Instance.gameOver = true;
                }
            }

            if (playerAssignedTo == 1)
            {
                GameManager.Instance.p2Score += 1;

                if (GameManager.Instance.p2Score == GameManager.Instance.maxScore)
                {
                    GameManager.Instance.gameOver = true;
                }
            }

            if (!GameManager.Instance.gameOver)
            {
                StartCoroutine(collision.gameObject.GetComponent<Ball>().ResetPosition());

                foreach (Paddle obj in FindObjectsOfType<Paddle>())
                {
                    StartCoroutine(obj.ResetPosition());
                }

                foreach (Player obj in FindObjectsOfType<Player>())
                {
                    StartCoroutine(obj.ResetPosition());
                }
            }
        }
    }
}
