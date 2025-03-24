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
                ChangeState(NPCStateName.Idle);
                //nowState=;
                //nowState.EnterState(this);
                //Debug.Log("setidle");
            }
            else
            {
                ChangeState(NPCStateName.Walk);
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
                    ChangeState(NPCStateName.Idle);//
                    //nowState=;
                    //nowState.EnterState(this);
                    //Debug.Log("setidle");
                }
                else
                {
                    ChangeState(NPCStateName.Walk);
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
    public override void OnEnable()
    {
        if (PhotonNetwork.IsConnected == false)
        {
            return;
            //if (Random.Range(0f, 1f) < 0.5f)//�Ϻδ� �ٷ� �̵� �Ϻδ� ��� 
            //{
            //    ChangeState(new NPCIdle());
            //    //nowState=;
            //    //nowState.EnterState(this);
            //    //Debug.Log("setidle");
            //}
            //else
            //{
            //    ChangeState(new NPCMove());
            //    //nowState = new NPCMove();
            //    //nowState.EnterState(this);
            //    // Debug.Log("setmove");
            //}
            //StartCoroutine(CheckState());
        }
        else
        {
            if (PhotonNetwork.IsMasterClient == true)
            {
                if (Random.Range(0f, 1f) < 0.5f)//�Ϻδ� �ٷ� �̵� �Ϻδ� ��� 
                {
                    ChangeState(NPCStateName.Idle);//
                    //nowState=;
                    //nowState.EnterState(this);
                    //Debug.Log("setidle");
                }
                else
                {
                    ChangeState(NPCStateName.Walk);
                    //nowState = new NPCMove();
                    //nowState.EnterState(this);
                    // Debug.Log("setmove");
                }
                StartCoroutine(CheckState());
            }

        }
    }
    private void OnDisable()
    {
        

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
    #region ���º�ȭ ���� �ڵ�
    [PunRPC]
    //public void ChangeState(INPCState changeState)
    //{
    //    
    //    nowState = changeState;
    //    nowState.EnterState(this);
    //    nowState.StateAction();
    //   
    //}

    public void ChangeState(NPCStateName stateName)
    {
        switch (stateName)
        {
            case NPCStateName.None:
                break;
            case NPCStateName.Hit:
                nowState = new NPCHit();
                nowState.EnterState(this);
                nowState.StateAction();
                break;
            case NPCStateName.Idle:
                nowState = new NPCIdle();
                nowState.EnterState(this);
                nowState.StateAction();
                break;
            case NPCStateName.Walk:
                nowState = new NPCMove();
                nowState.EnterState(this);
                nowState.StateAction();
                break;
            default:
                break;
        }


        
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
                        ChangeState(NPCStateName.Idle);//new NPCIdle());
                    }
                    
                }


                if (nowState.CheckStateEnd() == true)
                {
                    haveToChangeState=true;
                    if(nowState is NPCIdle)
                    {
                        ChangeState(NPCStateName.Walk);//new NPCMove());
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
                        ChangeState(NPCStateName.Idle);
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

    #endregion
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



    #region �ǰ� ���� �ڵ�
    [PunRPC]
    public void GetHit()//puncallback�ؾ� ��-> �ִϸ��̼� �� �����̼��� �����ؼ� �÷��̾����� ���� ȭ���� �� 
    {
        selfCollider.enabled = false;


        if (PhotonNetwork.IsConnected == false)
        {
            ChangeState(NPCStateName.Hit);//���� �Ⱦ� �� ����ϴ� ��
            //Debug.Log("hitmethod");
        }
        else
        {
            photonView.RPC("ChangeState", Photon.Pun.RpcTarget.MasterClient, NPCStateName.Hit);//������ �� �ݹ����� ������ Ŭ���̾�Ʈ�� ������
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

    #endregion
}
