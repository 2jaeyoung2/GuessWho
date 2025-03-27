using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviourPun, IHittable
{
    private IPlayerStates currentState;

    public Animator playerAnim;


    [Header("Move"), Space(10)]

    public float moveSpeed;

    public float targetSpeed;

    public Rigidbody rb;

    public Vector2 direction;

    public bool isRunning = false;

    public RotateModel modelRotator;

    public int nowWeaponArrayNum = 0;


    [Space(20), Header("Attack"), Space(10)]

    public ItemData holdingWeapon;

    public bool isAttackTriggered = false;

    public GameObject[] weapons;

    public ItemData[] nowHaveItems = new ItemData[3]; //���� �������� �������� �迭 
                                                      //������ �κ��丮ó�� �������ּ��� [�Ǽ�][�������][�������] �̷�����
    public ItemData footData;

    public ItemData nowWeapon;

    public bool isHit = false;

    public bool isNPC = false;

    public Vector3 apologizeTo;


    [Space(20), Header("Sound Control"), Space(10)]

    public AudioSource audioSource;

    public AudioClip kick;

    public AudioClip getHit;

    private void OnEnable()
    {
        holdingWeapon = footData;

        foreach (var weapon in weapons)
        {
            weapon.SetActive(false); // ��� ���� ��Ȱ��ȭ
        }
    }

    private void Start()
    {

        if (photonView.IsMine)
        {
            ChangeStateTo(new IdleState());

            GetComponent<RotateView>().SetTarget(this.transform);
        }

        if (photonView.IsMine)
        {
            ChangeStateTo(new IdleState());
        }
    }

    private void Update()
    {
        if (photonView.IsMine) // �ڽ��� ĳ���͸� ���ÿ��� ���� ����
        {
            currentState?.UpdatePerState();
        }
    }

    public void ChangeStateTo(IPlayerStates nextState)
    {
        currentState?.ExitState();

        currentState = nextState;

        currentState.EnterState(this);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            direction = ctx.ReadValue<Vector2>();
        }
        else if (ctx.phase == InputActionPhase.Canceled)
        {
            direction = Vector2.zero;

            rb.velocity = Vector3.zero;
        }
    }

    public void OnWeaponChange(InputAction.CallbackContext ctx) //���� ��ȯ �޼���
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            nowWeaponArrayNum++;

            if(nowWeaponArrayNum < 3)
            {
                holdingWeapon = nowHaveItems[nowWeaponArrayNum];
            }
            
            else if(nowWeaponArrayNum >= 3)
            {
                nowWeaponArrayNum = 0;
                holdingWeapon = nowHaveItems[nowWeaponArrayNum];

            }

            if(holdingWeapon != null)
            {
                UnityEngine.Debug.Log(holdingWeapon.name);
            }

            else if(holdingWeapon == null)
            {
                UnityEngine.Debug.Log("null");
            }
        }
    }

    public void GetItem(ItemData item) //������ �޾ƿ��� �޼���
    {

        if (item.itemType == ItemType.Stone || item.itemType == ItemType.Gun)
        {
            nowWeapon = item;
            nowHaveItems[1] = nowWeapon;

        }

        else if (item.itemType == ItemType.Whistle)
        {
            nowHaveItems[2] = item;
        }
    }

    public void OnRun(InputAction.CallbackContext ctx)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (ctx.phase == InputActionPhase.Performed)
        {
            isRunning = true;
        }
        else if (ctx.phase == InputActionPhase.Canceled)
        {
            isRunning = false;
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx) // ��Ŭ�� ���ε�
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (ctx.phase == InputActionPhase.Started)
        {
            isAttackTriggered = true;
        }
    }

    public void OnKickEnable() // Kick �ִϸ��̼� Ư�� ��ġ�� �̺�Ʈ ���ε�����.
    {
        weapons[0].SetActive(true);
    }

    public void OnKickDisable() // Kick �ִϸ��̼� Ư�� ��ġ�� �̺�Ʈ ���ε�����.
    {
        weapons[0].SetActive(false);
    }

    public void GetHit()
    {
        photonView.RPC("SyncHitState", RpcTarget.Others, true);
    }

    // V RPC Methods

    [PunRPC]
    private void SyncHitState(bool hit)
    {
        isHit = hit;
    }

    [PunRPC]
    void RPC_PlayAttackSound(Vector3 soundPosition)
    {
        audioSource.transform.position = soundPosition; // ���� ��ġ ����

        audioSource.PlayOneShot(kick);
    }
}
