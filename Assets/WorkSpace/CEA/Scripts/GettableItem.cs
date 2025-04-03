using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public interface IGetable
{
    public void SendItem(PlayerControl player, ItemData itemData);
}


public sealed class GettableItem : MonoBehaviourPun, IGetable
{
    #region 아이템 회전 관련 변수
    [SerializeField]
    private float rotSpeed = 100.0f;

    [SerializeField]
    private GameObject ItemModel;
    #endregion

    [SerializeField]
    [Header("아이템 개별 속성 (Scriptable object)")] private ItemData itemData;

    [SerializeField]
    [Header("아이템이 사라질 때 출력되는 효과")] private ParticleSystem destroyParticle;

    [SerializeField]
    [Header("아이템 근접 시 활성화되는 UI")] private Image ItemInteractImage;

    [SerializeField]
    [Header("자신의 상위에 위치한 아이템 스포너")] private ItemSpawnctrl myParent;

    private Collider tempCollider;

    public GameObject itemAudioPlayer;

    public void SendItem(PlayerControl player, ItemData itemData)
    {
        //플레이어가 아이템의 정보를 받아올 메서드
        player.GetItem(itemData);

        Debug.Log("플레이어에게 아이템 전달" + itemData.itemName);
    }

    private void Update()
    {
        ItemModel.transform.rotation = Quaternion.Euler(ItemModel.transform.rotation.eulerAngles.x,
            ItemModel.transform.rotation.eulerAngles.y + (rotSpeed * Time.deltaTime), ItemModel.transform.rotation.eulerAngles.z);
    }


    private void OnTriggerStay(Collider other)
    {
        tempCollider = other;

        if (other.CompareTag("Player"))
        {
            PlayerControl player = other.GetComponentInParent<PlayerControl>();

            PlayerUI playerUI = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<PlayerUI>();

            if (player == null)
            {
                return;
            }

            Image[] PlayerInteractImages = other.GetComponentsInChildren<Image>(true);

            foreach (Image image in PlayerInteractImages)
            {
                if (image.gameObject.name == "PressFImage")
                {
                    ItemInteractImage = image;
                    break;
                }
            }

            if (player.photonView.IsMine)
            {
                if (ItemInteractImage != null)
                {
                    ItemInteractImage.gameObject.SetActive(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.F) && player.photonView.IsMine)
            {
                SendItem(player, itemData);//플레이어에게 아이템 데이터 전송

                Instantiate(itemAudioPlayer, transform.position, Quaternion.identity);

                Instantiate(destroyParticle, transform.TransformPoint(0, 1.0f, 0), Quaternion.identity);
                //사운드와 파티클 요소 출력

                photonView.RPC("NoticeIsAllitemOff", RpcTarget.AllBuffered, myParent.name);

                ItemInteractImage.gameObject.SetActive(false);//상호작용 이미지 off

                switch (itemData.itemType)
                {
                    case ItemType.Stone:
                        PlayerItemGetAndOff(1);
                        //필드에 존재하는 아이템을 비활성화할 때 사용하는 메서드

                        playerUI.ChangeWeaponIcon(player);
                        break;

                    case ItemType.Gun:
                        PlayerItemGetAndOff(2);

                        playerUI.ChangeWeaponIcon(player);
                        break;

                        //case ItemType.Whistle:
                        //    PlayerItemGetAndOff(3);
                        //    break;
                }
            }
        }
    }

    private void PlayerItemGetAndOff(int itemnum)
    {
        photonView.RPC("PlayerItemGet", RpcTarget.AllBuffered, myParent.name, itemnum);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ItemInteractImage.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (tempCollider != null)
        {
            if (tempCollider.CompareTag("Player"))
            {
                if (ItemInteractImage.gameObject != null)
                {
                    ItemInteractImage.gameObject.SetActive(false);
                }
            }
        }
    }
}
