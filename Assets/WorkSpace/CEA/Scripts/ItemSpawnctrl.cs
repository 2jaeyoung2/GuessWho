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

    private bool isAllItemOff = false;

    public bool IsAllItemOff 
    {
        get { return isAllItemOff; }
        set { isAllItemOff = value; }
    }

    private float respawnCoolTime = 20.0f;
    private float respawnElapsedTime;

    private void Update()
    {
        if(isAllItemOff == true)
        {
            respawnElapsedTime += Time.deltaTime;
            //Debug.Log("������ ������ �����: " + respawnElapsedTime);

            if (respawnCoolTime < respawnElapsedTime)
            {
                ItemSpawn();
                isAllItemOff = false;
                respawnElapsedTime = 0;
            }
        }
    }

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
