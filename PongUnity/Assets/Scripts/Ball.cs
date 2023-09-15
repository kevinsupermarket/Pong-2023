using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb;
    public TrailRenderer ballTrail;

    public Vector2 lastVelocity;

    public float moveSpeed;

    public bool isSpiked;

    public bool isScored;
    public float ownedBy;

    Vector2 spawnPoint;

    public static Ball Instance;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // give ball to HOME side first
        rb.velocity = Vector2.left * moveSpeed;

        // -1 is "unowned" value, 0 is home, 1 is away
        ownedBy = -1;

        // save spawnpoint as current position of object
        spawnPoint = transform.position;
    }

    private void Update()
    {
        lastVelocity = rb.velocity;

        // change ball's color & trail color based on ownership
        if (ownedBy == -1)
        {
            GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        }
        if (ownedBy == 0)
        {
            GetComponent<SpriteRenderer>().color = new Color(0, 0, 255);
        }
        if (ownedBy == 1)
        {
            GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Wall>())
        {
            /* THIS NEEDS TO BE CHANGED!
             - cannot bounce horizontally
             - will always bounce vertically even if colliding with a horizontal wall
             - need to find a good method to detect which way to bounce
            */

            //rb.velocity = new Vector2(lastVelocity.x, -lastVelocity.y * 0.9f);
        }
    }

    public IEnumerator ResetPosition()
    {
        if (!GameManager.Instance.gameOver)
        {
            yield return new WaitForSeconds(2);
        }

        // reset at ball spawnpoint (no specific point saved currently, so just using 0,0)
        transform.position = spawnPoint;

        // resets trail position
        for (var i = 0; i < ballTrail.positionCount; i++)
        {
            ballTrail.SetPosition(i, transform.position);
        }

        // move towards winner of last point
        if (Goal.Instance.lastTeamtoScore == 0)
        {
            rb.velocity = Vector2.left * moveSpeed;
        }
        else if (Goal.Instance.lastTeamtoScore == 1)
        {
            rb.velocity = Vector2.right * moveSpeed;
        }

        // reset score & ownership states
        isScored = false;
        ownedBy = -1;

        yield break;
    }
}
