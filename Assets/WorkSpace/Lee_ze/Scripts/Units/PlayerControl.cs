using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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


    [Space(20), Header("Attack"), Space(10)]

    public ItemType holdingWeapon;

    public bool isAttackTriggered = false;

    public GameObject[] weapons;

    public bool isHit = false;

    public bool isNPC = false;

    private void OnEnable()
    {
        holdingWeapon = ItemType.None;

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

            Camera.main.GetComponent<RotateView>().SetTarget(this.transform);
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

    public void OnRun(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            isRunning = true;
        }
        else if (ctx.phase == InputActionPhase.Canceled)
        {
            isRunning = false;
        }

        // ��Ʈ��ũ�� ���� ����ȭ
        photonView.RPC("SetRunningState", RpcTarget.Others, isRunning);
    }

    public void OnAttack(InputAction.CallbackContext ctx) // ��Ŭ�� ���ε�
    {
        if (ctx.phase == InputActionPhase.Started)
        {
            isAttackTriggered = true;

            photonView.RPC("TriggerAttack", RpcTarget.Others); // RPC�� ���� ���� ����
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
        isHit = true;

        photonView.RPC("SyncHitState", RpcTarget.Others, isHit);
    }

    // V RPC Methods

    [PunRPC]
    private void SetRunningState(bool state)
    {
        isRunning = state;
    }

    [PunRPC]
    private void TriggerAttack()
    {
        isAttackTriggered = true;
    }

    [PunRPC]
    private void SyncHitState(bool hit)
    {
        isHit = hit;
    }
}
