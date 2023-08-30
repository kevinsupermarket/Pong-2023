using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isAI;

    Ball ball;

    public Rigidbody2D rb;

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
        currentJumpCount = maxJumpCount;
    }

    // Update is called once per frame
    void Update()
    {
        // let this thing move itself
        if (isAI)
        {
            MoveAuto();
        }

        // let's move this thing
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
    }

    public void MoveLeft()
    {
        transform.position += moveSpeed * Time.deltaTime * Vector3.left;
    }

    public void MoveRight()
    {
        transform.position += moveSpeed * Time.deltaTime * Vector3.right;
    }

    public void Jump()
    {
        hasJumped = true;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        currentJumpCount--;
    }

    public void MoveAuto()
    {
        // if ball is to the right of player, move right
        if (ball.transform.position.x > transform.position.x && ball.transform.position.x - transform.position.x < 3 && Mathf.Sign(transform.position.x) == courtSide)
        {
            MoveRight();
        }

        // if ball is to the left of player, move left
        if (ball.transform.position.x < transform.position.x && ball.transform.position.x - transform.position.x > -3 && Mathf.Sign(transform.position.x) == courtSide)
        {
            MoveLeft();
        }

        // if ball is a certain distance above player, jump
        if (ball.transform.position.y > transform.position.y + 1 && currentJumpCount > 0)
        {
            Jump();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>())
        {
            collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(-transform.position.x, hitForce);
        }

        if (collision.gameObject.GetComponent<Wall>())
        {
            isGrounded = true;
            hasJumped = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Wall>())
        {
            isGrounded = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>())
        {
            if (Input.GetKeyDown(smashKey))
            {
                collision.gameObject.GetComponent<Ball>().rb.velocity = new Vector2(-transform.position.x * 5, -hitForce);
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
