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
    [Header("�����")] private PhotonView pointphotonView;

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
    }

    private void Update()
    {
        if(isAllItemOff == true)
        {
            respawnElapsedTime += Time.deltaTime;
            //Debug.Log("������ ������ �����: " + respawnElapsedTime);

            if (respawnCoolTime < respawnElapsedTime)
            {
                SetRandomNumber();
                isAllItemOff = false;
                respawnElapsedTime = 0;
            }
        }
    }

   

    private void SetRandomNumber()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            int RandomNumber = Random.Range(1, 11);
            photonView.RPC("SetItem", RpcTarget.AllBuffered, RandomNumber, this.name);
        }
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
