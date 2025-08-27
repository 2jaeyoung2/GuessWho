using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApologizeState : IPlayerStates
{
    PlayerControl player;

    Transform playerModeling;

    Transform backUp;

    public void EnterState(PlayerControl player)
    {
        this.player = player;

        playerModeling = this.player.transform.Find("Modeling");

        backUp = playerModeling;
    }

    public void UpdatePerState()
    {
        player.StartCoroutine(Apologize());

        // ¡ä State ÀüÈ¯

        if (player.isNPC == false)
        {
            player.ChangeStateTo(new IdleState());
        }
    }

    public void ExitState()
    {
        playerModeling = backUp;
    }

    IEnumerator Apologize()
    {
        player.moveSpeed = 0;

        player.apologizeTo.y = playerModeling.position.y;

        playerModeling.LookAt(player.apologizeTo);

        player.playerAnim.SetBool("IsNPC", true);

        yield return new WaitForSeconds(3.5f);

        player.playerAnim.SetBool("Forgived", true);

        yield return null;

        player.playerAnim.SetBool("IsNPC", false);

        player.playerAnim.SetBool("Forgived", false);

        player.isNPC = false; 
    }
}