using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attacks : MonoBehaviour
{
    public event Action<bool> OnAttackStateChanged;

    [SerializeField]
    private GameObject foot;

    [SerializeField]
    private Animator kickAnim;

    private bool isAttacking;

    public bool IsAttacking => isAttacking;

    private void Start()
    {
        foot.SetActive(false);
    }

    public void OnAttack(InputAction.CallbackContext ctx) // ��Ŭ�� ���ε�
    {
        if (ctx.phase == InputActionPhase.Started && isAttacking == false)
        {
            StartCoroutine(KickAttack());
        }
    }

    public void OnKickEnable() // �ִϸ��̼� Ư�� ��ġ�� �̺�Ʈ ���ε�����.
    {
        foot.SetActive(true);
    }

    public void OnKickDisable() // �ִϸ��̼� Ư�� ��ġ�� �̺�Ʈ ���ε�����.
    {
        foot.SetActive(false);
    }

    IEnumerator KickAttack()
    {
        isAttacking = true;  // ���� ����

        OnAttackStateChanged?.Invoke(isAttacking);

        kickAnim.SetBool("IsKick", true);

        yield return new WaitForSeconds(1f);

        kickAnim.SetBool("IsKick", false);

        isAttacking = false;

        OnAttackStateChanged?.Invoke(isAttacking);
    }
}
