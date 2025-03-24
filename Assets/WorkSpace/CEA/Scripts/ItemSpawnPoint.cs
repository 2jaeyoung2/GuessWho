using Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ItemSpawnPoint : MonoBehaviour
{
    [SerializeField]
    [Header("������ ���� ����Ʈ")] private GameObject[] spawnPoints;

    [SerializeField]
    [Header("�����")] private PhotonView photonView;
    
    private ItemSpawnctrl itemSpawnctrl;


    //��� ����Ʈ�� �Ѱ��ϴ� itemspawnpoints ������Ʈ�� ó�� ����Ǿ �� �޼���
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(PhotonNetwork.InRoom)
            {
                int itemNum = Random.Range(1, 11);
                photonView.RPC("RPC_SpawnItems", RpcTarget.All, itemNum);
            }
        }
    }

    [PunRPC]
    private void RPC_SpawnItems(int itemNum)
    {
        foreach(var spawnPoint in spawnPoints)
        {
            itemSpawnctrl = spawnPoint.GetComponent<ItemSpawnctrl>();

            if (itemSpawnctrl != null)
            {
                itemSpawnctrl.ItemSpawn(itemNum);
            }
        }
    }


}
