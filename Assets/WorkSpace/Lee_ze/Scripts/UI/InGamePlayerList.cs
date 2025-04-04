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

    private void UpdatePlayerList() // 현재 같이 플레이 중인 플레이어 리스트 업데이트
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerEntry = Instantiate(inGamePlayerID, list.transform);

            playerEntry.GetComponent<TMP_Text>().text = $"{player.NickName}";

            playerEntries[player.ActorNumber] = playerEntry;
        }
    }

    public void UpdateAlivePlayerCount() // 왼쪽 하단 생존 플레이어 표시
    {
        aliveCount = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("isHit"))
            {
                bool isHit = (bool)player.CustomProperties["isHit"];

                if (isHit == false) // 맞지 않은 플레이어만 카운트
                {
                    aliveCount++;
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
