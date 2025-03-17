using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class NPCManager : MonoBehaviour
{
    // Start is called before the first frame update
    //�ʵ忡 �����ϴ� ���ǽõ� 
    //public GameObject npcPrefab;
    private NPCPool pool;//NPC Ǯ
    [SerializeField] GameObject npc;// NPC ������
    [SerializeField] GameObject npcGroup;//������ npc���� ������ ���� �θ� ������Ʈ
    [SerializeField] GameObject spawnGroup;// ��������Ʈ�� ������ ���� �θ� ������Ʈ
    [SerializeField] List<GameObject> npcSpawnList;// �θ� ������Ʈ�κ��� ��������Ʈ�� ������ ����

    [SerializeField] float mapSizeZ1;//z�� �ּҰ�
    [SerializeField] float mapSizeZ2;//z�� �ִ밪
    [SerializeField] float mapSizeX1;//x�� �ּҰ�
    [SerializeField] float mapSizeX2;//x�� �ִ밪

    private List<GameObject> npcList;//������ NPC�� �����ϴ� ����Ʈ ���� ��1?
    private List<NPC> npcScriptList;//������ NPC�� �����ϴ� ����Ʈ ���� ��1?
    private List<Vector3> npcDestinations;//npc�� �������� ���� ��ġ�� ������ ����Ʈ
    public NavMeshModifierVolume temp;
    void Start()
    {
        Debug.Log(temp.size.x+","+ temp.size.y+","+ temp.size.z);


        pool = new NPCPool();//Ǯ ����
        npcList = new List<GameObject>();
        npcScriptList = new List<NPC>();
        npcSpawnList = new List<GameObject>();
        npcDestinations = new List<Vector3>();
        SetSpawnPoint();//�ʱ� ���� ��Ŀ���� -> ���߿� �ϼ����� ����ø��� �ٸ� ������ ����� ���� -> �������δ� �е��� ������ �� ������ ���� �� �ۿ� ����
        pool.SetPrefab(npc);
        //InitialSet();
        //InitialSetBySpawnPoint();
        InitialForDebug();
        //pool.NPCS.Get();//NPC ����

    }
    public void SetSpawnPoint()//�ϴ� ���� ����Ʈ�� �������� ���� ��ġ�� �����̱� ������ ��� X Ȥ�� �𸣴� ���ܳ��� ���߿� ����
    {
        for (int i = 0; i < spawnGroup.transform.childCount; i++)//���� ����Ʈ 
        {
            npcSpawnList.Add(spawnGroup.transform.GetChild(i).gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void InitialSetBySpawnPoint()//��������Ʈ�� �������� npc ��ġ 
    {
        CreateAllNPC();
        foreach (NPC npc in npcScriptList)//npc����
        {
            //Debug.Log(npcSpawnList.Count);
            SetNPCTransform(npc.gameObject, npcSpawnList[Random.Range(0, npcSpawnList.Count)].transform.position);//�����ϰ� ��ġ ����

        }
    }

    public void InitialForDebug()
    {
        SpawNPC();
        SetNPCTransform(npc.gameObject, npcSpawnList[Random.Range(0, npcSpawnList.Count)].transform.position);
    }




    public void InitialSet()//�� ���½� NPC �Ѹ��� 
    {
        CreateAllNPC();//���� ������ 50����ŭ NPC ȣ��
        foreach(NPC npc in npcScriptList)//npc����
        {
            SetNPCTransform(npc.gameObject, ReturnRandomDestination());//�����ϰ� ��ġ ����
        }


    }

    



    
    public void SetNPCTransform(GameObject npc, Vector3 position)//y ��ǥ 1.09
    {
        npc.transform.position = position;
    }
    public void SpawNPC()
    {
        pool.NPCS.Get();
    }
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
            npcScriptList.Add(pool.GetNPC(npcGroup).GetComponent<NPC>());
        }
    }
    




}
