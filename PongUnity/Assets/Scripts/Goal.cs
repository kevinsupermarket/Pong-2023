using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            }

            if (playerAssignedTo == 1)
            {
                GameManager.Instance.p2Score += 1;
            }

            StartCoroutine(collision.gameObject.GetComponent<Ball>().ResetPosition());
            foreach (Paddle obj in FindObjectsOfType<Paddle>())
            {
                StartCoroutine(obj.ResetPosition());
            }
        }
    }

    /* invisible goals
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>())
        {
            if (playerAssignedTo == 0)
            {
                GameManager.Instance.p1Score += 1;
            }

            if (playerAssignedTo == 1)
            {
                GameManager.Instance.p2Score += 1;
            }

            StartCoroutine(collision.gameObject.GetComponent<Ball>().ResetPosition());
            foreach (Paddle obj in FindObjectsOfType<Paddle>())
            {
                StartCoroutine(obj.ResetPosition());
            }
        }
    }*/
}
