using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class TestingNPC : NPC,IHittable
{

    //���� ����
    //[SerializeField] GameObject tempDestination;// ���� ������ ���� �ý��� ����� ���� ����ϴ� �ӽ� ����
    [SerializeField] NavMeshAgent selfAgent;
    [SerializeField] Collider selfCollider;
    private INPCState nowState;
    //private NavMeshSurface gamefield;
    private bool haveToChangeState;//����μ��� �ʿ� ������ ���ӸŴ����� ����� ���� ������ ���⿡ ������ ����
    private bool hasHit = false;
    private float hitPenaltyTime=0.15f;
    [SerializeField] float hitTime = 0;
    //public GameObject forTest;//������ ����׿� ����ǰ�� �ʿ� ����
    public Animator animator;
    
    public NavMeshAgent SelfAgent { get { return selfAgent; } set { selfAgent = value; } }



    void Start()
    {
        //selfAgent.SetDestination(tempDestination.transform.position);//���� ������ ���� �ý��� ����� ���� ����ϴ� �ӽ� �ڵ� 

       // Debug.Log("npcStart");
        haveToChangeState = false;
        #region ����׿� �ڵ�
        //nowState = new NPCMove();
        //(nowState as NPCMove).SetDestination(NPCManager.ReturnRandomDestination());
        //nowState.EnterState(this);
        //nowState.StateAction();
        //Instantiate(forTest, (nowState as NPCMove).ReturnDestination(),Quaternion.identity);
        //StartCoroutine(CheckState());
        #endregion
        #region ���� ��� �ڵ� 
        //ȣ��Ʈ���� �����ؼ� ȣ��Ʈ�� ��쿡�� �۵�
        if (PhotonNetwork.IsConnected == false)
        {
            if (Random.Range(0f, 1f) < 0.5f)//�Ϻδ� �ٷ� �̵� �Ϻδ� ��� 
            {
                ChangeState(new NPCIdle());
                //nowState=;
                //nowState.EnterState(this);
                //Debug.Log("setidle");
            }
            else
            {
                ChangeState(new NPCMove());
                //nowState = new NPCMove();
                //nowState.EnterState(this);
                // Debug.Log("setmove");
            }
            StartCoroutine(CheckState());
        }
        else
        {
            if (PhotonNetwork.IsMasterClient == true)
            {
                if (Random.Range(0f, 1f) < 0.5f)//�Ϻδ� �ٷ� �̵� �Ϻδ� ��� 
                {
                    ChangeState(new NPCIdle());
                    //nowState=;
                    //nowState.EnterState(this);
                    //Debug.Log("setidle");
                }
                else
                {
                    ChangeState(new NPCMove());
                    //nowState = new NPCMove();
                    //nowState.EnterState(this);
                    // Debug.Log("setmove");
                }
                StartCoroutine(CheckState());
            }

        }
        
        #endregion
        //   selfAgent.
    }

    // Update is called once per frame
    //void Update()
    //{
    //    
    //}
    //private void FixedUpdate()
    //{
    //    //if(nowState != null)
    //    //{
    //    //    nowState.StateAction();
    //    //}
    //}
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
                if(nowState is NPCHit)
                {
                    hitTime += Time.deltaTime;
                    if (hitPenaltyTime <= hitTime)
                    {
                        selfCollider.enabled = true;
                        hitTime = 0;
                        (nowState as NPCHit).StopAnimation();
                        ChangeState(new NPCIdle());
                    }
                    
                }


                if (nowState.CheckStateEnd() == true)
                {
                    haveToChangeState=true;
                    if(nowState is NPCIdle)
                    {
                        ChangeState(new NPCMove());
                        //if (Random.Range(0, 100) < 70)
                        //{
                        //    //Debug.Log("changetomove");
                        //    ChangeState(new NPCMove());
                        //    
                        //}
                        //else
                        //{
                        //    //Debug.Log("stayidle");
                        //    ChangeState(new NPCIdle());
                        //}
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

    
    public bool CheckNPCPlacedRight()//�����س����� ��������� �ʴ��� 
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

    //private void OnTriggerEnter(Collider other)//�׽�Ʈ�� �÷��̾�� ������ hit ���
    //{
    //    if (other.transform.tag == "Player")
    //    {
    //        GetHit();
    //    }
    //}
    //public void HitByPlayer()
    //{
    //    GetHit();
    //}
    public void GetHit()//puncallback�ؾ� ��-> �ִϸ��̼� �� �����̼��� �����ؼ� �÷��̾����� ���� ȭ���� �� 
    {
        selfCollider.enabled = false;


        if (PhotonNetwork.IsConnected == false)
        {
            ChangeState(new NPCHit());//���� �Ⱦ� �� ����ϴ� ��
            //Debug.Log("hitmethod");
        }
        else
        {
            photonView.RPC("ChangeState", Photon.Pun.RpcTarget.MasterClient, new NPCHit());//������ �� �ݹ����� ������ Ŭ���̾�Ʈ�� ������
        }


        //Debug.Log("gethit");
        haveToChangeState = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            var player = other.transform.root.transform;//�÷��̾���� ���� �Ͼ �ÿ��� �ڵ� ���� �ؾ� ��
            //Debug.Log("detectplayer");
            transform.LookAt(player);


        }
    }


}
