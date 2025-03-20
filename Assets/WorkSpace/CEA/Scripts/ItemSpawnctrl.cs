using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnctrl : MonoBehaviour
{
    [SerializeField]
    private GameObject gunItem;

    [SerializeField]
    private GameObject whistleItem;

    [SerializeField]
    private GameObject stoneItem;

    //�ڽ� ��ü�� ��� �����ִ��� Ȯ���ϴ� �޼���
    public bool AllItemDisabled(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.activeSelf)
            {
                return false;
            }
        }
        return true;
    } 

    public void ItemSpawn() //�������� �����ϴ� �Լ�
    {
        int itemNum = Random.Range(1, 11);

        switch (itemNum)
        {
            case 1:
                gunItem.SetActive(true);
                break;

            case 2:
                whistleItem.SetActive(true);
                break;

            default:
                stoneItem.SetActive(true);
                break;
        }
    }
}
