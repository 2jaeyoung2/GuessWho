using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AttackState : IPlayerStates
{
    PlayerControl player;

    ItemType itemType;

    public void EnterState(PlayerControl player)
    {
        // TOOD: PlayerControl���� ���� �ٲٴ� ����. ���⼭ �ϴ°� �ƴ�

        this.player = player;

        if(player.holdingWeapon != null)
        {
            itemType = player.holdingWeapon.itemType;
        }
        
        if(player.holdingWeapon == null)
        {
            itemType = player.footData.itemType;
        }

        player.moveSpeed = 0;

        // ���� ����
        switch (itemType)
        {
            case (ItemType.None):

                player.StartCoroutine(AttackKick());

                break;

            case (ItemType.Stone):

                AttackThrow();
                Debug.Log("�� ������");
                break;

            case (ItemType.Gun):

                AttackShoot();
                Debug.Log("�ѽ��");
                break;
        }
    }

    public void UpdatePerState()
    {
        if (player.isAttackTriggered == false)
        {// �� �߰��Ǵ� �ٸ� ���� �������� player.isAttackTriggered = false;
            player.ChangeStateTo(new IdleState());

            return;
        }

        if (player.isHit == true)
        {
            player.ChangeStateTo(new KnockDownState());

            return;
        }
    }

    public void ExitState()
    {

    }

    IEnumerator AttackKick()
    {
        player.playerAnim.SetBool("IsKick", true);

        if (player.photonView.IsMine)
        {
            player.audioSource.PlayOneShot(player.kick); // ���� kick ����

            player.photonView.RPC("RPC_PlayAttackSound", RpcTarget.Others, player.transform.position); // RPC�� kick ���� ���� ��
        }

        yield return new WaitForSeconds(1f);

        player.playerAnim.SetBool("IsKick", false);

        player.isAttackTriggered = false;
    }

    void AttackThrow()
    {

        player.isAttackTriggered = false;
    }

    void AttackShoot()
    {
        Debug.Log("�� ���");
        player.isAttackTriggered = false;
    }
}
