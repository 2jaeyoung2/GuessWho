using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem.LowLevel;

public class IdleState : IPlayerStates
{
    PlayerControl player;

    public void EnterState(PlayerControl player)
    {
        this.player = player;
    }

    public void UpdatePerState()
    {
        player.moveSpeed = Mathf.Lerp(player.moveSpeed, 0, Time.deltaTime * 5f);

        player.playerAnim.SetFloat("Speed", player.moveSpeed / 0.12f);

        // ¡ä State ÀüÈ¯

        if (player.direction != Vector2.zero)
        {
            player.ChangeStateTo(new MoveState());

            return;
        }

        if (player.isAttackTriggered == true)
        {
            player.ChangeStateTo(new AttackState());

            return;
        }

        if (player.isHit == true)
        {
            player.ChangeStateTo(new KnockDownState());

            return;
        }

        if (player.isNPC == true)
        {
            player.ChangeStateTo(new ApologizeState());

            return;
        }
    }

    public void ExitState()
    {

    }
}
