using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIPopupManager : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject dictionaryUI;
    public GameObject MainUI;
    public GameObject userInfoUI;
    public GameObject rankingUI;

    public void OpenCloseInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
        MainUI.SetActive(!MainUI.activeSelf);
    }

    public void OpenCloseDictionary()
    {
        dictionaryUI.SetActive(!dictionaryUI.activeSelf);
        MainUI.SetActive(!MainUI.activeSelf);
    }

    public void OpenCloseUserInfo()
    {
        userInfoUI.SetActive(!userInfoUI.activeSelf);
    }

    public void OpenCloseRanking()
    {
        MainUI.SetActive(!MainUI.activeSelf);
        rankingUI.SetActive(!rankingUI.activeSelf);
    }


}
