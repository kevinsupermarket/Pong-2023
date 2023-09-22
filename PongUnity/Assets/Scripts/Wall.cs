using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    Player[] players;

    public bool canKnockOutPlayers;
    public float ballBounceDirection;
    public bool isFloor;

 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if this wall can knock out players, do so when they collide with this wall, then prevent them from passing through any NoCol walls
        if (canKnockOutPlayers && collision.gameObject.GetComponent<Player>() && !collision.gameObject.GetComponent<Player>().isKnockedOut)
        {
            collision.gameObject.GetComponent<Player>().isKnockedOut = true;

            StartCoroutine(collision.gameObject.GetComponent<Player>().KOCooldown());

            foreach (WallIgnoreCol wallIgnoreCol in FindObjectsOfType<WallIgnoreCol>())
            {
                wallIgnoreCol.EnableCollisionForKO(collision.gameObject.GetComponent<Player>().gameObject);
            }
        }
    }
}