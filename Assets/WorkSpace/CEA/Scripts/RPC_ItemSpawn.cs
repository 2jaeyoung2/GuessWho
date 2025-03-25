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
        Debug.Log("�������׽�Ʈ");
    }

    [PunRPC]
    private void NoticeIsAllitemOff(string parentName)
    {
        GameObject.Find(parentName).GetComponent<ItemSpawnctrl>().IsAllItemOff = true;
        //myParent.IsAllItemOff = true;
        Debug.Log("RPC�׽�Ʈ");
    }

    [PunRPC]
    private void PlayerItemGet(string parentName, ItemData itemData)
    {
        ItemSpawnctrl parent = GameObject.Find(parentName).GetComponent<ItemSpawnctrl>();

        switch (itemData.itemType)
        {
            case ItemType.Stone:
                parent.StoneItem.SetActive(false);
                break;

            case ItemType.Gun:
                parent.GunItem.SetActive(false);
                break;

            case ItemType.Whistle:
                parent.WhistleItem.SetActive(false);
                break;
        }

    }
}
