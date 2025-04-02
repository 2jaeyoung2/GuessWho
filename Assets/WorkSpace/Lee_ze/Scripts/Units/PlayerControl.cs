using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable; //Photon Hashtable
using System;

public class PlayerControl : MonoBehaviourPun, IHittable
{
    private IPlayerStates currentState;

    public Animator playerAnim;

    private InGamePlayerList inGamePlayerList;

    private ExitGame exitGame;

    private bool isTeabagging = false;


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

    public GameObject itemStonePrefab;

    public GameObject GunFire;

    public bool isHit = false;

    public bool isNPC = false;

    public Vector3 apologizeTo;

    public int leftBullet;

    public bool canShoot = true;


    [Space(20), Header("Sound Control"), Space(10)]

    public AudioSource audioSource;

    public AudioClip running;

    public AudioClip kick;

    public AudioClip getHit;

    private void OnEnable()
    {
        nowHaveItems[0] = footData;

        holdingWeapon = nowHaveItems[0];

        foreach (var weapon in weapons)
        {
            weapon.SetActive(false); // ��� ���� ��Ȱ��ȭ
        }
    }

    private void Start()
    {
        inGamePlayerList = FindObjectOfType<InGamePlayerList>();

        exitGame = FindObjectOfType<ExitGame>();

        SetIsHit(false);

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

    public void OnRun(InputAction.CallbackContext ctx)
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        if (ctx.phase == InputActionPhase.Performed)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx) // ��Ŭ�� ���ε�
    {
        if (photonView.IsMine == false)
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
            leftBullet = item.bulletAmount;
        }

        else if (item.itemType == ItemType.Whistle)
        {
            nowHaveItems[2] = item;
        }
    }

    public void StonenOff()
    {
        if (weapons[1] != null)
        {
            weapons[1].SetActive(false);
        }
    }

    public void GunOff()
    {
        if (weapons[2] != null)
        {
            weapons[2].SetActive(false);
        }
    }

    public void GunParameterOff()
    {
        playerAnim.SetBool("IsShoot", false);
        //canShoot = true;
    }

    public void GetHit()
    {
        if (photonView.IsMine)
        {
            SetIsHit(true);

            exitGame.OnExitButton();
        }

        photonView.RPC("SyncHitState", RpcTarget.Others, true);

        StartCoroutine(WaitSetPlayerCount());
    }

    public void SetIsHit(bool value)
    {
        PhotonHashtable properties = new PhotonHashtable
        {
            {"isHit", value }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
    }

    private IEnumerator WaitSetPlayerCount()
    {
        yield return new WaitForSeconds(1f); // RPC ó�� ������

        inGamePlayerList.UpdateAlivePlayerCount();
    }

    public void ShootPistol()
    {
        float gunRange = 20.0f;

        Vector3 rayPosition = modelRotator.transform.position + modelRotator.transform.TransformDirection(new Vector3(0, 1, 1));
        Vector3 rayDirection = modelRotator.transform.TransformDirection(Vector3.forward);

        Debug.DrawRay(rayPosition, rayDirection, Color.red, gunRange);

        if (photonView.IsMine)
        {
            RaycastHit hit;

            if (Physics.Raycast(rayPosition, rayDirection, out hit, gunRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    PhotonView photonView = hit.collider.GetComponent<PhotonView>();

                    if (photonView != null)
                    {
                        Debug.Log("�÷��̾� �� ����"); 
                        Vector3 hitPosition = hit.point;
                        Quaternion hitRotation = Quaternion.LookRotation(hit.normal);
                        photonView.RPC("ApplyDamage", RpcTarget.AllBuffered, photonView.ViewID);

                        Instantiate(GunFire, hitPosition, hitRotation);
                    }
                }

                else if (hit.collider.CompareTag("Map"))
                {
                    Debug.Log("�� ����");
                    Vector3 hitPosition = hit.point;
                    Quaternion hitRotation = Quaternion.LookRotation(hit.normal);
                    Instantiate(GunFire, hitPosition, hitRotation);
                }

                else if (hit.collider.CompareTag("NPC"))
                {
                    Debug.Log("NPC�� ����");
                    Vector3 hitPosition = hit.point;
                    Quaternion hitRotation = Quaternion.LookRotation(hit.normal);
                    Instantiate(GunFire, hitPosition, hitRotation);
                    hit.collider.gameObject.GetComponent<TestingNPC>().GetDie();
                }
            }


        }
    }


    // V RPC Methods
    [PunRPC]
    private void SyncHitState(bool hit)
    {
        isHit = hit;
    }

    void StoneThrow()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("InstantiateStone", RpcTarget.All, photonView.ViewID);
        }
    }


    [PunRPC]
    private void InstantiateStone(int whoThrow)
    {
        GameObject stone = Instantiate(itemStonePrefab, transform.position, transform.rotation);

        Rigidbody stoneRb = stone.GetComponent<Rigidbody>();

        stone.GetComponent<StoneController>().whoThrow = whoThrow;

        Vector3 throwDirection = modelRotator.transform.TransformDirection(new Vector3(0, 5f, 10f));
        stoneRb.AddForce(throwDirection, ForceMode.Impulse);
    }

    [PunRPC]
    void ApplyDamage(int targetViewID)
    {
        PhotonView targetView = PhotonView.Find(targetViewID);

        if (targetView != null)
        {
            targetView.GetComponent<PlayerControl>().GetHit();

        }
    }

    // V RPC Methods (Sound)
    [PunRPC]
    void RPC_PlayAttackSound(Vector3 soundPosition)
    {
        audioSource.transform.position = soundPosition; // ���� ��ġ ����

        audioSource.PlayOneShot(kick);
    }

    [PunRPC]
    void RPC_PlayHitSound(Vector3 soundPosition)
    {
        audioSource.transform.position = soundPosition;

        audioSource.PlayOneShot(getHit);
    }

    [PunRPC]
    void RPC_PlayRunningSound(Vector3 soundPosition)
    {
        audioSource.transform.position = soundPosition;

        if (audioSource.isPlaying == false) // �̹� ��� ���̸� �ٽ� �������� ����
        {
            audioSource.clip = running;

            audioSource.loop = true;

            audioSource.Play();
        }
    }

    [PunRPC]
    void RPC_StopRunningSound(Vector3 soundPosition)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
