using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // this is just at the top so it's easier to find amongst everything else
    public bool isAI;

    // stuff that won't be called anywhere else for any reason
    Ball ball;
    BoxCollider2D hitTrigger;
    float hitTriggerOffsetX;

    // self & self-related objects
    public Rigidbody2D rb;
    public TMP_Text playerTag;

    // controls
    public KeyCode upKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode smashKey;

    // spawnpoint
    public Vector3 spawnPoint;

    // moving vars
    public float moveSpeed;
    public float maxMoveSpeed;
    public float jumpForce;
    public int maxJumpCount;
    public int currentJumpCount;
    public bool hasJumped;
    public bool isGrounded;

    // hitting vars
    public float hitForce;
    public float maxHitCooldown;
    public float currentHitCooldown;
    public bool hasHit;

    // in-game team vars
    public float courtSide;
    public float teamIdentity;

    // AI
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

        // get the hit trigger from this game object's component list
        foreach (BoxCollider2D boxcol2d in GetComponents<BoxCollider2D>())
        {
            if (boxcol2d.isTrigger)
            {
                hitTrigger = boxcol2d;
                hitTriggerOffsetX = boxcol2d.offset.x;
            }
        }

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

        currentHitCooldown = maxHitCooldown;
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

        // reset jumps
        if (isGrounded && !hasJumped)
        {
            currentJumpCount = maxJumpCount;
        }


        // set cooldown for hitting stuff
        CheckForHit();
        if (Input.GetKeyDown(smashKey))
        {
            hasHit = true;
        }


        if (!isAI && (Input.GetKey(leftKey) || Input.GetKey(rightKey)))
        {
            Move(GetDirection());
            SpeedLimiter();
        }

        // make sure player tag stays on player
        playerTag.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

    }



    public Vector2 GetDirection()
    {
        float horizontal = Input.GetAxis("Horizontal");
        return new Vector2(horizontal, 0);
    }

    public void Move(Vector2 direction)
    {
        if (direction.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            hitTrigger.offset = new Vector2(-hitTriggerOffsetX, hitTrigger.offset.y);
        }
        else if (direction.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            hitTrigger.offset = new Vector2(hitTriggerOffsetX, hitTrigger.offset.y);
        }

        rb.AddForce(moveSpeed * Time.deltaTime * direction.normalized);
    }

    public void MoveAuto()
    {
        /* THIS NEEDS UPDATING!
         - players should be able to move anywhere between the two inner walls 
        */


        if (Ball.Instance.ownedBy != teamIdentity)
        {
            // if ball is to the left of player's hit trigger, move left
            if (ball.transform.position.x < (transform.position.x + hitTrigger.offset.x) && ball.transform.position.x > (transform.position.x + hitTrigger.offset.x) - ballXRange)
            {
                Move(new Vector2(-moveSpeed, 0));
                SpeedLimiter();

                // if ball is a certain distance above player, jump
                if (ball.transform.position.y > transform.position.y && ball.transform.position.y < transform.position.y + ballYRange && currentJumpCount > 0)
                {
                    Jump();
                }
            }

            // if ball is to the right of player's hit trigger, move right
            if (ball.transform.position.x > (transform.position.x + hitTrigger.offset.x) && ball.transform.position.x < (transform.position.x + hitTrigger.offset.x) + ballXRange)
            {
                Move(new Vector2(moveSpeed, 0));
                SpeedLimiter();

                if (ball.transform.position.y > transform.position.y && ball.transform.position.y < transform.position.y + ballYRange && currentJumpCount > 0)
                {
                    Jump();
                }
            }
        }
    }

    public void SpeedLimiter()
    {
        if (rb.velocity.x > maxMoveSpeed)
        {
            rb.velocity = new Vector2(maxMoveSpeed, rb.velocity.y);
        }

        if (rb.velocity.x < -maxMoveSpeed)
        {
            rb.velocity = new Vector2(-maxMoveSpeed, rb.velocity.y);
        }
    }

    public void Jump()
    {
        hasJumped = true;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        currentJumpCount--;
    }

    public void CheckForHit()
    {
        if (hasHit)
        {
            currentHitCooldown -= Time.deltaTime;
        }
        else if (!hasHit && currentHitCooldown != maxHitCooldown)
        {
            currentHitCooldown = maxHitCooldown;
        }

        if (currentHitCooldown <= 0)
        {
            hasHit = false;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>())
        {
            // auto controls
            if (isAI && transform.position.y < GameManager.Instance.scoreLine.transform.position.y && currentHitCooldown == maxHitCooldown)
            {
                if (rb.velocity.x < 0) // moving left
                {
                    collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(rb.velocity.x, hitForce);
                }

                if (rb.velocity.x > 0) // moving right
                {
                    collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(rb.velocity.x, hitForce);
                }

                collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(0, hitForce);
                collision.gameObject.GetComponent<Ball>().ownedBy = teamIdentity;
            }

            // manual controls
            if (currentHitCooldown == maxHitCooldown)
            {
                // hit ball (combo movement keys with spike to spike in different directions), tell ball who it's owned by (last hitter's team)
                if (Input.GetKey(leftKey))
                {
                    collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(rb.velocity.x, hitForce);
                }
                else if (Input.GetKey(rightKey))
                {
                    collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(rb.velocity.x, hitForce);
                }
                else
                {
                    collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(0, hitForce);
                }

                collision.gameObject.GetComponent<Ball>().ownedBy = teamIdentity;
            }
        }

        // handle jump stuff
        if (collision.gameObject.GetComponent<Wall>())
        {
            isGrounded = true;
            hasJumped = false;
        }

        if (collision.gameObject.GetComponent<Player>())
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

        if (collision.gameObject.GetComponent<Player>())
        {
            isGrounded = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // spike the ball & hit players when they are in trigger zone
        if (collision.gameObject.GetComponent<Ball>())
        {
            // auto controls
            if (isAI && transform.position.y >= GameManager.Instance.scoreLine.transform.position.y)
            {
                if (rb.velocity.x < 0) // moving left
                {
                    collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(-hitForce, -hitForce * 2);
                }

                else if (rb.velocity.x > 0) // moving right
                {
                    collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(hitForce, -hitForce * 2);
                }

                else
                {
                    collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(0, -hitForce * 2);
                }

                collision.gameObject.GetComponent<Ball>().ownedBy = teamIdentity;
            }

            if (Input.GetKeyDown(smashKey))
            {
                if (Input.GetKey(leftKey))
                {
                    collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(-hitForce, -hitForce * 2);
                }

                else if (Input.GetKey(rightKey))
                {
                    collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(hitForce, -hitForce * 2);
                }

                else
                {
                    collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(0, -hitForce * 2);
                }

                collision.gameObject.GetComponent<Ball>().ownedBy = teamIdentity;
            }
        }

        // hit opponents away in trigger zone
        if (collision.gameObject.GetComponent<Player>() && collision.gameObject.GetComponent<Player>().teamIdentity != teamIdentity)
        {
            if (Input.GetKeyDown(smashKey))
            {
                // "hitTrigger.offset.x" used to determine direction to hit player towards (hit players left when facing left, vice versa)
                collision.gameObject.GetComponent<Player>().rb.velocity = new Vector2(hitForce * hitTrigger.offset.x, hitForce / 2);
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
        rb.velocity = Vector2.zero;

        yield break;
    }
}
