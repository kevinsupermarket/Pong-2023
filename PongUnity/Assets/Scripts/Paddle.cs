using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public float moveSpeed;
    public float maxYPosition;
    public KeyCode upKey;
    public KeyCode downKey;
    public int ballHitXDirection;

    // Update is called once per frame
    void Update()
    {
        // let's move this thing
        if (Input.GetKey(upKey) && transform.position.y < maxYPosition)
        {
            MoveUp();
        }

        if (Input.GetKey(downKey) && transform.position.y > -maxYPosition)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Ball>())
        {
            Vector3 hitDirection = new Vector3(ballHitXDirection, 0, 0);
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
