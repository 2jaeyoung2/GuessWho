using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;


public interface IGetable
{
    public void SendItem(PlayerControl player, ItemData itemData);
}


public sealed class GettableItem : MonoBehaviour, IGetable
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

    [SerializeField]
    [Header("�����")] private PhotonView photonView;

    public void SendItem(PlayerControl player, ItemData itemData)
    {
        //�÷��̾ �������� ������ �޾ƿ� �޼���
        player.GetItem(itemData);

        /*
         * �� �̰� �޾ƿԾ�
         * �� ������ ������ �־�
         * �� ���� ��ź��
         * 
         * �Ǽ� �� ¯��
         *
         *�÷��̾ ������ �޾ƿ� �׹���....
         *���ٰ� �� ������ �����ؼ�
         *������ ������ ���� � �����
         *����,���� ���� ��
         */

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

            if(ItemInteractImage != null)
            {
                ItemInteractImage.gameObject.SetActive(true);
            }

            else
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                SendItem(player, itemData);
                Instantiate(destroyParticle, transform.TransformPoint(0, 1.0f, 0), Quaternion.identity);
                ItemInteractImage.gameObject.SetActive(false);
                this.gameObject.SetActive(false);

                photonView.RPC("NoticeIsAllitemOff", RpcTarget.MasterClient);
            }
        }
    }

    [PunRPC]
    private void NoticeIsAllitemOff()
    {
        myParent.IsAllItemOff = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ItemInteractImage.gameObject.SetActive(false);
        }
    }


}
