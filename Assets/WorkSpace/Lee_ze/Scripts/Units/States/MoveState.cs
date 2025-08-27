using Photon.Pun;
using UnityEngine;

public class MoveState : IPlayerStates
{
    PlayerControl player;

    public void EnterState(PlayerControl player)
    {
        this.player = player;
    }

    public void UpdatePerState()
    {
        PlayerMovement();

        // °‰ State ¿¸»Ø
        if (player.direction == Vector2.zero)
        {
            player.ChangeStateTo(new IdleState());

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
        player.photonView.RPC("RPC_StopRunningSound", RpcTarget.All, player.transform.position);
    }

    private void PlayerMovement()
    {
        Vector3 camForward = Camera.main.transform.forward;

        camForward.y = 0;

        Vector3 camRight = Camera.main.transform.right;

        camRight.y = 0;

        if (player.direction != Vector2.zero)
        {
            player.targetSpeed = player.isRunning ? 0.12f : 0.06f; // ∂‹ : ∞…¿Ω

            player.moveSpeed = Mathf.Lerp(player.moveSpeed, player.targetSpeed, Time.deltaTime * 5f);

            Vector3 moveDir = (camRight * player.direction.x + camForward * player.direction.y).normalized;

            Vector3 move = player.moveSpeed * new Vector3(moveDir.x, 0, moveDir.z);

            player.rb.MovePosition(player.transform.position + move);

            player.rb.MovePosition(player.transform.position + move);

            player.modelRotator.SetTargetDirection(move);
        }
        else
        {
            player.isRunning = false;

            player.moveSpeed = Mathf.Lerp(player.moveSpeed, 0, Time.deltaTime * 5f);
        }

        // ∂€ ∂ß running sound
        if (player.isRunning == true)
        {
            player.photonView.RPC("RPC_PlayRunningSound", RpcTarget.All, player.transform.position);
        }
        else
        {
            player.photonView.RPC("RPC_StopRunningSound", RpcTarget.All, player.transform.position);
        }

        player.playerAnim.SetFloat("Speed", player.moveSpeed / 0.12f);

    }
}
