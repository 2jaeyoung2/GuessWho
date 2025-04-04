using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using ZL.Unity;
using Photon.Realtime;

public class PlayerUI : MonoBehaviour
{
    public Image normalAttackSlot;
    public Image WeaponAttackSlot;

    public Image normalSlotSelect;
    public Image WeaponSlotSelect;

    public GameObject leftBullet;
    public Image[] weaponImages;

    TextMeshProUGUI leftBulletText;


    private void Awake()
    {
        leftBulletText = leftBullet.GetComponent<TextMeshProUGUI>();
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => PhotonNetwork.InRoom);
        normalAttackSlot.SetActive(true);
        WeaponAttackSlot.SetActive(true);
    }

    public void ChangeWeaponSelectUI(PlayerControl player)
    {
        if (player.holdingWeapon == player.footData || player.holdingWeapon == null)
        {
            normalSlotSelect.SetActive(true);
            WeaponSlotSelect.SetActive(false);

        }

        else if (player.holdingWeapon.itemType == ItemType.Stone ||
            player.holdingWeapon.itemType == ItemType.Gun)
        {
            WeaponSlotSelect.SetActive(true);
            normalSlotSelect.SetActive(false);
        }
    }

    public void ChangeWeaponIcon(PlayerControl player)
    {
        if(player.nowHaveItems[1].itemType == ItemType.Stone)
        {
            foreach(Image image in weaponImages)
            {
                image.gameObject.SetActive(false);
            }

            weaponImages[1].SetActive(true);
            leftBulletText.text = player.nowHaveItems[1].bulletAmount.ToString();
        }

        else if(player.nowHaveItems[1].itemType == ItemType.Gun)
        {
            foreach (Image image in weaponImages)
            {
                image.gameObject.SetActive(false);
            }

            weaponImages[2].SetActive(true);
            leftBulletText.text = player.nowHaveItems[1].bulletAmount.ToString();
        }
    }

    public void ChangeLeftBulletAmount(PlayerControl player)
    {
        leftBulletText.text = player.leftBullet.ToString();

        if(leftBulletText.text == "0")
        {
            ChangeWeaponIconToNull(player);
        }
    }

    public void ChangeWeaponIconToNull(PlayerControl player)
    {
        foreach (Image image in weaponImages)
        {
            image.gameObject.SetActive(false);
        }

        weaponImages[0].SetActive(true);
        leftBulletText.text = "-";

        normalSlotSelect.SetActive(true);
        WeaponSlotSelect.SetActive(false);

        player.nowWeaponArrayNum = 0;
        player.holdingWeapon = player.nowHaveItems[0];
    }

}
