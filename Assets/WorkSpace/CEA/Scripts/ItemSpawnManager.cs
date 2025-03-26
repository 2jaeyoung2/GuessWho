using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class ItemSpawnManager : MonoBehaviourPunCallbacks
{
    IEnumerator Start()
    {
        yield return new WaitUntil(() => PhotonNetwork.InRoom);
        Debug.Log("�濡 ������");

        yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("������ ��������");
            PhotonNetwork.Instantiate("ItemSpawnPoints", Vector3.zero, Quaternion.identity);
        }
    }

}
