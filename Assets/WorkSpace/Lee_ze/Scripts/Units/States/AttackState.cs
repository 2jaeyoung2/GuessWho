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

    PlayerUI playerUI;

    public void EnterState(PlayerControl player)
    {
        // TOOD: PlayerControl���� ���� �ٲٴ� ����. ���⼭ �ϴ°� �ƴ�
        playerUI = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<PlayerUI>();

        this.player = player;

        if (player.holdingWeapon != null)
        {
            itemType = player.holdingWeapon.itemType;
        }

        if (player.holdingWeapon == null)
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

                player.StartCoroutine(AttackThrow());

                break;

            case (ItemType.Gun):

                player.StartCoroutine(AttackShoot());

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

    IEnumerator AttackThrow()
    {
        player.playerAnim.SetBool("IsThrow", true);

        player.weapons[1].SetActive(true);

        yield return new WaitForSeconds(1.8f);
        player.playerAnim.SetBool("IsThrow", false);

        player.leftBullet--;
        playerUI.ChangeLeftBulletAmount(player);

        if (player.leftBullet <= 0)
        {
            player.nowHaveItems[1] = null;
            player.holdingWeapon = null;
        }

        player.isAttackTriggered = false;
    }

    IEnumerator AttackShoot() //�� ���� �޼���
    {
        player.playerAnim.SetBool("IsShoot", true); //�� �߻� �ִϸ��̼� ����

        player.weapons[2].SetActive(true); //�÷��̾ �տ� ����ִ� ���� Ȱ��ȭ
        player.photonView.RPC("GunActive",RpcTarget.Others); //�ٸ� �÷��̾�Ե� ���̵��� ����ȭ

        player.leftBullet--; //źâ ����
        playerUI.ChangeLeftBulletAmount(player); //źâ ���� UI�� ����

        if (player.leftBullet <= 0) //źâ�� ��� �Ҹ�Ǹ�
        {
            player.nowHaveItems[1] = null; //���� ������ �ִ� ������ �κ��丮 ���
            player.holdingWeapon = null; //���� ������ �ִ� ���� ����
        }

        yield return new WaitForSeconds(1.0f);

        player.isAttackTriggered = false; 
    }
}
