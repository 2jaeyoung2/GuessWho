using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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
        Vector3 camForward = Camera.main.transform.forward;

        camForward.y = 0;

        Vector3 camRight = Camera.main.transform.right;

        camRight.y = 0;

        if (player.direction != Vector2.zero)
        {
            player.targetSpeed = player.isRunning ? 0.12f : 0.06f; // �� : ����

            player.moveSpeed = Mathf.Lerp(player.moveSpeed, player.targetSpeed, Time.deltaTime * 5f);

            Vector3 moveDir = (camRight * player.direction.x + camForward * player.direction.y).normalized;

            Vector3 move = player.moveSpeed * new Vector3(moveDir.x, 0, moveDir.z);

            player.rb.MovePosition(player.transform.position + move);

            player.rb.MovePosition(player.transform.position + move);

            player.modelRotator.SetTargetDirection(move);

            RunningSound();

            player.photonView.RPC("RPC_PlayRunningSound", RpcTarget.Others, player.transform.position); // RPC�� kick ���� ���� ��
        }
        else
        {
            player.moveSpeed = Mathf.Lerp(player.moveSpeed, 0, Time.deltaTime * 5f);
        }

        player.playerAnim.SetFloat("Speed", player.moveSpeed / 0.12f);

        // �������� ���� �� IdleState�� ��ȯ
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

    }

    private void RunningSound()
    {
        if (player.isRunning == true)
        {
            if (player.audioSource.isPlaying == false) // �̹� ��� ���̸� �ٽ� �������� ����
            {
                player.audioSource.clip = player.running;

                player.audioSource.loop = true;

                player.audioSource.Play();
            }
        }
        else
        {
            player.audioSource.clip = player.running;

            player.audioSource.loop = false;

            player.audioSource.Stop();
        }
    }
}
