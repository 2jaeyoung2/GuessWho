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

            if (player == null)
            {
                return;
            }

            Image[] PlayerInteractImages = other.GetComponentsInChildren<Image>(true);
            
            foreach(Image image in PlayerInteractImages)
            {
                if(image.gameObject.name == "PressFImage")
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
                SendItem(player, itemData);
                Instantiate(destroyParticle, transform.TransformPoint(0, 1.0f, 0), Quaternion.identity);

                photonView.RPC("NoticeIsAllitemOff", RpcTarget.AllBuffered, myParent.name);

                ItemInteractImage.gameObject.SetActive(false);

                switch(itemData.itemType)
                {
                    case ItemType.Stone:
                        PlayerItemGetAndOff(1);
                        break;

                    case ItemType.Gun:
                        PlayerItemGetAndOff(2);
                        break;

                    case ItemType.Whistle:
                        PlayerItemGetAndOff(3);
                        break;
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
        if(tempCollider != null)
        {
            if(ItemInteractImage.gameObject != null)
            {
                ItemInteractImage.gameObject.SetActive(false);
            }
        }
    }

}
