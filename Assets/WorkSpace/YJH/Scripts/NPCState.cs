using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface INPCState
{
    public void PlayAnimation();
    public void StopAnimation();
    public void SetState();

}

public class NPCMove : INPCState
{
    List<Vector3> destinations;//navmesh �Ⱦ��� 


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
        destinations = new List<Vector3> ();
        destinations.Add (NPCManager.ReturnRandomDestination ());
    }
    



}
public class NPCRun : INPCState
{
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

    }

    


}
public class NPCHit : INPCState
{
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

    }
    public void NPCGetHit()
    {
        //���� �Ŵ����� ��Ÿ �ǰݽ� ����� ���
    }

}
public class NPCIdle : INPCState
{
    private float idleTime;


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
        idleTime = UnityEngine.Random.Range(0.5f,2f);//��� �ð� ����
    }



}



