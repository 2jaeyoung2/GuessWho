using Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ItemSpawnPoint : MonoBehaviourPunCallbacks
{
    [SerializeField]
    [Header("������ ���� ����Ʈ")] private GameObject[] spawnPoints;


    private ItemSpawnctrl itemSpawnctrl;


    //��� ����Ʈ�� �Ѱ��ϴ� itemspawnpoints ������Ʈ�� ó�� ����Ǿ �� �޼���
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.InRoom)
            {
                int itemNum = Random.Range(0, 10);
                photonView.RPC("RPC_SpawnItems", RpcTarget.AllBuffered, itemNum);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("AllItemOff", RpcTarget.AllBuffered);

            int itemNum = Random.Range(0, 10);
            photonView.RPC("RPC_SpawnItems", RpcTarget.AllBuffered, itemNum);

        }

    }
    [PunRPC]
    private void AllItemOff()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            itemSpawnctrl = spawnPoint.GetComponent<ItemSpawnctrl>();

            itemSpawnctrl.WhistleItem.SetActive(false);
            itemSpawnctrl.GunItem.SetActive(false);
            itemSpawnctrl.StoneItem.SetActive(false);
        }
    }

    [PunRPC]
    private void RPC_SpawnItems(int itemNum)
    {
        foreach (var spawnPoint in spawnPoints)
        {
            itemSpawnctrl = spawnPoint.GetComponent<ItemSpawnctrl>();

            itemSpawnctrl.WhistleItem.SetActive(false);
            itemSpawnctrl.GunItem.SetActive(false);
            itemSpawnctrl.StoneItem.SetActive(false);

            itemSpawnctrl.IsAllItemOff = false;
        }

        foreach (var spawnPoint in spawnPoints)
        {
            itemSpawnctrl = spawnPoint.GetComponent<ItemSpawnctrl>();

            if (itemSpawnctrl != null)
            {
                itemSpawnctrl.ItemSpawn(itemNum);
                itemNum++;
            }
        }
    }


}
