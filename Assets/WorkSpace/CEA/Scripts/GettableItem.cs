using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ItemList
{
    stone, gun
}

public interface IGetable
{
    public void GetItem(ItemData itemData);
}


public class GettableItem : MonoBehaviour, IGetable
{
    #region ������ ȸ�� ���� ����
    [SerializeField]
    private float rotSpeed = 100.0f;

    [SerializeField]
    private GameObject ItemModel;
    #endregion

    [SerializeField]
    private ItemData itemData;
    [SerializeField]
    private ParticleSystem destroyParticle;

    public void GetItem(ItemData itemData)
    {
        //�÷��̾ �������� ������ �޾ƿ� �޼���
        //player.getitem(itemdata)

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

        Debug.Log("�÷��̾�� ������ ����");
    }

    private void Update()
    {
        //ItemModel.transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime, Space.Self);
        ItemModel.transform.rotation = Quaternion.Euler(ItemModel.transform.rotation.eulerAngles.x, 
            ItemModel.transform.rotation.eulerAngles.y + (rotSpeed * Time.deltaTime), ItemModel.transform.rotation.eulerAngles.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetItem(itemData);
            Instantiate(destroyParticle, transform.TransformPoint(0, 1.0f, 0), Quaternion.identity);

            Destroy(gameObject);

            // Destroy(destroyParticle, 2.0f);
        }
    }
}
