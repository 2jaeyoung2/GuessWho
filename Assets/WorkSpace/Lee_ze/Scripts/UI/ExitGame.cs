using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using ZL.Unity;

public class ExitGame : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private PlayerControl player;

    [SerializeField]
    private Button exitButton;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => PhotonNetwork.PlayerList.Length > 0);

        exitButton.SetActive(false);

        player.OnExitButton += OnExitButton; // ��ư ������ ���� ���� �ؾߵ�.

        exitButton.onClick.AddListener(ExitThisGame);
    }

    private void OnExitButton()
    {
        Debug.Log("asdf");

        exitButton.SetActive(true);
    }

    private void ExitThisGame()
    {
        Debug.Log("������");
    }
}
