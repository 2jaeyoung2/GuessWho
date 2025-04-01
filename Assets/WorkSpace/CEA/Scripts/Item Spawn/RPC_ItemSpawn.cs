using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPC_ItemSpawn : MonoBehaviour
{
    [PunRPC]
    private void SetItem(int itemNum, string pointName)
    {
        GameObject.Find(pointName).GetComponent<ItemSpawnctrl>().ItemSpawn(itemNum);
        Debug.Log("리스폰테스트");
    }

    [PunRPC]
    private void NoticeIsAllitemOff(string parentName)
    {
        GameObject.Find(parentName).GetComponent<ItemSpawnctrl>().IsAllItemOff = true;
        //myParent.IsAllItemOff = true;
        Debug.Log("RPC테스트");
    }

    [PunRPC]
    private void PlayerItemGet(string parentName, int itemNum)
    {
        ItemSpawnctrl parent = GameObject.Find(parentName).GetComponent<ItemSpawnctrl>();

        switch (itemNum)
        {
            case 1:
                parent.StoneItem.SetActive(false);
                break;

            case 2:
                parent.GunItem.SetActive(false);
                break;

            case 3:
                parent.WhistleItem.SetActive(false);
                break;
        }

    }
}
