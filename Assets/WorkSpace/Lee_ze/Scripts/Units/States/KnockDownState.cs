using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockDownState : IPlayerStates
{
    PlayerControl player;

    public void EnterState(PlayerControl player)
    {
        this.player = player;

        this.player.audioSource.PlayOneShot(player.getHit); // ���� �ǰ� ����

        this.player.photonView.RPC("RPC_PlayHitSound", RpcTarget.Others, player.transform.position); // RPC�� kick ���� ���� ��

        player.playerAnim.SetBool("IsKnockDown", true);
    }

    public void UpdatePerState()
    {
        //Any State���� IsKnockDown�� �ݺ������� true�� �Ǵ� ������ ����.
        AnimatorStateInfo stateInfo = player.playerAnim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("KnockDown") == false)
        {
            return;
        }

        player.playerAnim.SetBool("IsKnockDown", false);
    }

    public void ExitState()
    {

    }
}
