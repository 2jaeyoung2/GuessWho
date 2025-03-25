using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
public class ItemSpawnctrl : MonoBehaviour
{
    [SerializeField]
    private GameObject gunItem;

    [SerializeField]
    private GameObject whistleItem;

    [SerializeField]
    private GameObject stoneItem;

    [SerializeField]
    [Header("�����")] private PhotonView photonView;

    private bool isAllItemOff = false;

    public bool IsAllItemOff 
    {
        get { return isAllItemOff; }
        set { isAllItemOff = value; }
    }

    private float respawnCoolTime = 20.0f;
    private float respawnElapsedTime;

    private void Update()
    {
        if(isAllItemOff == true)
        {
            respawnElapsedTime += Time.deltaTime;
            //Debug.Log("������ ������ �����: " + respawnElapsedTime);

            if (respawnCoolTime < respawnElapsedTime)
            {

                isAllItemOff = false;
                respawnElapsedTime = 0;
            }
        }
    }

    //�ڽ� ��ü�� ��� �����ִ��� Ȯ���ϴ� �޼���
    public bool AllItemDisabled(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    //[PunRPC]
    //public void PickRandomItemNum()
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        RandomItemNum = Random.Range(1, 11);
    //    }
    //}

    private void SetRandomNumber()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            int RandomNumber = Random.Range(1, 11);
            photonView.RPC("SetItem", RpcTarget.All, RandomNumber);
        }
    }

    [PunRPC]
    private void SetItem(int itemNum)
    {
        ItemSpawn(itemNum);
    }


    public void ItemSpawn(int itemNum) //�������� �����ϴ� �Լ�
    {
        switch (itemNum)
        {
            case 1:
                gunItem.SetActive(true);
                break;

            case 2:
                whistleItem.SetActive(true);
                break;

            default:
                stoneItem.SetActive(true);
                break;
        }
    }
}
