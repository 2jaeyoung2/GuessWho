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
    public void EnterState(NPC npc);

    public void StateAction();
    public bool CheckStateEnd();
    public bool ForceStateEnd();
}

public class NPCMove : INPCState
{
    //private List<Vector3> destinations;//navmesh �Ⱦ��� 
    //private bool isMoveEnd;
    private Vector3 destination;
    private NPC nowNPC;
    private NavMeshAgent selfAgent;
    //float temp = 0;
    private Animator npcAnimator;
    readonly int hashHit = Animator.StringToHash("isHit");
    readonly int hashMove = Animator.StringToHash("isMove");
    readonly int hashIdle = Animator.StringToHash("isIdle");

    //GameObject forTest;
    [PunRPC]
    public void PlayAnimation()
    {
        //animator����
        
        npcAnimator.SetBool(hashIdle, false);
        npcAnimator.SetBool(hashMove, true);
        
    }
    [PunRPC]
    public void StopAnimation()
    {
        //animator����
        npcAnimator.SetBool(hashIdle, true);
        npcAnimator.SetBool(hashMove, false);
        selfAgent.isStopped = true;

    }
    [PunRPC]
    public void EnterState(NPC npc)
    {
        nowNPC = npc;
        selfAgent = npc.gameObject.GetComponent<NavMeshAgent>();
        selfAgent.isStopped = false;
        npcAnimator = (npc as TestingNPC).animator;
        destination = new Vector3();
        //destination.position = NPCManager.ReturnRandomDestination();//npc �Ŵ����� �����ϴ� ���� ��ǥ ���� �Լ��� ���, ���� ����ó�� �ȵǾ� ����
        //selfAgent.SetDestination(destination.position);//������ ���� 
        //forTest= npc.GetComponent<TestingNPC>().forTest;

        //temp+= Time.deltaTime;//�۵���! ���̵�!

        destination = NPCManager.ReturnRandomDestination();//npc �Ŵ����� �����ϴ� ���� ��ǥ ���� �Լ��� ���, ���� ����ó�� �ȵǾ� ����-> transform ���� �����ϸ� ������ ������ ����3 �ؼ� ���ߴ°� �� ����
        //�̰� navmesh���� �����ϰ� �ؾ� �ҵ� ���� ��¡ 
        //Debug.Log(destination);   
        
        //isMoveEnd = false;
       
    }
    public Vector3 ReturnDestination()
    {
        return destination;
    }
    public void SetDestination(Vector3 position)
    {
        destination = position;
    }
    //public void SetState(Transform destination)
    //{
    //    destinations = new List<Vector3>();
    //    destination.position=destinations[0];
    //    StateAction();
    //    this.destination = destination;
    //}
    [PunRPC]
    public void StateAction()
    {
        PlayAnimation();
        selfAgent.SetDestination(destination);
        nowNPC.transform.LookAt(destination);



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
        if ((destination - nowNPC.transform.position).magnitude < 5.0f)
        {
            //Debug.Log((destination - nowNPC.transform.position).magnitude);
            StopAnimation();
            return true;
        }
        else
        {
            return false;
        }
    }

}

public class NPCHit : INPCState
{
    private bool isHit;
    private NPC nowNPC;
    private Animator npcAnimator;
    private NavMeshAgent npcAgent;
    readonly int hashHit = Animator.StringToHash("isHit");
    readonly int hashMove = Animator.StringToHash("isMove");
    readonly int hashIdle = Animator.StringToHash("isIdle");

    //private bool isEnd;
    [PunRPC]
    public void PlayAnimation()
    {
        //animator����
        npcAnimator.SetBool(hashHit,true);
        npcAnimator.SetBool(hashIdle,true);
        npcAnimator.SetBool("isAnger", true);
    }
    [PunRPC]
    public void StopAnimation()
    {
        //animator����
        npcAnimator.SetBool("isAnger", false);
        npcAnimator.SetBool(hashHit, false);

    }

    [PunRPC]
    public void EnterState(NPC npc)
    {
        isHit = false;
        nowNPC = npc;
        npcAnimator = (npc as TestingNPC).animator;
        npcAgent= (npc as TestingNPC).SelfAgent;
        npcAgent.isStopped = true;
    }
    public void NPCGetHit()
    {
        //���� �Ŵ����� ��Ÿ �ǰݽ� ����� ���
        isHit=true;
    }
    [PunRPC]
    public void StateAction()
    {
        PlayAnimation();
        
    }
    public bool CheckStateEnd()
    {
        if (npcAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
        if (isHit == true)
        {
            StopAnimation();
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
public class NPCIdle : INPCState
{
    private float idleTime;// ����ؾ��� �ð�
    private float delaiedime;//����� �ð�
    private bool isEnd;
    private NPC nowNPC;
    private Animator npcAnimator;
    private NavMeshAgent npcAgent;
    readonly int hashHit = Animator.StringToHash("isHit");
    readonly int hashMove = Animator.StringToHash("isMove");
    readonly int hashIdle = Animator.StringToHash("isIdle");
    [PunRPC]
    public void PlayAnimation()
    {
        npcAnimator.SetBool(hashIdle, true);
        npcAnimator.SetBool(hashMove, false);
        npcAnimator.SetBool("isAnger", false);
        npcAnimator.SetBool(hashHit, false);
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
    public void EnterState(NPC npc)
    {
        isEnd = false;
        idleTime = UnityEngine.Random.Range(0.5f,2f);//��� �ð� ����
        //Debug.Log(idleTime);
        nowNPC = npc;
        npcAnimator = (npc as TestingNPC).animator;
        npcAgent = (npc as TestingNPC).SelfAgent;
        npcAgent.isStopped = true;
    }
    [PunRPC]
    public void StateAction()
    {
        //StartCoroutine(NPCIdleAnimationPlay());
        PlayAnimation();
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
            StopAnimation();
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



