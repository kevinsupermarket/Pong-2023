using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb;
    public TrailRenderer ballTrail;
    public LineRenderer ballSpikeDirLine;
    public Sprite ballNeutral, ballHome, ballAway;

    public Vector2 lastVelocity;

    public float moveSpeed;

    public bool isSpiked;
    public bool wasSpikedAboveScoreLine;

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
            GetComponent<SpriteRenderer>().sprite = ballNeutral;
        }
        if (ownedBy == 0)
        {
            GetComponent<SpriteRenderer>().sprite = ballHome;
        }
        if (ownedBy == 1)
        {
            GetComponent<SpriteRenderer>().sprite = ballAway;
        }
    }

    public IEnumerator SetSpikeDirLinePos(float hitForceX, float hitForceY)
    {
        ballSpikeDirLine.positionCount = 2;
        ballSpikeDirLine.SetPosition(0, transform.position);
        ballSpikeDirLine.SetPosition(1, new Vector2(transform.position.x + hitForceX, transform.position.y + hitForceY));

        yield return new WaitUntil(() => rb.constraints == ~RigidbodyConstraints2D.FreezePosition);

        ballSpikeDirLine.positionCount = 0;

        yield break;
    }


    /// <summary>
    /// Carl's awesome ball bounce code
    /// </summary>
    /// <returns></returns>
/*    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Wall>())
        {
            if (collision.gameObject.GetComponent<Wall>().isFloor)
            {
                rb.velocity = new Vector2(lastVelocity.x, -lastVelocity.y * 0.9f);
            }
            else
            {
                rb.velocity = new Vector2(-lastVelocity.x, lastVelocity.y * 0.9f);
            }
        }
    }*/

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

        // move towards loser of last point
        if (Goal.Instance.lastTeamToScore == 0)
        {
            rb.velocity = Vector2.right * moveSpeed;
        }
        else if (Goal.Instance.lastTeamToScore == 1)
        {
            rb.velocity = Vector2.left * moveSpeed;
        }

        // reset score & ownership states
        isScored = false;
        isSpiked = false;
        wasSpikedAboveScoreLine = false;
        ownedBy = -1;

        yield break;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // reset ball state to neutral ownership when colliding with floor
        if (collision.gameObject.GetComponent<Wall>() && collision.gameObject.GetComponent<Wall>().isFloor)
        {
            ownedBy = -1;
        }
    }
}