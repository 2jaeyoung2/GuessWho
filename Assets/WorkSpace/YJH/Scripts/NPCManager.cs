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
    void Start()
    {
        pool = new NPCPool();//Ǯ ����
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void InitialSet()
    {
        
    }
    

    public static Vector3 ReturnRandomDestination()
    {
        Vector3 destination;
        destination = new Vector3(Random.Range(1,100),0, Random.Range(1, 100));
        return destination;
    }

    




}
