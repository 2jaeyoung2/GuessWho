using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class NPCManager : MonoBehaviour
{
    // Start is called before the first frame update
    //�ʵ忡 �����ϴ� ���ǽõ� 
    //public GameObject npcPrefab;
    private NPCPool pool;
    [SerializeField] GameObject npc;
    void Start()
    {
        pool = new NPCPool();//Ǯ ����
        pool.SetPrefab(npc);
        //pool.NPCS.Get();//NPC ����

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void InitialSet()//�� ���½� NPC �Ѹ��� 
    {
        
    }
    
    public void SetNPCTransform()
    {

    }
    public void SpawNPC()
    {
        pool.NPCS.Get();
    }
    public static Vector3 ReturnRandomDestination()
    {
        Vector3 destination;
        destination = new Vector3(Random.Range(1,100),0, Random.Range(1, 100));
        return destination;
    }

    




}
