using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class TestingNPC : NPC
{

    //���� ����
    [SerializeField] GameObject tempDestination;// ���� ������ ���� �ý��� ����� ���� ����ϴ� �ӽ� ����
    [SerializeField] NavMeshAgent selfAgent;
    private INPCState nowState;
    private NavMeshSurface gamefield;
    private bool haveToChangeState;//����μ��� �ʿ� ������ ���ӸŴ����� ����� ���� ������ ���⿡ ������ ����

    public GameObject forTest;
    // Start is called before the first frame update
    void Start()
    {
        //selfAgent.SetDestination(tempDestination.transform.position);//���� ������ ���� �ý��� ����� ���� ����ϴ� �ӽ� �ڵ� 

       // Debug.Log("npcStart");
        haveToChangeState = false;
        #region ����׿� �ڵ�
        nowState = new NPCMove();
        (nowState as NPCMove).SetDestination(NPCManager.ReturnRandomDestination());
        nowState.EnterState(this);
        nowState.StateAction();
        Instantiate(forTest, (nowState as NPCMove).ReturnDestination(),Quaternion.identity);
        StartCoroutine(CheckState());
        #endregion
        #region ���� ��� �ڵ� 
        //if (Random.Range(0f,1f) < 0.5f)//�Ϻδ� �ٷ� �̵� �Ϻδ� ��� 
        //{
        //    nowState=new NPCIdle();
        //    nowState.EnterState(this);
        //    Debug.Log("setidle");
        //}
        //else
        //{
        //    nowState = new NPCMove();
        //    nowState.EnterState(this);
        //    Debug.Log("setmove");
        //}
        //StartCoroutine(CheckState());
        #endregion
        //   selfAgent.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        //if(nowState != null)
        //{
        //    nowState.StateAction();
        //}
    }
    public void ChangeState(INPCState changeState)
    {
        
        nowState = changeState;
        nowState.EnterState(this);
        nowState.StateAction();
       
    }
    IEnumerator CheckState()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (nowState != null)
            {
                if (nowState.CheckStateEnd() == true)
                {
                    haveToChangeState=true;
                    if(nowState is NPCIdle)
                    {
                        if (Random.Range(0, 100) < 70)
                        {
                            //Debug.Log("changetomove");
                            ChangeState(new NPCMove());
                            
                        }
                        else
                        {
                            //Debug.Log("stayidle");
                            ChangeState(new NPCIdle());
                        }
                        haveToChangeState = false;

                    }
                    else if(nowState is NPCMove)
                    {
                        ChangeState(new NPCIdle());
                        //Debug.Log("changetoidle");
                        haveToChangeState = false;
                    }
                }
                else
                {
                    haveToChangeState=false;
                }

            }



        }
    }

    //IEnumerator CheckNPCPlacedRight()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(0.5f);
    //
    //
    //
    //    }
    //}

    public bool CheckNPCPlacedRight()
    {
        if(selfAgent.isOnNavMesh)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void HitByPlayer()
    {
        ChangeState(new NPCHit());
        haveToChangeState = false;
    }
    //public void SetNPCState()
    //{
    //
    //}
}
