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

        this.player.audioSource.PlayOneShot(player.getHit, UnityEngine.Random.Range(0.5f, 1f)); // �ǰ� ����

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
