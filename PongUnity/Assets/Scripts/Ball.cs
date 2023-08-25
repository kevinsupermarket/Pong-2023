using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector3 direction;
    public float moveSpeed;
    public bool isScored;

    private void Start()
    {
        rb.velocity = Vector2.left * moveSpeed;
    }

    public void Bounce(Vector3 newDirection)
    {
        direction = newDirection;
    }

    public IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(2);

        isScored = false;

        transform.position = Vector3.zero;

        // move towards winner of last point
        direction = -direction;
        rb.velocity = Vector2.left * moveSpeed;

        yield break;
    }
}
