using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public bool isAI;
    public float moveSpeed;
    public float maxYPosition;
    public KeyCode upKey;
    public KeyCode downKey;
    public int ballHitXDirection;

    // Update is called once per frame
    void Update()
    {
        // let this thing move itself
        if (isAI)
        {
            MoveAuto();
        }

        // let's move this thing
        if (!isAI && Input.GetKey(upKey) && transform.position.y < maxYPosition)
        {
            MoveUp();
        }

        if (!isAI && Input.GetKey(downKey) && transform.position.y > -maxYPosition)
        {
            MoveDown();
        }
    }

    public void MoveUp()
    {
        transform.position += moveSpeed * Time.deltaTime * Vector3.up;
    }

    public void MoveDown()
    {
        transform.position += moveSpeed * Time.deltaTime * Vector3.down;
    }

    public void MoveAuto()
    {
        // if ball is above paddle, move up
        if (FindObjectOfType<Ball>().transform.position.y > transform.position.y && transform.position.y < maxYPosition)
        {
            transform.position += moveSpeed * Time.deltaTime * Vector3.up;
        }

        // if ball is below paddle, move down
        if (FindObjectOfType<Ball>().transform.position.y < transform.position.y && transform.position.y > -maxYPosition)
        {
            transform.position += moveSpeed * Time.deltaTime * Vector3.down;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>())
        {
            Vector3 hitDirection = new(ballHitXDirection, collision.transform.position.y - transform.position.y, 0);
            collision.gameObject.GetComponent<Ball>().Bounce(hitDirection);
        }
    }

    public IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(2);

        transform.position = new Vector3(transform.position.x, 0);

        yield break;
    }
}
