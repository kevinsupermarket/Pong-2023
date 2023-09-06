using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class Goal : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>() && !collision.gameObject.GetComponent<Ball>().isScored && collision.gameObject.GetComponent<Ball>().ownedBy != -1)
        {
            // declare the ball as "scored" if the ball is owned by a team and has collided with this object
            collision.gameObject.GetComponent<Ball>().isScored = true;

            if (collision.gameObject.GetComponent<Ball>().ownedBy == 0)
            {
                GameManager.Instance.homeScore += 1;

                if (GameManager.Instance.homeScore == GameManager.Instance.maxScore)
                {
                    GameManager.Instance.gameOver = true;
                }
            }

            if (collision.gameObject.GetComponent<Ball>().ownedBy == 1)
            {
                GameManager.Instance.awayScore += 1;

                if (GameManager.Instance.awayScore == GameManager.Instance.maxScore)
                {
                    GameManager.Instance.gameOver = true;
                }
            }

            if (!GameManager.Instance.gameOver)
            {
                StartCoroutine(collision.gameObject.GetComponent<Ball>().ResetPosition());

                foreach (Player obj in FindObjectsOfType<Player>())
                {
                    StartCoroutine(obj.ResetPosition());
                }
            }
        }
    }
}
