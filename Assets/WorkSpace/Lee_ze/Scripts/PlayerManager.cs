using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            // ���� ���� ��ġ ����
            Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));

            // �÷��̾� ����
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

            //Debug.Log(player);

            //Camera.main.GetComponent<RotateView>().SetTarget(player.transform);
        }
    }
}
