using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isAI;

    Ball ball;

    public Rigidbody2D rb;
    public TMP_Text playerTag;

    public KeyCode upKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode smashKey;

    public Vector3 spawnPoint;

    public float moveSpeed;

    public int maxJumpCount;
    public int currentJumpCount;
    public float jumpForce;
    public bool hasJumped;
    public bool isGrounded;

    public float hitForce;

    public float courtSide;
    public float teamIdentity;

    public float ballXRange;
    public float ballYRange;


    public static Player Instance;


    void Awake()
    {
        Instance = this;
        spawnPoint = transform.position;
    }

    private void Start()
    {
        ball = FindObjectOfType<Ball>();

        courtSide = Mathf.Sign(transform.position.x);

        // set team identity so ball can be scored properly
        if (courtSide == -1)
        {
            teamIdentity = 0;
        }
        else if (courtSide == 1)
        {
            teamIdentity = 1;
        }

        currentJumpCount = maxJumpCount;

        // show tag above player if not an AI
        if (!isAI)
        {
            playerTag.text = gameObject.name;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // let this thing move itself if it's an AI
        if (isAI)
        {
            MoveAuto();
        }

        // let's move this thing (manually)
        if (!isAI && Input.GetKeyDown(upKey) && currentJumpCount > 0)
        {
            Jump();
        }

        if (isGrounded && !hasJumped)
        {
            currentJumpCount = maxJumpCount;
        }

        if (!isAI && Input.GetKey(leftKey))
        {
            MoveLeft();
        }

        if (!isAI && Input.GetKey(rightKey))
        {
            MoveRight();
        }

        // make sure player tag stays on player
        playerTag.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

    }

    public void MoveAuto()
    {
        /* THIS NEEDS UPDATING!
         - players should be able to move anywhere between the two inner walls 
        */


        if ((courtSide == -1 && transform.position.x <= courtSide) || (courtSide == 1 && transform.position.x >= courtSide))
        {
            // if ball is to the left of player, move left
            if (ball.transform.position.x < transform.position.x && ball.transform.position.x > transform.position.x - ballXRange)
            {
                MoveLeft();

                // if ball is a certain distance above player, jump
                if (ball.transform.position.y > transform.position.y && ball.transform.position.y < transform.position.y + ballYRange && currentJumpCount > 0)
                {
                    Jump();
                }
            }

            // if ball is to the right of player, move right
            if (ball.transform.position.x > transform.position.x && ball.transform.position.x < transform.position.x + ballXRange)
            {
                MoveRight();

                if (ball.transform.position.y > transform.position.y && ball.transform.position.y < transform.position.y + ballYRange && currentJumpCount > 0)
                {
                    Jump();
                }
            }
        }

        if (Mathf.Sign(ball.transform.position.x) != courtSide)
        {
            MoveToSpawn();
        }
    }

    public void MoveLeft()
    {
        GetComponent<SpriteRenderer>().flipX = true;
        transform.position += moveSpeed * Time.deltaTime * Vector3.left;
    }

    public void MoveRight()
    {
        GetComponent<SpriteRenderer>().flipX = false;
        transform.position += moveSpeed * Time.deltaTime * Vector3.right;
    }

    public void Jump()
    {
        hasJumped = true;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        currentJumpCount--;
    }

    public void MoveToSpawn()
    {
        if (transform.position.x > spawnPoint.x)
        {
            MoveLeft();
        }
        else if (transform.position.x < spawnPoint.x)
        {
            MoveRight();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // hit ball, tell ball who it's owned by (last hitter's team)
        if (collision.gameObject.GetComponent<Ball>())
        {
            collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(-transform.position.x, hitForce);
            collision.gameObject.GetComponent<Ball>().ownedBy = teamIdentity;
        }

        // handle jump stuff
        if (collision.gameObject.GetComponent<Wall>())
        {
            isGrounded = true;
            hasJumped = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // handle more jump stuff
        if (collision.gameObject.GetComponent<Wall>())
        {
            isGrounded = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // smash ball when it is in trigger zone
        if (collision.gameObject.GetComponent<Ball>())
        {
            if (Input.GetKeyDown(smashKey))
            {
                collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(-transform.position.x * 5, -hitForce);
                collision.gameObject.GetComponent<Ball>().ownedBy = teamIdentity;
            }
        }

        // hit opponents away in trigger zone
        if (collision.gameObject.GetComponent<Player>() && collision.gameObject.GetComponent<Player>().courtSide != courtSide)
        {
            if (Input.GetKeyDown(smashKey))
            {
                collision.gameObject.GetComponent<Player>().rb.velocity = new Vector2(hitForce * courtSide, 4);
            }
        }
    }

    public IEnumerator ResetPosition()
    {
        if (!GameManager.Instance.gameOver)
        {
            yield return new WaitForSeconds(2);
        }

        transform.position = spawnPoint;

        yield break;
    }
}
