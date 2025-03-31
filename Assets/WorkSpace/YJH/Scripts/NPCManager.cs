using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using ZL.Unity;

public class NPCManager : MonoBehaviourPunCallbacks, IPunObservable, ISingleton<NPCManager>
{
    //public GameObject test;

    
    private NPCPool pool;//NPC Ǯ
    [SerializeField] GameObject npc;// NPC ������
    [SerializeField] GameObject npcGroup;//������ npc���� ������ ���� �θ� ������Ʈ
    [SerializeField] GameObject spawnGroup;// ��������Ʈ�� ������ ���� �θ� ������Ʈ
    [SerializeField] List<GameObject> npcSpawnList;// �θ� ������Ʈ�κ��� ��������Ʈ�� ������ ����

    //[SerializeField] float mapSizeZ1;//z�� �ּҰ�
    //[SerializeField] float mapSizeZ2;//z�� �ִ밪
    //[SerializeField] float mapSizeX1;//x�� �ּҰ�
    //[SerializeField] float mapSizeX2;//x�� �ִ밪

    //private List<GameObject> npcList;//������ NPC�� �����ϴ� ����Ʈ ���� ��1?
    private List<TestingNPC> npcScriptList;//������ NPC�� �����ϴ� ����Ʈ ���� ��1?
                                    



    //public GameObject temp;

    //public Transform[] forTestSpawnPoint;
    private void Awake()
    {
        ISingleton<NPCManager>.TrySetInstance(this);

        pool = new NPCPool();//Ǯ ����
        pool.SetPrefab(npc);
    }

    void Start()
    {
        //Debug.Log(temp.size.x+","+ temp.size.y+","+ temp.size.z);

        
        npcScriptList = new List<TestingNPC>();
        npcSpawnList = new List<GameObject>();
        //npcDestinations = new List<Vector3>();
        SetSpawnPoint();//�ʱ� ���� ��Ŀ���� -> ���߿� �ϼ����� ����ø��� �ٸ� ������ ����� ���� -> �������δ� �е��� ������ �� ������ ���� �� �ۿ� ����
        
        
        
        //InitialSetBySpawnPoint();// ��������Ʈ�� �ʱ� ����
        
        //InitialForDebug();// ����׿� �ϳ� ����
        
    }

    private void OnDestroy()
    {
        ISingleton<NPCManager>.Release(this);
    }

    public void SetSpawnPoint()
    {
        for (int i = 0; i < spawnGroup.transform.childCount; i++)//���� ����Ʈ 
        {
            npcSpawnList.Add(spawnGroup.transform.GetChild(i).gameObject);
        }
    }
    // Update is called once per frame
    //void Update()
    //{
    //    
    //}
    
    public void InitialSetBySpawnPoint()//��������Ʈ�� �������� npc ��ġ 
    {
        //PhotonNetwork.InstantiateRoomObject(test.name, Vector3.zero, Quaternion.identity, 0);
        CreateAllNPC();
        //Debug.Log(npcScriptList.Count);
        if (PhotonNetwork.IsConnected == false)//���濡 ����Ǿ� ���� �ʴٸ�
        {
            foreach (TestingNPC npc in npcScriptList)//npc����
            {

                
                int spawnIndex = Random.Range(0, npcSpawnList.Count);

                //npc.transform.position = npcSpawnList[spawnIndex].transform.position*2;
                (npc as TestingNPC).SelfAgent.enabled = false;
                SetNPCTransform(npc.gameObject, npcSpawnList[spawnIndex].transform.position + new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2)));//�����ϰ� ��ġ ����
                npc.gameObject.transform.Rotate(0, Random.Range(0f, 360f), 0);
                (npc as TestingNPC).SelfAgent.enabled = true;
                (npc as TestingNPC).InitialSet();
                //Quaternion ransRoation= Quaternion
                //npc ���� ��ġ�� �����̼ǵ� ȸ��       
                

                //npc.transform.Translate(new Vector3(npcSpawnList[spawnIndex].transform.position.x, 3.0f, npcSpawnList[spawnIndex].transform.position.z));


            }
        }
        else//���濡 ����Ǿ� ������
        {
            if (PhotonNetwork.IsMasterClient != true)
            {
                foreach (TestingNPC npc in npcScriptList)
                {
                    (npc as TestingNPC).SelfAgent.enabled = false;
                }
                    //Debug.Log("notmaster");
                    //CreateAllNPC();
                    //return;
            }
            else
            {
                
                Debug.Log(npcScriptList.Count);
                foreach (TestingNPC npc in npcScriptList)//npc����
                {
                    //Debug.Log("1");
                    //CreateAllNPC();
                    int spawnIndex = Random.Range(0, npcSpawnList.Count);

                    //npc.transform.position = npcSpawnList[spawnIndex].transform.position*2;
                    (npc as TestingNPC).SelfAgent.enabled = false;
                    int tempID = npc.photonView.ViewID;
                    Vector3 temp = new Vector3();
                    temp = npcSpawnList[spawnIndex].transform.position +new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2));
                    photonView.RPC("SetNPCTransformByID", Photon.Pun.RpcTarget.All, tempID, temp);
                    //photonView.RPC("AddNPCListByPhotonID", Photon.Pun.RpcTarget.All, tempID);
                    //SetNPCTransform(npc.gameObject, npcSpawnList[spawnIndex].transform.position + new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2)));//�����ϰ� ��ġ ����
                    npc.gameObject.transform.Rotate(0, Random.Range(0f, 360f), 0);                                                                                            
                    (npc as TestingNPC).SelfAgent.enabled = true;
                    (npc as TestingNPC).InitialSet();
                    //npc.transform.Translate(new Vector3(npcSpawnList[spawnIndex].transform.position.x, 3.0f, npcSpawnList[spawnIndex].transform.position.z));


                }
            }
        }

        PhotonNetwork.SendAllOutgoingCommands();
    }

    //public void InitialForDebug()
    //{
    //    SpawNPC();
    //    SetNPCTransform(temp, new Vector3(0, 1.5f, 0));
    //    //SetNPCTransform(npc.gameObject, npcSpawnList[Random.Range(0, npcSpawnList.Count)].transform.position);
    //    //SetNPCTransform(npc.gameObject, new Vector3(0, 1.5f, 0));
    //    
    //}


    [PunRPC]// RPC�� NPC�� �� ���༭ ���ϰ� �ٲ����...
    public void SetNPCTransform(GameObject npc, Vector3 position)//y ��ǥ 1.09// �۵��� �ϴµ� ���ϴ� ��ġ���� �̵����� �ʰ� �߰��� ���ߴ� ���� �߻� ���� �ϴµ� Agent �����ΰ�? 
    {
        //Vector3 temp = new Vector3(position.x, 1.5f, position.z);
        npc.transform.position= new Vector3(position.x, 3.0f, position.z);
    }
    [PunRPC]
    public void SetNPCTransformByID(int viewID,Vector3 position)
    {
        PhotonView.Find(viewID).transform.position = position;
    }
    [PunRPC]
    public void AddNPCListByPhotonID(int viewID)
    {
        var tempNPC=PhotonView.Find(viewID);
        Debug.Log(npcGroup.transform);
        tempNPC.gameObject.transform.SetParent(npcGroup.transform);
        npcScriptList.Add(tempNPC.GetComponent<TestingNPC>());

    }
    //public void SpawNPC()
    //{
    //   temp= pool.NPCS.Get();
    //}
    public static Vector3 ReturnRandomDestination()
    {
        Vector3 destination;
        destination = new Vector3(Random.Range(-74,72),1.5f, Random.Range(-78, 74));//���� ���� ũ�⸦ �����ؼ� ���Խ�Ŵ static �Լ��� �����߱� ������ �̷��� ������ �ʿ��ϴٸ� npcmanager�� static class�� �����ϴ��� �̱��� �������� �����ϸ� �ɵ�
        //Debug.Log(mapSi);
        return destination;
    }
    
    public void CreateAllNPC()//npc�� �ʱ� ���ڸ�ŭ ����
    {
        for(int i=0; i<pool.InitialNPCNum; i++)
        {
            //npcList.Add(pool.NPCS.Get());
            if (PhotonNetwork.IsConnected)
            {
                



                if (PhotonNetwork.IsMasterClient == true)
                {
                    
                    
                    var tempNPC = PhotonNetwork.InstantiateRoomObject(npc.name, Vector3.zero, Quaternion.identity, 0);
                    tempNPC.transform.SetParent(npcGroup.transform);
                    int tempID=tempNPC.GetComponent<PhotonView>().ViewID;
                    //Debug.Log(tempID);
                    photonView.RPC("AddNPCListByPhotonID", Photon.Pun.RpcTarget.AllBuffered, tempID);
                    
                }
                else
                {
                    return;
                }
            }
            else
            {
                var tempNPC = pool.GetNPC(npcGroup);
                npcScriptList.Add(tempNPC.GetComponent<TestingNPC>());
            }
            
        }
    }

    public Vector3 RandomDestination(TestingNPC nowNPC)
    {
        
        Vector3 destination = new Vector3();
        var init = Random.insideUnitCircle;
        destination = nowNPC.transform.position + new Vector3(init.x * 20, 0, init.y * 20);
        while (IsDestinationOutOfRange(destination,nowNPC) == true)
        {
            
            var temp = Random.insideUnitCircle;
            destination = nowNPC.transform.position + new Vector3(temp.x * 20, 0, temp.y * 20);
        }
        
        return destination;
    }
    



    public bool IsDestinationOutOfRange(Vector3 destination,TestingNPC nowNPC)
    {
        if ((destination - nowNPC.transform.position).magnitude < 5f)
        {
            return true;
        }
        if (destination.x < -74 || destination.x > 72)
        {
            return true;
        }
        else if (destination.z > 74 || destination.z < -78)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(PhotonNetwork.IsConnected)
        {
            if(stream.IsWriting)
            {

            }
            else
            {

            }
        }
    }

    

}
