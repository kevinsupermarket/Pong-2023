using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;
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
    public GameObject innerWall;
    public GameObject outerWall;
    public float phaseFunction;

    // controls
    public KeyCode upKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode smashKey;

    // spawnpoint
    Vector3 spawnPoint;

    // moving vars
    public float moveSpeed;
    public float maxMoveSpeed;
    public float jumpForce;
    public int maxJumpCount;
    int currentJumpCount;
    bool hasJumped;
    bool isGrounded;
<<<<<<< HEAD
    public Image cooldownFill;
=======
>>>>>>> parent of 1136a54 (ugh)

    // hitting vars
    public float hitForce;
    public float maxHitCooldown;
<<<<<<< HEAD
    public float currentHitCooldown;
    bool canHit;
=======
    float currentHitCooldown;
    public bool canHit;
>>>>>>> parent of 1136a54 (ugh)

    // in-game team vars
    float courtSide;
    float teamIdentity;
    public bool isKnockedOut;
    public bool canRecover;
    public float maxKOTime;
    float currentKOTime;

    // AI
    public float ballXRange;
    public float ballYRange;


    public static Player Instance;

    Player[] players;

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

        canHit = true;

        currentHitCooldown = maxHitCooldown;
        currentJumpCount = maxJumpCount;
        currentKOTime = maxKOTime;
<<<<<<< HEAD
 
=======
>>>>>>> parent of 1136a54 (ugh)

        // show tag above player if not an AI
        if (!isAI)
        {
            playerTag.text = gameObject.name;
        }
    }

    void UpdateFillUI()
    {
        if (cooldownFill == null) return;
        cooldownFill.fillAmount = currentHitCooldown / maxHitCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFillUI();

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
            canHit = false;
        }

        // start KO timer when knocked out
        if (isKnockedOut)
        {
            CheckForKO();
        }
        if (canRecover && currentKOTime != maxKOTime)
        {
            currentKOTime = maxKOTime;
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
<<<<<<< HEAD
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
        if (!canHit)
=======
    {
        /* THIS NEEDS UPDATING!
         - AI players should be able to move anywhere between the two inner walls 
        */


        if (Ball.Instance.ownedBy != teamIdentity)
>>>>>>> parent of 1136a54 (ugh)
        {
            currentHitCooldown -= Time.deltaTime;
        }
        else if (canHit && currentHitCooldown != maxHitCooldown)
        {
            currentHitCooldown = maxHitCooldown;
        }

        if (currentHitCooldown <= 0)
        {
            canHit = true;
        }
    }

<<<<<<< HEAD
    public void CheckForKO()
=======
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

    public IEnumerator AutoJump()
>>>>>>> parent of 1136a54 (ugh)
    {
        currentHitCooldown = 0;
        currentKOTime -= Time.deltaTime;

        if (currentKOTime <= 0)
        {
            currentHitCooldown = maxHitCooldown;
            canRecover = true;
        }
<<<<<<< HEAD
=======
        // delay next jump request by the values in Random.Range
        yield return new WaitForSeconds(Random.Range(0.25f, 0.4f));
        hasJumped = false;
    }

    public void CheckForHit()
    {
        if (!canHit)
        {
            currentHitCooldown -= Time.deltaTime;
        }
        else if (canHit && currentHitCooldown != maxHitCooldown)
        {
            currentHitCooldown = maxHitCooldown;
        }

        if (currentHitCooldown <= 0)
        {
            canHit = true;
        }
    }

    public void CheckForKO()
    {
        currentHitCooldown = 0;
        currentKOTime -= Time.deltaTime;

        if (currentKOTime <= 0)
        {
            currentHitCooldown = maxHitCooldown;
            canRecover = true;
        }
>>>>>>> parent of 1136a54 (ugh)
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
                else if (rb.velocity.x > 0) // moving right
                {
                    collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(rb.velocity.x, hitForce);
                }
                else
                {
                    collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(0, hitForce);
                }

                collision.gameObject.GetComponent<Ball>().ownedBy = teamIdentity;
                collision.gameObject.GetComponent<Ball>().isSpiked = false;
            }

            // manual controls
<<<<<<< HEAD
            // hit ball (combo movement keys with spike to spike in different directions), tell ball who it's owned by (last hitter's team), un-spike ball
=======
            // hit ball in direction of current movement, tell ball who it's owned by (last hitter's team), un-spike ball
>>>>>>> parent of 1136a54 (ugh)
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
            collision.gameObject.GetComponent<Ball>().isSpiked = false;
        }

        // handle jump stuff
        if (collision.gameObject.GetComponent<Wall>())
        {
            isGrounded = true;
            hasJumped = false;
        }

        if (collision.gameObject.GetComponent<Player>() && collision.gameObject.GetComponent<Player>().teamIdentity != teamIdentity && transform.position.y > collision.gameObject.GetComponent<Player>().transform.position.y)
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

        if (collision.gameObject.GetComponent<Player>() && collision.gameObject.GetComponent<Player>().teamIdentity != teamIdentity && transform.position.y > collision.gameObject.GetComponent<Player>().transform.position.y)
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
<<<<<<< HEAD
            if (isAI && transform.position.y >= GameManager.Instance.scoreLine.transform.position.y)
=======
            if (isAI && transform.position.y >= GameManager.Instance.scoreLine.transform.position.y && canHit)
>>>>>>> parent of 1136a54 (ugh)
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
                collision.gameObject.GetComponent<Ball>().isSpiked = true;
<<<<<<< HEAD
            }

=======

                // start the spike cooldown for AI
                canHit = false;
            }

            // manual controls
            // combo movement keys (left, right, or neither) with spike to spike in different directions
>>>>>>> parent of 1136a54 (ugh)
            if (Input.GetKeyDown(smashKey) && canHit)
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
                collision.gameObject.GetComponent<Ball>().isSpiked = true;
<<<<<<< HEAD
=======
                print("Ball was spiked!");
>>>>>>> parent of 1136a54 (ugh)
            }
        }

        // hit opponents away in trigger zone
        if (collision.gameObject.GetComponent<Player>() && collision.gameObject.GetComponent<Player>().teamIdentity != teamIdentity)
        {
<<<<<<< HEAD
            if (Input.GetKeyDown(smashKey))
=======
            // auto controls
            if (isAI && canHit)
            {
                collision.gameObject.GetComponent<Player>().rb.velocity = new Vector2(hitForce * hitTrigger.offset.x, hitForce / 2);
                canHit = false;
            }

            // manual controls
            if (Input.GetKeyDown(smashKey) && canHit)
>>>>>>> parent of 1136a54 (ugh)
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

        currentJumpCount = maxJumpCount;

        isKnockedOut = false;
        currentKOTime = maxKOTime;
        currentHitCooldown = maxHitCooldown;
        canRecover = false;

        foreach (WallIgnoreCol wallIgnoreCol in FindObjectsOfType<WallIgnoreCol>())
        {
            wallIgnoreCol.DisableCollisionForNonKO(gameObject);
        }

        yield break;
    }
}
