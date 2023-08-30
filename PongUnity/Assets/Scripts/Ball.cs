using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb;
    public TrailRenderer ballTrail;
    public Vector3 direction;
    public Vector2 lastVelocity;
    public float moveSpeed;
    public bool isScored;

    public static Ball Instance;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rb.velocity = Vector2.left * moveSpeed;
    }

    private void Update()
    {
        lastVelocity = rb.velocity;
    }

    public void Bounce(Vector3 newDirection)
    {
        direction = newDirection;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Wall>())
        {
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

        transform.position = Vector3.zero;

        // resets trail position
        for (var i = 0; i < ballTrail.positionCount; i++)
        {
            ballTrail.SetPosition(i, transform.position);
        }

        // move towards winner of last point
        direction = -direction;
        rb.velocity = Vector2.left * moveSpeed;

        yield break;
    }
}
