using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class InGamePlayerList : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject inGamePlayerListPanel;

    [SerializeField]
    private GameObject inGamePlayerID;

    [SerializeField]
    private GameObject inGamePlayerCount;

    [SerializeField]
    private WinnerChecker winnerChecker;

    private GameObject list;

    private GameObject playerCount;

    public int playerNum;

    public int aliveCount = 0;

    private Dictionary<int, GameObject> playerEntries = new Dictionary<int, GameObject>();

    IEnumerator Start()
    {
        yield return new WaitUntil(() => (PhotonNetwork.PlayerList).Length > 0);

        playerNum = PhotonNetwork.PlayerList.Length;

        playerCount = Instantiate(inGamePlayerCount, this.transform);

        list = Instantiate(inGamePlayerListPanel, this.transform);

        list.SetActive(false);

        UpdatePlayerList();

        UpdateAlivePlayerCount();
    }

    public void OnCheckAlivePlayer(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            list.SetActive(true);
        }
        else if (ctx.phase == InputActionPhase.Canceled)
        {
            list.SetActive(false);
        }
    }

    private void UpdatePlayerList()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerEntry = Instantiate(inGamePlayerID, list.transform);

            playerEntry.GetComponent<TMP_Text>().text = $"{player.NickName}";

            playerEntries[player.ActorNumber] = playerEntry;
        }
    }

    public void UpdateAlivePlayerCount() // ���� �ϴ� ���� �÷��̾� ǥ��
    {
        int i = 0;
        aliveCount = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("isHit"))
            {
                bool isHit = (bool)player.CustomProperties["isHit"];

                if (isHit == false) // ���� ���� �÷��̾ ī��Ʈ
                {
                    aliveCount++;

                    Debug.Log(player.NickName + i++);
                }
            }
            else
            {
                aliveCount++;
            }
        }

        winnerChecker.CheckWinner();

        playerCount.GetComponent<TMP_Text>().text = $"{aliveCount}";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject playerEntry = Instantiate(inGamePlayerID, list.transform);

        playerEntry.GetComponent<TMP_Text>().text = $"{newPlayer.ActorNumber}";

        playerEntries[newPlayer.ActorNumber] = playerEntry;

        // �� �����Ǹ� �߰������� ���� �� ���� ������ ���߿� ������ ��.
        //playerNum = PhotonNetwork.PlayerList.Length;

        //UpdateAlivePlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (playerEntries.ContainsKey(otherPlayer.ActorNumber))
        {
            Destroy(playerEntries[otherPlayer.ActorNumber]);

            playerEntries.Remove(otherPlayer.ActorNumber);
        }

        playerNum = PhotonNetwork.PlayerList.Length;

        UpdateAlivePlayerCount();
    }
}
