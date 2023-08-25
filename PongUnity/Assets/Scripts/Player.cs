using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isAI;

    public Rigidbody2D rb;

    public KeyCode upKey;
    public KeyCode leftKey;
    public KeyCode rightKey;

    public Vector3 spawnPoint;
    public float moveSpeed;
    public float jumpForce;
    public bool isGrounded;
    public float hitForce;


    void Awake()
    {
        spawnPoint = transform.position;
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
        if (!isAI && Input.GetKeyDown(upKey) && isGrounded)
        {
            Jump();
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
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    public void MoveAuto()
    {
        // if ball is above paddle, move up
        if (FindObjectOfType<Ball>().transform.position.y > transform.position.y)
        {
            transform.position += moveSpeed * Time.deltaTime * Vector3.up;
        }

        // if ball is below paddle, move down
        if (FindObjectOfType<Ball>().transform.position.y < transform.position.y)
        {
            transform.position += moveSpeed * Time.deltaTime * Vector3.down;
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
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Wall>())
        {
            isGrounded = false;
        }
    }

    public IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(2);

        transform.position = spawnPoint;

        yield break;
    }
}
