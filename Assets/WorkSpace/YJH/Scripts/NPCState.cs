using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon;
using Photon.Pun;
using Unity.VisualScripting;


public enum NPCStateName
{
    Hit,Idle,Walk,None

}
public interface INPCState
{
    
    public void PlayAnimation();
    public void StopAnimation();
    public void EnterState(TestingNPC npc);
    public void EnterState(TestingNPC npc,float time);

    public void StateAction();
    public bool CheckStateEnd();
    public bool ForceStateEnd();
}

public class NPCMove : INPCState
{
    //private List<Vector3> destinations;//navmesh �Ⱦ��� 
    //private bool isMoveEnd;
    private Vector3 destination;
    private TestingNPC nowNPC;
    private NavMeshAgent selfAgent;
    private Rigidbody npcRb;
    //float temp = 0;
    private Animator npcAnimator;
    readonly int hashHit = Animator.StringToHash("isHit");
    readonly int hashMove = Animator.StringToHash("isMove");
    readonly int hashIdle = Animator.StringToHash("isIdle");
    readonly int hashselfVel = Animator.StringToHash("selfVel");


    private float maxDelayTime = 1000f;
    private float delayTime = 0;
    //GameObject forTest;
    [PunRPC]
    public void PlayAnimation()
    {
        //animator����
        
        npcAnimator.SetBool(hashIdle, false);
        npcAnimator.SetBool(hashMove, true);
        npcAnimator.SetFloat(hashselfVel, 1f);
        
    }
    [PunRPC]
    public void StopAnimation()
    {
        //animator����
        npcAnimator.SetBool(hashIdle, true);
        npcAnimator.SetBool(hashMove, false);
        npcAnimator.SetFloat(hashselfVel, 0f);
        
            selfAgent.isStopped = true;
        

    }
    [PunRPC]
    public void EnterState(TestingNPC npc)
    {

        nowNPC = npc;
        selfAgent = npc.gameObject.GetComponent<NavMeshAgent>();
        selfAgent.isStopped = false;
        npcAnimator = (npc as TestingNPC).animator;
        destination = new Vector3();
        delayTime = 0;
        RandomDestination();
        //selfAgent.SetDestination(destination.position);//������ ���� 


        //temp+= Time.deltaTime;//�۵���! ���̵�!

        //destination = NPCManager.ReturnRandomDestination(nowNPC);//npc �Ŵ����� �����ϴ� ���� ��ǥ ���� �Լ��� ���, ���� ����ó�� �ȵǾ� ����-> transform ���� �����ϸ� ������ ������ ����3 �ؼ� ���ߴ°� �� ����


    }
    public void EnterState(TestingNPC npc, float time)
    {
        
            
        
        selfAgent.enabled = true;
        nowNPC = npc;
        selfAgent = npc.gameObject.GetComponent<NavMeshAgent>();
        selfAgent.isStopped = false;
        nowNPC.SelfCollider.enabled = true;
        npcAnimator = (npc as TestingNPC).animator;
        destination = new Vector3();
        delayTime = 0;
        RandomDestination();
        
        
    }
    public void EnterState(TestingNPC npc, Vector3 randomDestination)//RPC������ �����ε�
    {
        
            nowNPC = npc;
            selfAgent = npc.gameObject.GetComponent<NavMeshAgent>();
        //if (PhotonNetwork.IsMasterClient == true)
        //{
        //    selfAgent.isStopped = false;
        //}
        selfAgent.isStopped = false;
        npcAnimator = (npc as TestingNPC).animator;
            destination = new Vector3();
            destination = randomDestination;
            delayTime = 0;
        
        //RandomDestination();
    }
    private void RandomDestination()
    {
        //Debug.Log("first"+destination);
        //Debug.Log("NPC"+nowNPC.transform.position);
        var init = Random.insideUnitCircle;
        destination = nowNPC.transform.position + new Vector3(init.x * 20, 0, init.y * 20);
        while (IsDestinationOutOfRange() == true)
        {
            //Debug.Log("re");
            var temp = Random.insideUnitCircle;
            destination = nowNPC.transform.position + new Vector3(temp.x * 20, 0, temp.y * 20);
        }
        //Debug.Log(destination);

    }
    //Random.Range(-74,72),1.5f, Random.Range(-78, 74)
    public bool IsDestinationOutOfRange()
    {
        if ((destination-nowNPC.transform.position).magnitude<5f)
        {
            return true;
        }
        if (destination.x < -74 || destination.x > 72)
        {
            return true;
        }else if (destination.z>74||destination.z<-78) {
            return true;
        }
        else
        {
            return false;
        }
        
    }
    public Vector3 ReturnDestination()
    {
        return destination;
    }
    public void SetDestination(Vector3 position)
    {
        destination = position;
    }
    
    [PunRPC]
    public void StateAction()
    {
        selfAgent.SetDestination(destination);
        nowNPC.transform.LookAt(destination);//�̰� Ŀ�� ������ ���� �Ǵ°���?
        
        //PlayAnimation();
        



    }
    //public bool CheckStateEnd(Transform npcTransform)
    //{
    //    if ((destination.position - npcTransform.position).magnitude < 0.2f)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //
    //}

    public bool ForceStateEnd()
    {
        return true;
    }
    public bool CheckStateEnd()// ������ �� �� �Ű��� �ҵ� �ǹ� �ȿ� ������ �������� ������ �ڲ� ���峲
    {
        delayTime += Time.deltaTime;
        if ((destination - nowNPC.transform.position).magnitude < 5.0f)
        {
            //Debug.Log((destination - nowNPC.transform.position).magnitude);
            //StopAnimation();
            return true;
        }
        else
        {
            if (delayTime >= maxDelayTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    
}

public class NPCHit : INPCState
{
    //private bool isHit;
    private TestingNPC nowNPC;
    private Animator npcAnimator;
    private NavMeshAgent npcAgent;
    readonly int hashHit = Animator.StringToHash("isHit");
    readonly int hashMove = Animator.StringToHash("isMove");
    readonly int hashIdle = Animator.StringToHash("isIdle");
    readonly int hashselfVel = Animator.StringToHash("selfVel");

    //private bool isEnd;
    [PunRPC]
    public void PlayAnimation()
    {
        //animator����
        //npcAnimator.SetBool(hashHit,true);
        //npcAnimator.SetBool(hashIdle,true);
        //npcAnimator.SetBool("isAnger", true);
        //npcAnimator.SetFloat(hashselfVel, 0f);
    }
    [PunRPC]
    public void StopAnimation()
    {
        //animator����
        npcAnimator.SetBool("isAnger", false);
        npcAnimator.SetBool(hashHit, false);
        npcAnimator.SetFloat(hashselfVel, 0f);
    }

    [PunRPC]
    public void EnterState(TestingNPC npc)
    {
        //isHit = false;
        nowNPC = npc;
        npcAnimator = (npc as TestingNPC).animator;
        npcAgent= (npc as TestingNPC).SelfAgent;
        npcAgent.isStopped = true;
        npcAnimator.SetBool(hashHit, true);
        //npcAnimator.SetBool("isAnger", true);
        //npcAnimator.SetFloat(hashselfVel, 0f);

    }
    public void NPCGetHit()
    {
        //���� �Ŵ����� ��Ÿ �ǰݽ� ����� ���
       // isHit=true;
    }
    [PunRPC]
    public void StateAction()
    {
        //PlayAnimation();
        
    }
    public bool CheckStateEnd()
    {
        //if (npcAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime == 1)
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
        return false;
        
    }
    public bool ForceStateEnd()
    {
        return true;
    }

    public void EnterState(TestingNPC npc, float time)
    {
        
    }
}
public class NPCIdle : INPCState
{
    private float idleTime;// ����ؾ��� �ð�
    private float delaiedime;//����� �ð�
    private bool isEnd;
    private TestingNPC nowNPC;
    private Animator npcAnimator;
    private NavMeshAgent npcAgent;
    readonly int hashHit = Animator.StringToHash("isHit");
    readonly int hashMove = Animator.StringToHash("isMove");
    readonly int hashIdle = Animator.StringToHash("isIdle");
    readonly int hashselfVel = Animator.StringToHash("selfVel");
    [PunRPC]
    public void PlayAnimation()
    {
        npcAnimator.SetBool(hashIdle, true);
        npcAnimator.SetBool(hashMove, false);
        npcAnimator.SetBool("isAnger", false);
        npcAnimator.SetBool(hashHit, false);
        npcAnimator.SetFloat(hashselfVel, 0f);
        //animator����

    }
    [PunRPC]
    public void StopAnimation()
    {
        npcAnimator.SetBool(hashIdle, false);
        npcAnimator.SetBool(hashMove, true);
        //animator����

    }
    [PunRPC]
    public void EnterState(TestingNPC npc)
    {
        isEnd = false;
        idleTime = UnityEngine.Random.Range(0.2f,1.0f);//��� �ð� ����
        //Debug.Log(idleTime);
        nowNPC = npc;
        npcAnimator = (npc as TestingNPC).animator;
        npcAgent = (npc as TestingNPC).SelfAgent;
        npcAgent.isStopped = true;
        npcAnimator.SetBool("isAnger", false);
        npcAnimator.SetBool(hashHit, false);
        npcAnimator.SetBool(hashIdle, true);
        npcAnimator.SetBool(hashMove, false);
        npcAnimator.SetFloat(hashselfVel, 0f);
        //npcAnimator.SetBool("isAnger", false);
        //npcAnimator.SetBool(hashHit, false);
    }
    public void EnterState(TestingNPC npc, float time)
    {

        isEnd = false;
        idleTime = time;
        //Debug.Log(idleTime);
        nowNPC = npc;
        npcAnimator = (npc as TestingNPC).animator;
        npcAgent = (npc as TestingNPC).SelfAgent;
        nowNPC.SelfCollider.enabled = true;
        npcAgent.isStopped = true;
        npcAnimator.SetBool("isAnger", false);
        npcAnimator.SetBool(hashHit, false);
        
        npcAnimator.SetFloat(hashselfVel, 0f);
    }
    [PunRPC]
    public void StateAction()
    {
        //StartCoroutine(NPCIdleAnimationPlay());
        //PlayAnimation();
    }

    public void SetDelayTime(float time)
    {
        idleTime=time;
    }
    public bool CheckStateEnd()
    {
        delaiedime += Time.deltaTime;
        if(delaiedime>idleTime)
        {
            isEnd = true;
        }
        else
        {
            isEnd=false;
        }
        if (isEnd == true)
        {
            //Debug.Log("endidle");
            //StopAnimation();
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool ForceStateEnd()
    {
        return true;
    }

    
}
public class NPCDead : INPCState
{

    private TestingNPC nowNPC;
    private Animator npcAnimator;
    private NavMeshAgent npcAgent;
    readonly int hashHit = Animator.StringToHash("isHit");
    readonly int hashselfVel = Animator.StringToHash("selfVel");
    public bool CheckStateEnd()
    {
        return false;
    }

    public void EnterState(TestingNPC npc)
    {

    }

    public void EnterState(TestingNPC npc, float time)
    {
        nowNPC = npc;
        npcAnimator = (npc as TestingNPC).animator;
        npcAgent = (npc as TestingNPC).SelfAgent;
        npcAgent.isStopped = true;
    }

    public bool ForceStateEnd()
    {
        return true;
    }

    public void PlayAnimation()
    {

    }

    public void StateAction()
    {

    }

    public void StopAnimation()
    {

    }
}











//public class NPCRun : INPCState// �ϴ� �ȱ⸸ ���� �޸���� ���߿�
//{
//    public void PlayAnimation()
//    {
//        //animator����
//
//    }
//
//    public void StopAnimation()
//    {
//        //animator����
//
//    }
//
//    public void SetState()
//    {
//
//    }
//
//    public void StateAction()
//    {
//
//    }
//
//
//}



