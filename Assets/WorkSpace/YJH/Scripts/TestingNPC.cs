using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using ZL.Unity;

public class TestingNPC : MonoBehaviourPunCallbacks,IHittable,IPunObservable
{

    //���� ����
    //[SerializeField] GameObject tempDestination;// ���� ������ ���� �ý��� ����� ���� ����ϴ� �ӽ� ����
    [SerializeField] NavMeshAgent selfAgent;
    [SerializeField] Collider selfCollider;
    [SerializeField] AudioSource hitSoundSource;
    
    private INPCState nowState;
    //private NavMeshSurface gamefield;
    //private bool haveToChangeState;//����μ��� �ʿ� ������ ���ӸŴ����� ����� ���� ������ ���⿡ ������ ����
    //private bool hasHit = false;
    private float hitPenaltyTime=0.3f;
    [SerializeField] float hitTime = 0;
    //public GameObject forTest;//������ ����׿� ����ǰ�� �ʿ� ����
    public Animator animator;
    //public string forDebug;
    private Vector3 npcDestination=new Vector3();
    readonly int hashselfVel = Animator.StringToHash("selfVel");
    public NavMeshAgent SelfAgent { get { return selfAgent; } set { selfAgent = value; } }
    public INPCState NowState
    {
        get { return nowState; }
        set { nowState = value; }
    }
    public Collider SelfCollider
    {
        get { return selfCollider; }
    }

    

    public override void OnEnable()
    {
        base.OnEnable();
        if (PhotonNetwork.IsMasterClient == false)
        {
            //selfAgent.enabled = false;
            
        }
    }
    public void SetNPCTransform(Vector3 transformTo)
    {
        transform.position=transformTo;
    }


    public void InitialSet()
    {
        if (PhotonNetwork.IsConnected == false)
        {
            StartCoroutine(CheckState());
            return;
            
        }
        else
        {
            //Debug.Log("conn");
            StartCoroutine (NPCAnimationControl());
            if (PhotonNetwork.IsMasterClient == true)
            {
                if (Random.Range(0f, 1f) < 0.5f)//�Ϻδ� �ٷ� �̵� �Ϻδ� ��� 
                {
                    photonView.RPC("ChangeState", Photon.Pun.RpcTarget.All, NPCStateName.Idle, UnityEngine.Random.Range(0.2f, 1.0f));
                    //nowState=;
                    //nowState.EnterState(this);
                    //Debug.Log("setidle");
                }
                else
                {
                    photonView.RPC("ChangeState", Photon.Pun.RpcTarget.All, NPCStateName.Walk, 0.0f);
                    //nowState = new NPCMove();
                    //nowState.EnterState(this);
                    // Debug.Log("setmove");
                }
                StartCoroutine(CheckState());
            }
            else
            {
                //selfAgent.enabled = false;
            }

        }
    }


    
    #region ���º�ȭ ���� �ڵ�
    
    [PunRPC]
    public void ChangeState(NPCStateName stateName)
    {
        //forDebug=stateName.ToString();
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
    
    [PunRPC]
    public void ChangeState(NPCStateName stateName, float time)//RPC��
    {
        //forDebug = stateName.ToString();
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
                nowState.EnterState(this,time);
                nowState.StateAction();
                
                break;
            case NPCStateName.Walk:
                nowState = new NPCMove();
                
                (nowState as NPCMove).EnterState(this, ISingleton<NPCManager>.Instance.RandomDestination(this));
                nowState.StateAction();
                npcDestination = (nowState as NPCMove).ReturnDestination();
                break;
            default:
                break;
        }
        PhotonNetwork.SendAllOutgoingCommands();


    }
    public void AfterHit()
    {
        if (nowState is NPCHit)
        {
            (nowState as NPCHit).StopAnimation();
        }
        else
        {
            return;
        }
        photonView.RPC("ChangeState", Photon.Pun.RpcTarget.All, NPCStateName.Idle, UnityEngine.Random.Range(0.2f, 1.0f));
    }


    IEnumerator NPCAnimationControl()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            animator.SetFloat(hashselfVel, selfAgent.velocity.magnitude);
        }
    }

    


    IEnumerator CheckState()//master client�� �̰� �����ؾ� ��
    {
        

        while (true)
        {
            
            yield return new WaitForSeconds(0.1f);
            if (PhotonNetwork.IsMasterClient == false)
            {

            }
            else if (nowState != null)
            {
                if(nowState is NPCHit)
                {
                    //hitTime += Time.deltaTime;
                    //if (hitPenaltyTime <= hitTime)
                    //{
                    //    
                    //    (nowState as NPCHit).StopAnimation();
                    //    photonView.RPC("ChangeState", Photon.Pun.RpcTarget.All, NPCStateName.Idle, UnityEngine.Random.Range(0.2f, 1.0f));
                    //    //ChangeState(NPCStateName.Idle);//new NPCIdle());
                    //    selfCollider.enabled = true;
                    //    hitTime = 0;
                    //}
                    
                }else if (nowState.CheckStateEnd() == true)
                {
                    //haveToChangeState=true;
                    if(nowState is NPCIdle)
                    {
                        photonView.RPC("ChangeState", Photon.Pun.RpcTarget.All, NPCStateName.Walk, 0f);

                        //ChangeState(NPCStateName.Walk);//new NPCMove());
                        

                    }
                    else if(nowState is NPCMove)
                    {
                        
                        if (Random.Range(0, 100) < 70)
                        {
                            //Debug.Log("changetomove");
                            photonView.RPC("ChangeState", Photon.Pun.RpcTarget.All, NPCStateName.Idle,0.3f);
                            //Debug.Log(photonView.ViewID);
                            //ChangeState(NPCStateName.Idle,true);
                            
                            //ChangeState(NPCStateName.Walk);

                        }
                        else
                        {
                            //Debug.Log("stayidle");
                            photonView.RPC("ChangeState", Photon.Pun.RpcTarget.All, NPCStateName.Idle, UnityEngine.Random.Range(0.8f, 1.5f));
                            //ChangeState(NPCStateName.Idle);
                        }
                    }
                }
                else
                {
                    //haveToChangeState=false;
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
    public void GetHit()//puncallback�ؾ� ��-> �ִϸ��̼� �� �����̼��� �����ؼ� �÷��̾����� ���� ȭ���� �� ->clear
    {
        hitSoundSource.Play();
        


        if (PhotonNetwork.IsConnected == false)
        {
            ChangeState(NPCStateName.Hit);//���� �Ⱦ� �� ����ϴ� ��
            //Debug.Log("hitmethod");
        }
        else
        {
            photonView.RPC("ChangeState", Photon.Pun.RpcTarget.All, NPCStateName.Hit);//������ �� �ݹ����� ������ Ŭ���̾�Ʈ�� ������
        }


        //Debug.Log("gethit");
        //haveToChangeState = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            selfCollider.enabled = false;
            Debug.Log("hit");
            var player = other.transform.root.transform;//�÷��̾���� ���� �Ͼ �ÿ��� �ڵ� ���� �ؾ� ��
            //Debug.Log("detectplayer");
            transform.LookAt(player);
            


        }
        else if(other.transform.root.tag == "Stone")
        {
            var stone = other.transform.root.transform;
            transform.LookAt(stone);
            GetHit();

        }
    }

    #endregion
    
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("master changed");
        //base.OnMasterClientSwitched(newMasterClient);
        if(PhotonNetwork.IsMasterClient)
        {
            SelfAgent.enabled = true;
            StartCoroutine(CheckState());
            if(nowState is NPCMove)
            {
                (nowState as NPCMove).EnterState(this, npcDestination);
                nowState.StateAction();
                //(nowState as NPCMove).SetDestination(npcDestination);

            }
        }




    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)
        //{
        //    stream.SendNext(transform.position);
        //    stream.SendNext(transform.rotation);
        //}
        //else
        //{
        //    transform.position=(Vector3)stream.ReceiveNext();
        //    transform.rotation=(Quaternion)stream.ReceiveNext();
        //}
    }
}
