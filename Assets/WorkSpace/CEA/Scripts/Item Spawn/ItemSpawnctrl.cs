using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
public class ItemSpawnctrl : MonoBehaviourPun
{
    [SerializeField]
    private GameObject gunItem;

    public GameObject GunItem
    {
        get { return gunItem; }
    }

    [SerializeField]
    private GameObject whistleItem;

    public GameObject WhistleItem
    {
        get { return whistleItem; }
    }

    [SerializeField]
    private GameObject stoneItem;

    public GameObject StoneItem
    {
        get { return stoneItem; }
    }

    [SerializeField]
    [Header("포톤뷰")] private PhotonView pointphotonView;

    private bool isAllItemOff = false;

    public bool IsAllItemOff 
    {
        get { return isAllItemOff; }
        set { isAllItemOff = value; }
    }

    private float respawnCoolTime = 20.0f;
    private float respawnElapsedTime;

    private void Start()
    {
        pointphotonView = GetComponentInParent<PhotonView>();
        respawnElapsedTime = 0;
}

    private void Update()
    {
        if(isAllItemOff == true)
        {
            respawnElapsedTime += Time.deltaTime;
            //Debug.Log("아이템 리스폰 대기중: " + respawnElapsedTime);

            if (respawnCoolTime < respawnElapsedTime)
            {
                SetRandomItem();
                isAllItemOff = false;
                respawnElapsedTime = 0;
            }
        }
    }

   

    private void SetRandomItem()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            int RandomNumber = Random.Range(0, 10);
            photonView.RPC("SetItem", RpcTarget.AllBuffered, RandomNumber, this.name);
        }
    }
    

    public void ItemSpawn(int itemNum) //아이템을 스폰하는 함수
    {
        switch (itemNum)
        {
            case 8:
                gunItem.SetActive(true);
                break;

            case 9:
                whistleItem.SetActive(true);
                break;

            default:
                stoneItem.SetActive(true);
                break;
        }
    }

}
