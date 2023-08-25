using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 direction;
    public float moveSpeed;


    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Move()
    {
        transform.position += moveSpeed * Time.deltaTime * direction;
    }

    public void Bounce(Vector3 newDirection)
    {
        direction = newDirection;
    }

    public IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(2);

        transform.position = Vector3.zero;

        // move towards winner of last point
        direction = -direction;

        yield break;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Wall>())
        {
            Bounce(new Vector3(direction.x, -direction.y, direction.z));
        }
    }
}
