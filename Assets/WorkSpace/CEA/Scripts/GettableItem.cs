using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public interface IGetable
{
    public void SendItem(PlayerControl player, ItemData itemData);
}


public sealed class GettableItem : MonoBehaviourPun, IGetable
{
    #region ������ ȸ�� ���� ����
    [SerializeField]
    private float rotSpeed = 100.0f;

    [SerializeField]
    private GameObject ItemModel;
    #endregion

    [SerializeField]
    [Header("������ ���� �Ӽ� (Scriptable object)")] private ItemData itemData;

    [SerializeField]
    [Header("�������� ����� �� ��µǴ� ȿ��")] private ParticleSystem destroyParticle;

    [SerializeField]
    [Header("������ ���� �� Ȱ��ȭ�Ǵ� UI")] private Image ItemInteractImage;

    [SerializeField]
    [Header("�ڽ��� ������ ��ġ�� ������ ������")] private ItemSpawnctrl myParent;

    public void SendItem(PlayerControl player, ItemData itemData)
    {
        //�÷��̾ �������� ������ �޾ƿ� �޼���
        player.GetItem(itemData);

        Debug.Log("�÷��̾�� ������ ����" + itemData.itemName);
    }

    private void Update()
    {
        ItemModel.transform.rotation = Quaternion.Euler(ItemModel.transform.rotation.eulerAngles.x, 
            ItemModel.transform.rotation.eulerAngles.y + (rotSpeed * Time.deltaTime), ItemModel.transform.rotation.eulerAngles.z);
    }


    private void OnTriggerStay(Collider other)
    {
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

                photonView.RPC("NoticeIsAllitemOff", RpcTarget.All, myParent.name);

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
        photonView.RPC("PlayerItemGet", RpcTarget.All, myParent.name, itemnum);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ItemInteractImage.gameObject.SetActive(false);
        }
    }


}
