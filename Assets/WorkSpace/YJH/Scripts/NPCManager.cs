using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class NPCManager : MonoBehaviour
{
    // Start is called before the first frame update
    //�ʵ忡 �����ϴ� ���ǽõ� 
    //public GameObject npcPrefab;
    private NPCPool pool;//NPC Ǯ
    [SerializeField] GameObject npc;// NPC ������
    [SerializeField] GameObject npcGroup;
    [SerializeField] GameObject spawnGroup;
    [SerializeField] List<GameObject> npcSpawnList;
    private List<GameObject> npcList;//������ NPC�� �����ϴ� ����Ʈ
    private List<NPC> npcScriptList;
    
    void Start()
    {
        pool = new NPCPool();//Ǯ ����
        npcList = new List<GameObject>();
        npcScriptList = new List<NPC>();
        npcSpawnList = new List<GameObject>();
        SetSpawnPoint();//�ʱ� ���� ��Ŀ���� -> ���߿� �ϼ����� ����ø��� �ٸ� ������ ����� ���� -> �������δ� �е��� ������ �� ������ ���� �� �ۿ� ����
        pool.SetPrefab(npc);
        //InitialSet();
        InitialSetBySpawnPoint();
        
        //pool.NPCS.Get();//NPC ����

    }
    public void SetSpawnPoint()
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
    
    public void InitialSetBySpawnPoint()
    {
        CreateAllNPC();
        foreach (NPC npc in npcScriptList)//npc����
        {
            SetNPCTransform(npc.gameObject, npcSpawnList[Random.Range(0, npcSpawnList.Count)].transform.position);//�����ϰ� ��ġ ����

        }
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
        destination = new Vector3(Random.Range(1,100),1.5f, Random.Range(1, 100));
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
