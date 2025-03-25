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

        itemType = player.holdingWeapon.itemType;

        player.moveSpeed = 0;

        // ���� ����
        switch (itemType)
        {
            case (ItemType.None):

                player.StartCoroutine(AttackKick());

                break;

            case (ItemType.Stone):

                AttackThrow();

                break;

            case (ItemType.Gun):

                AttackShoot();

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

        yield return new WaitForSeconds(1f);

        player.playerAnim.SetBool("IsKick", false);

        player.isAttackTriggered = false;
    }

    void AttackThrow()
    {
        Debug.Log("�� ������");
    }

    void AttackShoot()
    {
        Debug.Log("�� ���");
    }
}
