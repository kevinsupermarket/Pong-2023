using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public KeyCode spikeKey;

    // spawnpoint
    Vector3 spawnPoint;

    // moving vars
    public float moveSpeed;
    public float maxMoveSpeed;
    public float jumpForce;
    public int maxJumpCount;
    int currentJumpCount;
    bool hasJumped;
    public bool isGrounded;

    // hitting vars
    public float hitForce;
    public float maxSpikeCooldown;
    public float currentSpikeCooldown;
    public bool canSpike;
    public bool isBallInSpikeRange;

    // in-game team vars
    float courtSide;
    float teamIdentity;

    // sumo stuff
    public bool canBeHit;
    public bool isKnockedOut;
    public float maxKOCooldown;
    float currentKOCooldown;
    public bool isOpponentInHitRange;
    Player opponentInHitRange;

    // AI
    public float ballXRange;
    public float ballYRange;


    public static Player Instance;

    public Image cooldownFill;
    public AudioSource audioSpikePlayer;
    public AudioSource audioJumpPlayer;
    public AudioSource audioGruntPlayer;

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

        canSpike = true;

        currentSpikeCooldown = maxSpikeCooldown;
        currentJumpCount = maxJumpCount;
        currentKOCooldown = maxKOCooldown;

        // show tag above player if not an AI
        if (!isAI)
        {
            playerTag.text = gameObject.name;
        }
    }


    void UpdateFillUI()
    {
        if (cooldownFill == null) return;
        cooldownFill.fillAmount = currentSpikeCooldown / maxSpikeCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        // hit cooldown bar
        UpdateFillUI();

        // let this thing move itself if it's an AI
        if (isAI)
        {
            AutoMove();
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

        // spike the ball -- NEED TO ADD RANDOMNESS FOR AI HERE!
        if (isBallInSpikeRange && canSpike && (Input.GetKeyDown(spikeKey) || isAI))
        {
            if (GetComponent<SpriteRenderer>().flipX) // moving left
            {
                audioSpikePlayer.Play();
                audioGruntPlayer.Play();
                ball.rb.velocity = new Vector2(-hitForce, -hitForce * 2);
            }

            else if (!GetComponent<SpriteRenderer>().flipX) // moving right
            {
                audioSpikePlayer.Play();
                audioGruntPlayer.Play();
                ball.rb.velocity = new Vector2(hitForce, -hitForce * 2);
            }

            ball.ownedBy = teamIdentity;
            ball.isSpiked = true;
            if (ball.transform.position.y >= FindObjectOfType<ScoreLine>().transform.position.y)
            {
                ball.wasSpikedAboveScoreLine = true;
            }

            canSpike = false;
            StartCoroutine(SpikeCooldown());
        }

        // hit opponents -- NEED TO ADD RANDOMNESS FOR AI HERE!
        if (isOpponentInHitRange && canSpike && (Input.GetKeyDown(spikeKey) || isAI))
        {
            // "hitTrigger.offset.x" used to determine direction to hit player towards (hit players left when facing left, vice versa)
            opponentInHitRange.rb.velocity = new Vector2(hitForce * hitTrigger.offset.x, hitForce / 2);

            canSpike = false;
            StartCoroutine(SpikeCooldown());
        }

        if (!canSpike)
        {
            currentSpikeCooldown -= Time.deltaTime;
        }

        // set cooldown for KO
        if (isKnockedOut)
        {
            currentKOCooldown -= Time.deltaTime;
        }


        if (!isAI && (Input.GetKey(leftKey) || Input.GetKey(rightKey)))
        {
            Move(GetDirection());
            SpeedLimiter();
        }

        // make sure player tag stays on player
        playerTag.transform.position = new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z);

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
        audioJumpPlayer.Play();
        currentJumpCount--;
    }

    public void AutoMove()
    {
        /* THIS NEEDS UPDATING!
         - AI players should wander around if ball is out of range
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
                    StartCoroutine(AutoJump());
                }
            }

            // if ball is to the right of player's hit trigger, move right
            if (ball.transform.position.x > (transform.position.x + hitTrigger.offset.x) && ball.transform.position.x < (transform.position.x + hitTrigger.offset.x) + ballXRange)
            {
                Move(new Vector2(moveSpeed, 0));
                SpeedLimiter();

                if (ball.transform.position.y > transform.position.y && ball.transform.position.y < transform.position.y + ballYRange && currentJumpCount > 0)
                {
                    StartCoroutine(AutoJump());
                }
            }
        }
    }

    public IEnumerator AutoJump()
    {
        if (!hasJumped)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            audioSpikePlayer.Play();
            currentJumpCount--;
            hasJumped = true;
        }

        // delay next jump request by the values in Random.Range
        yield return new WaitForSeconds(Random.Range(0.25f, 0.4f));
        hasJumped = false;
    }

    public IEnumerator SpikeCooldown()
    {
        yield return new WaitUntil(() => currentSpikeCooldown <= 0);

        currentSpikeCooldown = maxSpikeCooldown;
        canSpike = true;

        yield break;
    }

    public IEnumerator KOCooldown()
    {
        currentSpikeCooldown = 0;

        yield return new WaitUntil(() => currentKOCooldown <= 0);

        currentSpikeCooldown = maxSpikeCooldown;
        currentKOCooldown = maxKOCooldown;
        isKnockedOut = false;

        foreach (WallIgnoreCol wallIgnoreCol in FindObjectsOfType<WallIgnoreCol>())
        {
            wallIgnoreCol.DisableCollisionForNonKO(gameObject);
        }

        yield break;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        // hit the ball on collision with it -- ball is hit in the direction the player is facing
        if (collision.gameObject.GetComponent<Ball>())
        {
            if (GetComponent<SpriteRenderer>().flipX) // moving left
            {
                collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(-hitForce / 2, hitForce);
            }
            else if (!GetComponent<SpriteRenderer>().flipX) // moving right
            {
                collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(hitForce / 2, hitForce);
            }

            collision.gameObject.GetComponent<Ball>().ownedBy = teamIdentity;
            collision.gameObject.GetComponent<Ball>().isSpiked = false;
        }

        // get jumps back when landing on a floor Wall
        if (collision.gameObject.GetComponent<Wall>() && collision.gameObject.GetComponent<Wall>().isFloor)
        {
            isGrounded = true;
            hasJumped = false;
        }

        // get jumps back when landing on top of an opponent
        if (collision.gameObject.GetComponent<Player>() && collision.gameObject.GetComponent<Player>().teamIdentity != teamIdentity && transform.position.y > collision.gameObject.GetComponent<Player>().transform.position.y)
        {
            isGrounded = true;
            hasJumped = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // subtract a jump when leaving a floor Wall
        if (collision.gameObject.GetComponent<Wall>() && collision.gameObject.GetComponent<Wall>().isFloor)
        {
            isGrounded = false;
        }

        // subtract a jump when leaving the top of an opponent
        if (collision.gameObject.GetComponent<Player>() && collision.gameObject.GetComponent<Player>().teamIdentity != teamIdentity && transform.position.y > collision.gameObject.GetComponent<Player>().transform.position.y)
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>())
        {
            isBallInSpikeRange = true;
        }

        if (collision.gameObject.GetComponent<Player>() && collision.gameObject.GetComponent<Player>().teamIdentity != teamIdentity)
        {
            isOpponentInHitRange = true;
            opponentInHitRange = collision.gameObject.GetComponent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>())
        {
            isBallInSpikeRange = false;
        }

        if (collision.gameObject.GetComponent<Player>() && collision.gameObject.GetComponent<Player>().teamIdentity != teamIdentity)
        {
            isOpponentInHitRange = false;
            opponentInHitRange = null;
        }
    }

    public IEnumerator ResetPosition()
    {
        // wait 2 seconds before restarting (unless game is over, then wait indefinitely)
        if (!GameManager.Instance.gameOver)
        {
            yield return new WaitForSeconds(2);
        }

        GetComponent<SpriteRenderer>().flipX = false;

        transform.position = spawnPoint;
        rb.velocity = Vector2.zero;

        currentJumpCount = maxJumpCount;

        canSpike = true;
        isKnockedOut = false;
        currentKOCooldown = maxKOCooldown;
        currentSpikeCooldown = maxSpikeCooldown;

        foreach (WallIgnoreCol wallIgnoreCol in FindObjectsOfType<WallIgnoreCol>())
        {
            wallIgnoreCol.DisableCollisionForNonKO(gameObject);
        }

        yield break;
    }
}