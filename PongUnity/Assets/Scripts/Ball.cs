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

        yield break;
    }
}
