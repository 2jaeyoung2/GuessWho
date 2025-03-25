using Photon.Pun;
using Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempNetwork : MonoBehaviour
{
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        if (PhotonNetwork.IsConnected) //���� ����� ������ �Ǿ��ִٸ�
        {
            PhotonNetwork.JoinRandomRoom(); //������ �濡 ���� ����
            Debug.Log("���� ��");
        }
    }

    public void OnConnectedToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
