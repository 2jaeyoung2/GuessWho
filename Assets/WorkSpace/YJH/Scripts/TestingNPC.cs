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

    
    
    [SerializeField] NavMeshAgent selfAgent;
    [SerializeField] Collider selfCollider;
    [SerializeField] AudioSource hitSoundSource;
    
    private INPCState nowState;
    
    private float hitPenaltyTime=0.3f;
    [SerializeField] float hitTime = 0;
    
    public Animator animator;
   
    private Vector3 npcDestination=new Vector3();
    readonly int hashselfVel = Animator.StringToHash("selfVel");
    public NavMeshAgent SelfAgent
    {
        get { return selfAgent; }
        set { selfAgent = value; }
    }

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
            
            StartCoroutine (NPCAnimationControl());
            if (PhotonNetwork.IsMasterClient == true)
            {
                if (Random.Range(0f, 1f) < 0.5f)//�Ϻδ� �ٷ� �̵� �Ϻδ� ��� 
                {
                    photonView.RPC("ChangeState", Photon.Pun.RpcTarget.All, NPCStateName.Idle, UnityEngine.Random.Range(0.2f, 1.0f));
                    
                }
                else
                {
                    photonView.RPC("ChangeState", Photon.Pun.RpcTarget.All, NPCStateName.Walk, 0.0f);
                    
                }
                StartCoroutine(CheckState());
            }
            

        }
    }


    
    #region ���º�ȭ ���� �ڵ�
    
    [PunRPC]
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
            case NPCStateName.Dead:
                nowState = new NPCDead();
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

            case NPCStateName.Dead:
                nowState = new NPCDead();
                nowState.EnterState(this,0);
                nowState.StateAction(); 
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
                    
                    
                }else if (nowState.CheckStateEnd() == true)
                {
                    
                    if(nowState is NPCIdle)
                    {
                        photonView.RPC("ChangeState", Photon.Pun.RpcTarget.All, NPCStateName.Walk, 0f);


                    }
                    else if(nowState is NPCMove)
                    {
                        
                        if (Random.Range(0, 100) < 70)
                        {
                            
                            photonView.RPC("ChangeState", Photon.Pun.RpcTarget.All, NPCStateName.Idle,0.3f);
                            

                        }
                        else
                        {
                            
                            photonView.RPC("ChangeState", Photon.Pun.RpcTarget.All, NPCStateName.Idle, UnityEngine.Random.Range(0.8f, 1.5f));
                            
                        }
                    }
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
            
        }
        else
        {
            photonView.RPC("ChangeState", Photon.Pun.RpcTarget.All, NPCStateName.Hit);//������ �� �ݹ����� ������ Ŭ���̾�Ʈ�� ������
        }

    }
    public void GetDie()
    {
        hitSoundSource.Play();
        photonView.RPC("ChangeState", Photon.Pun.RpcTarget.All, NPCStateName.Dead);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            selfCollider.enabled = false;
            
            var player = other.transform.root.transform;//�÷��̾���� ���� �Ͼ �ÿ��� �ڵ� ���� �ؾ� ��
            
            transform.LookAt(player);
            


        }
        else if(other.transform.root.tag == "Stone")
        {
            var stone = other.transform.root.transform;
            transform.LookAt(stone);
            GetHit();

        }else if(other.transform.root.tag == "Bullet")
        {
            var stone = other.transform.root.transform;
            transform.LookAt(stone);
            GetDie();
        }
    }

    #endregion
    
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        
        if(PhotonNetwork.IsMasterClient)
        {
            SelfAgent.enabled = true;
            StartCoroutine(CheckState());
            if(nowState is NPCMove)
            {
                (nowState as NPCMove).EnterState(this, npcDestination);
                nowState.StateAction();
               

            }
        }




    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       
    }
}
