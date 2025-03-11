using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon;
using Photon.Pun;
using Unity.VisualScripting;



public interface INPCState
{
    
    public void PlayAnimation();
    public void StopAnimation();
    public void SetState();

    public void StateAction();
    public bool CheckStateEnd();
}

public class NPCMove : MonoBehaviour, INPCState
{
    private List<Vector3> destinations;//navmesh �Ⱦ��� 
    private bool isMoveEnd;
    private Transform destination;
    private Transform nowTransform;
    private NavMeshAgent selfAgent;
    public void PlayAnimation()
    {
        //animator����
        
    }

    public void StopAnimation()
    {
        //animator����

    }
    public void SetState()
    {
        destinations = new List<Vector3>();
        destination.position=destinations[0];
        
       
    }
    public void AddDestination(Transform transform)
    {

    }
    //public void SetState(Transform destination)
    //{
    //    destinations = new List<Vector3>();
    //    destination.position=destinations[0];
    //    StateAction();
    //    this.destination = destination;
    //}
    public void StateAction()
    {
        selfAgent.SetDestination(destination.position);
    }
    public bool CheckStateEnd(Transform npcTransform)
    {
        if ((destination.position - npcTransform.position).magnitude < 0.2f)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    public bool CheckStateEnd()
    {
        return false;
    }

}

public class NPCHit : MonoBehaviour,INPCState
{
    private bool isHit;
    //private bool isEnd;
    public void PlayAnimation()
    {
        //animator����

    }

    public void StopAnimation()
    {
        //animator����
        
    }


    public void SetState()
    {
        isHit = false;
    }
    public void NPCGetHit()
    {
        //���� �Ŵ����� ��Ÿ �ǰݽ� ����� ���
        isHit=true;
    }
    public void StateAction()
    {
        PlayAnimation();
    }
    public bool CheckStateEnd()
    {
        if (isHit == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}
public class NPCIdle : MonoBehaviour,INPCState
{
    private float idleTime;
    private bool isEnd;
    
    public void PlayAnimation()
    {
        //animator����

    }

    public void StopAnimation()
    {
        //animator����
        isEnd = true;
    }

    public void SetState()
    {
        isEnd = false;
        idleTime = UnityEngine.Random.Range(0.5f,2f);//��� �ð� ����
    }

    public void StateAction()
    {
        StartCoroutine(NPCIdleAnimationPlay());
    }
    IEnumerator NPCIdleAnimationPlay()
    {
        PlayAnimation();
        yield return new WaitForSeconds(idleTime);
        StopAnimation();
    }

    public bool CheckStateEnd()
    {
        if (isEnd == true)
        {
            return true;
        }
        else
        {
            return false;
        }
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



