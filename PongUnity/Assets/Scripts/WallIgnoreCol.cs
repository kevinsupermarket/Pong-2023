using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallIgnoreCol : MonoBehaviour
{
    Player[] players;

    private void Start()
    {
        players = FindObjectsOfType<Player>();

        // disable collision for all players by default
        foreach (Player player in players)
        {
            Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
        }
    }

    public void DisableCollisionForNonKO(GameObject player)
    {
        // re-disable collision between this object & player that called this function when player is no longer knocked out
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
    }

    public void EnableCollisionForKO(GameObject player)
    {
        // using "false" to enable collision between this object and the player that called this function when knocked out
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
    }
}
