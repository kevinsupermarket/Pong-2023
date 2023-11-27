using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // this is just at the top so it's easier to find amongst everything else
    public bool isAI;

    // stuff that won't be called anywhere else for any reason
    Ball ball;
    BoxCollider2D hitTrigger;
    float hitTriggerOffsetX;
    float crosshairPosX;

    // self & self-related objects
    public Rigidbody2D rb;
    public TMP_Text playerTag;
    public GameObject crosshair;

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
    public bool hasJumped;
    public bool isGrounded;
    public Vector2 lastVelocity;

    // hitting vars
    public float hitForce;
    public float maxSpikeCooldown;
    public float currentSpikeCooldown;
    public float maxHitstopTime;
    float currentHitstopTime;
    public bool canSpike;
    public bool isBallInSpikeRange;
    public bool spikedTheBall;

    // in-game team vars
    float courtSide;
    float teamIdentity;

    // sumo stuff
    public bool wasHit;
    public float maxHitStunCooldown;
    public float currentHitstunCooldown;
    public bool isKnockedOut;
    public bool canRecover;
    public float maxKOCooldown;
    public float currentKOCooldown;
    public bool isOpponentInHitRange;
    Player opponentInHitRange;

    // AI
    public float ballXRange;
    public float ballYRange;


    public static Player Instance;

    public Image cooldownFill;

    void Awake()
    {
        Instance = this;
        spawnPoint = transform.position;
    }

    private void Start()
    {
        // get the baaaaawll
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

        // set the hit trigger and crosshair's position in relation to where player is facing
        crosshairPosX = crosshair.transform.localPosition.x;

        if (GetComponent<SpriteRenderer>().flipX)
        {
            hitTrigger.offset = new Vector2(-hitTriggerOffsetX, hitTrigger.offset.y);
            crosshair.transform.localPosition = new Vector2(-crosshairPosX, crosshair.transform.localPosition.y);
        }
        else
        {
            hitTrigger.offset = new Vector2(hitTriggerOffsetX, hitTrigger.offset.y);
            crosshair.transform.localPosition = new Vector2(crosshairPosX, crosshair.transform.localPosition.y);
        }

        // determine what team the player is on based on x position from 0,0 (-1 = home, 1 = away)
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

        // set cooldowns and whatnot
        canSpike = true;

        currentJumpCount = maxJumpCount;
        currentHitstopTime = maxHitstopTime;
        currentSpikeCooldown = maxSpikeCooldown;
        currentKOCooldown = maxKOCooldown;
        currentHitstunCooldown = maxHitStunCooldown;

        // disable player crosshair if an AI
        if (isAI)
        {
            crosshair.SetActive(false);
        }

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


        // trigger cooldown when spike is performed
        if (canSpike && Input.GetKeyDown(spikeKey))
        {
            if (isBallInSpikeRange)
            {
                if (ball.transform.position.y >= FindObjectOfType<ScoreLine>().transform.position.y)
                {
                    ball.wasSpikedAboveScoreLine = true;
                }
                else
                {
                    ball.wasSpikedAboveScoreLine = false;
                }

                StartCoroutine(SpikeHitstop());
                ball.ownedBy = teamIdentity;
                ball.isSpiked = true;
                spikedTheBall = true;
            }

            // hit opponents
            if (isOpponentInHitRange)
            {
                // knock opponent out of hitstop if they are hit during it
                if (opponentInHitRange.spikedTheBall && opponentInHitRange.rb.constraints == RigidbodyConstraints2D.FreezeAll)
                {
                    StopCoroutine(opponentInHitRange.SpikeHitstop());

                    ball.rb.constraints = RigidbodyConstraints2D.None;
                    ball.rb.velocity = ball.lastVelocity;
                    ball.isSpiked = false;

                    opponentInHitRange.rb.constraints = ~RigidbodyConstraints2D.FreezePosition;
                    opponentInHitRange.rb.velocity = opponentInHitRange.lastVelocity;
                    opponentInHitRange.currentHitstopTime = opponentInHitRange.maxHitstopTime;
                    opponentInHitRange.spikedTheBall = false;
                }

                // "hitTrigger.offset.x" used to determine direction to hit player towards (hit players left when facing left, vice versa)
                opponentInHitRange.wasHit = true;
                StartCoroutine(opponentInHitRange.HitstunCooldown());
                opponentInHitRange.rb.velocity = new Vector2(hitForce * hitTrigger.offset.x, hitForce / 2);
            }

            StartCoroutine(SpikeCooldown());
            canSpike = false;
        }


        // AI spike the ball
        if (isAI && isBallInSpikeRange && canSpike)
        {
            StartCoroutine(AutoSpike());
        }

        // AI hit opponents
        if (isAI && isOpponentInHitRange && canSpike)
        {
            StartCoroutine(AutoHitOpponents());
        }


        // count down the cooldown for hitstop
        if (spikedTheBall && ball.isSpiked && ball.rb.constraints == RigidbodyConstraints2D.FreezeAll)
        {
            currentHitstopTime -= Time.deltaTime;
        }

        // count down the cooldown for spike
        if (!canSpike)
        {
            currentSpikeCooldown -= Time.deltaTime;
        }

        // count down the cooldown for hitstun
        if (wasHit)
        {
            currentHitstunCooldown -= Time.deltaTime;
        }

        // count down the cooldown for KO
        if (isKnockedOut)
        {
            currentKOCooldown -= Time.deltaTime;
        }


        // move manually if not AI & not in hitstun
        if (!isAI && !wasHit && (Input.GetKey(leftKey) || Input.GetKey(rightKey)))
        {
            Move(GetDirection());
            SpeedLimiter();
        }

        // make sure player tag stays on player
        playerTag.transform.position = new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z);
    }



    public Vector2 GetDirection()
    {
        int horizontal = 0;
        if (Input.GetKey(leftKey)) horizontal = -1;
        if (Input.GetKey(rightKey)) horizontal = 1;
        return new Vector2(horizontal, 0);
    }

    public void Move(Vector2 direction)
    {
        if (direction.x < 0 && rb.constraints != RigidbodyConstraints2D.FreezeAll)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            hitTrigger.offset = new Vector2(-hitTriggerOffsetX, hitTrigger.offset.y);
            crosshair.transform.localPosition = new Vector2(-crosshairPosX, crosshair.transform.localPosition.y);
        }
        else if (direction.x > 0 && rb.constraints != RigidbodyConstraints2D.FreezeAll)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            hitTrigger.offset = new Vector2(hitTriggerOffsetX, hitTrigger.offset.y);
            crosshair.transform.localPosition = new Vector2(crosshairPosX, crosshair.transform.localPosition.y);
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
        currentJumpCount--;
        GetComponent<Animator>().SetTrigger("isJumping");
        /*should play jump anim */
    }

    public void AutoMove()
    {
        if (ball.ownedBy != teamIdentity)
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

        // let AI recover from KO on its own
        if (canRecover)
        {
            Move(new Vector2(-moveSpeed * Mathf.Sign(transform.position.x), 0));
            SpeedLimiter();
        }
    }

    public IEnumerator AutoJump()
    {
        if (!hasJumped)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            currentJumpCount--;
            hasJumped = true;
        }

        // delay next jump request by the values in Random.Range
        yield return new WaitForSeconds(Random.Range(0.25f, 0.4f));
        hasJumped = false;

        yield break;
    }

    public IEnumerator AutoSpike()
    {
        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

        if (isBallInSpikeRange)
        {
            if (ball.transform.position.y >= FindObjectOfType<ScoreLine>().transform.position.y)
            {
                ball.wasSpikedAboveScoreLine = true;
            }
            else
            {
                ball.wasSpikedAboveScoreLine = false;
            }

            // save ball's velocity before being frozen; useful for restoring velocity if ball is knocked out of hitstop
            ball.lastVelocity = ball.rb.velocity;

            StartCoroutine(SpikeHitstop());
            ball.ownedBy = teamIdentity;
            ball.isSpiked = true;
            spikedTheBall = true;
        }

        StartCoroutine(SpikeCooldown());
        canSpike = false;

        yield break;
    }

    public IEnumerator AutoHitOpponents()
    {
        yield return new WaitForSeconds(Random.Range(0.125f, 0.25f));

        if (opponentInHitRange)
        {
            if (opponentInHitRange.spikedTheBall && opponentInHitRange.rb.constraints == RigidbodyConstraints2D.FreezeAll)
            {
                StopCoroutine(opponentInHitRange.SpikeHitstop());

                ball.rb.constraints = RigidbodyConstraints2D.None;
                ball.rb.velocity = ball.lastVelocity;
                ball.isSpiked = false;

                opponentInHitRange.rb.constraints = ~RigidbodyConstraints2D.FreezePosition;
                opponentInHitRange.rb.velocity = opponentInHitRange.lastVelocity;
                opponentInHitRange.currentHitstopTime = opponentInHitRange.maxHitstopTime;
                opponentInHitRange.spikedTheBall = false;
            }

            opponentInHitRange.wasHit = true;
            StartCoroutine(opponentInHitRange.HitstunCooldown());
            opponentInHitRange.rb.velocity = new Vector2(hitForce * hitTrigger.offset.x, hitForce / 2);
        }

        StartCoroutine(SpikeCooldown());
        canSpike = false;

        yield break;
    }


    public IEnumerator SpikeHitstop()
    {
        foreach (Player player in FindObjectsOfType<Player>())
        {
            if (player.rb.constraints == RigidbodyConstraints2D.FreezeAll)
            {
                player.rb.constraints = ~RigidbodyConstraints2D.FreezePosition;
                player.rb.velocity = player.lastVelocity;
                player.currentHitstopTime = player.maxHitstopTime;
                ball.rb.velocity = ball.lastVelocity;
            }
            player.spikedTheBall = false;
        }

        lastVelocity = rb.velocity;

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        ball.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        ball.rb.angularVelocity = 0;

        // show the spike direction line in the ball while in hitstop
        if (GetComponent<SpriteRenderer>().flipX)
        {
            StartCoroutine(ball.SetSpikeDirLinePos(-hitForce, -hitForce * 2));
        }
        else
        {
            StartCoroutine(ball.SetSpikeDirLinePos(hitForce, -hitForce * 2));
        }


        yield return new WaitUntil(() => currentHitstopTime <= 0);


        currentHitstopTime = maxHitstopTime;

        rb.velocity = lastVelocity;
        rb.constraints = ~RigidbodyConstraints2D.FreezePosition;
        ball.rb.constraints = RigidbodyConstraints2D.None;

        if (GetComponent<SpriteRenderer>().flipX) // moving left
        {
            ball.rb.velocity = new Vector2(-hitForce, -hitForce * 2);
            ball.rb.angularVelocity -= hitForce * 4;
        }

        else if (!GetComponent<SpriteRenderer>().flipX) // moving right
        {
            ball.rb.velocity = new Vector2(hitForce, -hitForce * 2);
            ball.rb.angularVelocity += hitForce * 4;
        }

        yield break;
    }

    public IEnumerator SpikeCooldown()
    {
        yield return new WaitUntil(() => currentSpikeCooldown <= 0 && !isKnockedOut);

        currentSpikeCooldown = maxSpikeCooldown;
        canSpike = true;

        yield break;
    }

    public IEnumerator HitstunCooldown()
    {
        yield return new WaitUntil(() => currentHitstunCooldown <= 0);

        currentHitstunCooldown = maxHitStunCooldown;
        wasHit = false;

        yield break;
    }

    public IEnumerator KOCooldown()
    {
        currentSpikeCooldown = 0;
        canSpike = false;

        yield return new WaitUntil(() => currentKOCooldown <= 0);

        currentSpikeCooldown = maxSpikeCooldown;
        canSpike = true;
        currentKOCooldown = maxKOCooldown;
        isKnockedOut = false;
        canRecover = true;

        foreach (WallIgnoreCol wallIgnoreCol in FindObjectsOfType<WallIgnoreCol>())
        {
            wallIgnoreCol.DisableCollisionForNonKO(gameObject);
        }

        yield break;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>() && ball.rb.constraints == RigidbodyConstraints2D.None)
        {
            // hit the ball on collision with it -- ball is hit in the direction the player is facing
            if (GetComponent<SpriteRenderer>().flipX) // moving left
            {
                collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(-hitForce / 2, hitForce);
                collision.gameObject.GetComponent<Ball>().rb.angularVelocity -= hitForce * 2;
            }
            else if (!GetComponent<SpriteRenderer>().flipX) // moving right
            {
                collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(hitForce / 2, hitForce);
                collision.gameObject.GetComponent<Ball>().rb.angularVelocity += hitForce * 2;
            }

            collision.gameObject.GetComponent<Ball>().ownedBy = teamIdentity;
            collision.gameObject.GetComponent<Ball>().isSpiked = false;
            collision.gameObject.GetComponent<Ball>().wasSpikedAboveScoreLine = false;
            spikedTheBall = false;
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

            // make crosshair "glow" when something hittable is in range
            Color crosshairGlow = crosshair.GetComponent<SpriteRenderer>().color;
            crosshairGlow.a = 1;
            crosshair.GetComponent<SpriteRenderer>().color = crosshairGlow;
        }

        if (collision.gameObject.GetComponent<Player>() && collision.gameObject.GetComponent<Player>().teamIdentity != teamIdentity)
        {
            isOpponentInHitRange = true;

            Color crosshairGlow = crosshair.GetComponent<SpriteRenderer>().color;
            crosshairGlow.a = 1;
            crosshair.GetComponent<SpriteRenderer>().color = crosshairGlow;

            opponentInHitRange = collision.gameObject.GetComponent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>())
        {
            isBallInSpikeRange = false;

            // make crosshair stop "glowing" when no hittable thing is in range
            Color crosshairGlow = crosshair.GetComponent<SpriteRenderer>().color;
            crosshairGlow.a = 0.5f;
            crosshair.GetComponent<SpriteRenderer>().color = crosshairGlow;
        }

        if (collision.gameObject.GetComponent<Player>() && collision.gameObject.GetComponent<Player>().teamIdentity != teamIdentity)
        {
            isOpponentInHitRange = false;

            Color crosshairGlow = crosshair.GetComponent<SpriteRenderer>().color;
            crosshairGlow.a = 0.5f;
            crosshair.GetComponent<SpriteRenderer>().color = crosshairGlow;

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

        // reset player position to spawnpoint, set speed to 0
        transform.position = spawnPoint;
        rb.velocity = Vector2.zero;

        // reset crosshair position
        if (teamIdentity == 0) crosshair.transform.localPosition = new Vector2(crosshairPosX, crosshair.transform.localPosition.y);
        if (teamIdentity == 1) crosshair.transform.localPosition = new Vector2(-crosshairPosX, crosshair.transform.localPosition.y);

        // reset default looking direction
        if (teamIdentity == 0) GetComponent<SpriteRenderer>().flipX = false;
        if (teamIdentity == 1) GetComponent<SpriteRenderer>().flipX = true;

        // reset jump count
        currentJumpCount = maxJumpCount;

        // reset cooldowns, spike, and KO state
        isBallInSpikeRange = false;
        isKnockedOut = false;
        canRecover = false;
        wasHit = false;
        currentKOCooldown = maxKOCooldown;
        currentSpikeCooldown = maxSpikeCooldown;
        currentHitstopTime = maxHitstopTime;

        // reset constraints if player was in hitstop
        rb.constraints = ~RigidbodyConstraints2D.FreezePosition;

        // reset collision state for walls
        foreach (WallIgnoreCol wallIgnoreCol in FindObjectsOfType<WallIgnoreCol>())
        {
            wallIgnoreCol.DisableCollisionForNonKO(gameObject);
        }

        yield break;
    }
}