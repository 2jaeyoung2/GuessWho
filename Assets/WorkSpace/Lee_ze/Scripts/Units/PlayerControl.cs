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

    private ExitGame exitGame;

    PlayerUI playerUI;


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

    public ItemData[] nowHaveItems = new ItemData[2]; //현재 보유중인 아이템의 배열 
                                                      //아이템 인벤토리처럼 생각해주세요 [맨손][비어있음][비어있음] 이런느낌
    public ItemData footData;

    public ItemData nowWeapon;

    public GameObject itemStonePrefab;

    public GameObject GunFire;

    public bool isHit = false;

    public bool isNPC = false;

    public Vector3 apologizeTo;

    public int leftBullet;


    [Space(20), Header("Sound Control"), Space(10)]

    public AudioSource audioSource;

    public AudioClip running;

    public AudioClip kick;

    public AudioClip getHit;

    public AudioClip GunShoot;

    public AudioClip stoneThrow;

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

        exitGame = FindObjectOfType<ExitGame>();

        playerUI = GameObject.Find("PlayerUI").GetComponent<PlayerUI>();

        SetIsHit(false);

        if (photonView.IsMine)
        {
            ChangeStateTo(new IdleState());

            GetComponent<RotateView>().SetTarget(this.transform);
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

    public void OnAttack(InputAction.CallbackContext ctx) // 좌클릭 바인딩
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
            if (nowHaveItems[1]!=null)
            {
                nowWeaponArrayNum++;

                if (nowWeaponArrayNum < 2)
                {
                    holdingWeapon = nowHaveItems[nowWeaponArrayNum];

                    playerUI.ChangeWeaponSelectUI(this);
                }

                else if (nowWeaponArrayNum >= 2)
                {
                    nowWeaponArrayNum = 0;

                    holdingWeapon = nowHaveItems[nowWeaponArrayNum];

                    playerUI.ChangeWeaponSelectUI(this);
                }

                if (holdingWeapon != null)
                {
                    UnityEngine.Debug.Log(holdingWeapon.name);
                }

                else if (holdingWeapon == null)
                {
                    UnityEngine.Debug.Log("null");
                }
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
    }

    public void GetHit()
    {
        if (photonView.IsMine)
        {
            SetIsHit(true);

            StartCoroutine(exitGame.OnExitButton());
        }

        photonView.RPC("SyncHitState", RpcTarget.Others, true);

        StartCoroutine(WaitSetPlayerCount());
    }

    public void SetIsHit(bool value)
    {
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();

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

    public void ThrowStoneSoundPlay()
    {
        if (photonView.IsMine)
        {
            audioSource.PlayOneShot(stoneThrow); 

            photonView.RPC("RPC_PlayThrowSound", RpcTarget.Others, this.transform.position); // RPC로 stoneThrow 사운드 나게 함
        }
    }

    public void GunShootingSoundPlay()
    {
        if (photonView.IsMine)
        {
            audioSource.PlayOneShot(GunShoot);

            photonView.RPC("RPC_PlayGunSound", RpcTarget.Others, this.transform.position); // RPC로 GunShoot 사운드 나게 함
        }
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

            if (Physics.Raycast(rayPosition, rayDirection, out hit, gunRange)) // Raycast 발사
            {
                if (hit.collider.CompareTag("Player")) // 맞은 collider의 태그가 Player라면
                {
                    PhotonView photonView = hit.collider.GetComponent<PhotonView>(); 

                    if (photonView != null) // 포톤뷰 유무 판별
                    {
                        Debug.Log("플레이어 총 맞음"); 

                        Vector3 hitPosition = hit.point; 

                        Quaternion hitRotation = Quaternion.LookRotation(hit.normal);

                        photonView.RPC("ApplyDamage", RpcTarget.AllBuffered, photonView.ViewID); //타격 적용

                        Instantiate(GunFire, hitPosition, hitRotation); //맞은 위치에 피격 효과 출력
                    }
                }

                else if (hit.collider.CompareTag("Map"))
                {
                    Vector3 hitPosition = hit.point;

                    Quaternion hitRotation = Quaternion.LookRotation(hit.normal);

                    Instantiate(GunFire, hitPosition, hitRotation);
                }

                else if (hit.collider.CompareTag("NPC"))
                {
                    Vector3 hitPosition = hit.point;

                    Quaternion hitRotation = Quaternion.LookRotation(hit.normal);

                    Instantiate(GunFire, hitPosition, hitRotation);

                    hit.collider.gameObject.GetComponent<TestingNPC>().GetDie();
                }
            }


        }
    }


    // ▽ RPC Methods
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
    private void GunActive()
    {
        weapons[2].SetActive(true);
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

    // ▽ RPC Methods (Sound)
    [PunRPC]
    void RPC_PlayAttackSound(Vector3 soundPosition)
    {
        audioSource.transform.position = soundPosition; // 사운드 위치 설정

        audioSource.PlayOneShot(kick);
    }

    [PunRPC]
    void RPC_PlayThrowSound(Vector3 soundPosition)
    {
        audioSource.transform.position = soundPosition; 

        audioSource.PlayOneShot(stoneThrow);
    }

    [PunRPC]
    void RPC_PlayGunSound(Vector3 soundPosition)
    {
        audioSource.transform.position = soundPosition;

        audioSource.PlayOneShot(GunShoot, 0.7f);
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
