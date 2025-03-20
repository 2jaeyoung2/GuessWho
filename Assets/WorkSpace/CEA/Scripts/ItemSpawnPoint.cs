using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemSpawnPoint : MonoBehaviour
{
    [SerializeField]
    [Header("������ ���� ����Ʈ")] private GameObject[] spawnPoints;
    
    private ItemSpawnctrl itemSpawnctrl;
    

    private void Start()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            itemSpawnctrl = spawnPoint.GetComponent<ItemSpawnctrl>();

            if (itemSpawnctrl != null)
            {
                itemSpawnctrl.ItemSpawn();
            }
        }
    }

    
}
