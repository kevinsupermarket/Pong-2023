using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb;
    public TrailRenderer ballTrail;
    public GameObject innerWall;

    public Vector2 lastVelocity;

    public float moveSpeed;

    public bool isSpiked;

    public bool isScored;
    public float ownedBy;

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
    }

    private void Update()
    {
        lastVelocity = rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Wall>())
        {
            /* THIS NEEDS TO BE CHANGED!
             - cannot bounce horizontally
             - will always bounce vertically even if colliding with a horizontal wall
             - need to find a good method to detect which way to bounce
            */

            rb.velocity = new Vector2(lastVelocity.x, -lastVelocity.y * 0.9f);
        }
    }

    public IEnumerator ResetPosition()
    {
        if (!GameManager.Instance.gameOver)
        {
            yield return new WaitForSeconds(2);
        }

        isScored = false;
        ownedBy = -1;

        transform.position = Vector3.zero;

        // resets trail position
        for (var i = 0; i < ballTrail.positionCount; i++)
        {
            ballTrail.SetPosition(i, transform.position);
        }

        // move towards winner of last point
        rb.velocity = Vector2.left * moveSpeed;

        yield break;
    }
}
