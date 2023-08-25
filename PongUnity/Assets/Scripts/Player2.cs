using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    public float moveSpeed;
    public float maxYPosition;
    public int ballHitXDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Move()
    {
        // if ball is above player 2, move up
        if (Mathf.Abs(FindObjectOfType<Ball>().transform.position.y - transform.position.y) == 1)
        {
            transform.position += moveSpeed * Time.deltaTime * Vector3.up;
        }

        if (Mathf.Abs(FindObjectOfType<Ball>().transform.position.y - transform.position.y) == -1)
        {
            transform.position += moveSpeed * Time.deltaTime * Vector3.down;
        }
    }
}
