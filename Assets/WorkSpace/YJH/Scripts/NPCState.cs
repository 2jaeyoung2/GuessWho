using System.Collections;
using System.Collections.Generic;
using UnityEngine;



interface INPCState
{
    public void PlayAnimation();
    public void StopAnimation();
    public void SetState();

}

public class NPCMove : INPCState
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


}
public class NPCIdle : INPCState
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



