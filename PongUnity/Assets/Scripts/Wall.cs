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
        if (canKnockOutPlayers && collision.gameObject.GetComponent<Player>() && !collision.gameObject.GetComponent<Player>().isKnockedOut)
        {
            collision.gameObject.GetComponent<Player>().isKnockedOut = true;

            foreach (WallIgnoreCol wallIgnoreCol in FindObjectsOfType<WallIgnoreCol>())
            {
                wallIgnoreCol.EnableCollisionForKO(collision.gameObject.GetComponent<Player>().gameObject);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (canKnockOutPlayers && collision.gameObject.GetComponent<Player>() && collision.gameObject.GetComponent<Player>().canRecover)
        {
            collision.gameObject.GetComponent<Player>().canRecover = false;
            collision.gameObject.GetComponent<Player>().isKnockedOut = false;

            foreach (WallIgnoreCol wallIgnoreCol in FindObjectsOfType<WallIgnoreCol>())
            {
                wallIgnoreCol.DisableCollisionForNonKO(collision.gameObject.GetComponent<Player>().gameObject);
            }
        }
    }
}
