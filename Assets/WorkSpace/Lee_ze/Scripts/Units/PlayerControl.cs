using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable; //Photon Hashtable

public class PlayerControl : MonoBehaviourPun, IHittable
{
    private IPlayerStates currentState;

    public Animator playerAnim;

    private InGamePlayerList inGamePlayerList;


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

    public ItemData[] nowHaveItems = new ItemData[3]; //현재 보유중인 아이템의 배열 
                                                      //아이템 인벤토리처럼 생각해주세요 [맨손][비어있음][비어있음] 이런느낌
    public ItemData footData;

    public ItemData nowWeapon;

    public GameObject itemStonePrefab;

    public bool isHit = false;

    public bool isNPC = false;

    public Vector3 apologizeTo;

    public int leftBullet;


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
            weapon.SetActive(false); // 모든 무기 비활성화
        }
    }

    private void Start()
    {
        inGamePlayerList = FindObjectOfType<InGamePlayerList>();

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
        if (photonView.IsMine) // 자신의 캐릭터만 로컬에서 조작 가능
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
        if (!photonView.IsMine)
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

    public void OnAttack(InputAction.CallbackContext ctx) // 좌클릭 바인딩
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

    public void OnKickEnable() // Kick 애니메이션 특정 위치에 이벤트 바인딩했음.
    {
        weapons[0].SetActive(true);
    }

    public void OnKickDisable() // Kick 애니메이션 특정 위치에 이벤트 바인딩했음.
    {
        weapons[0].SetActive(false);
    }

    public void OnWeaponChange(InputAction.CallbackContext ctx) //무기 전환 메서드
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

    public void GetItem(ItemData item) //아이템 받아오는 메서드
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

    public void GetHit()
    {
        if (photonView.IsMine)
        {
            SetIsHit(true);
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
        yield return new WaitForSeconds(1f); // RPC 처리 딜레이

        inGamePlayerList.UpdateAlivePlayerCount();
    }


    void StoneThrow()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("InstantiateStone", RpcTarget.All, photonView.ViewID);
        }
    }

    // V RPC Methods
    [PunRPC]
    private void SyncHitState(bool hit)
    {
        isHit = hit;
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

    // V RPC Methods (Sound)
    [PunRPC]
    void RPC_PlayAttackSound(Vector3 soundPosition)
    {
        audioSource.transform.position = soundPosition; // 사운드 위치 설정

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

        if (audioSource.isPlaying == false) // 이미 재생 중이면 다시 실행하지 않음
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
