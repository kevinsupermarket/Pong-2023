using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    Player _player;

    private void Start()
    {
        animator = GetComponent<Animator>();
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (_player.rb.velocity.x != 0 && _player.isGrounded)
        {
            animator.SetBool("isRunning", true);
        }
        if (_player.rb.velocity.x == 0 && _player.isGrounded)
        {
            animator.SetBool("isRunning", false);
        }

        if (_player.hasJumped)
        {
            animator.SetBool("isJumping", true);
            animator.SetBool("isRunning", false);
        }
        if (_player.rb.velocity.y <= 0)
        {
            animator.SetBool("isJumping", false);
        }

        if ((Input.GetKeyDown(_player.spikeKey) || !_player.canSpike) && !_player.isGrounded)
        {
            animator.SetBool("isSpiking", true);
            animator.SetBool("isJumping", false);
        }
        if ((Input.GetKeyUp(_player.spikeKey) || _player.canSpike) && !_player.isGrounded)
        {
            animator.SetBool("isSpiking", false);
        }
    }
}
