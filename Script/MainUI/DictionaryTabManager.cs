using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryTabManager : MonoBehaviour
{
    public GameObject skillTab;
    public GameObject itemTab;
    //public GameObject mapTab;

    public GameObject skillUnderbar;
    public GameObject itemUnderbar;
    //public GameObject mapUnderbar;

    public void OpenSkillTab()
    {
        skillTab.SetActive(true);
        itemTab.SetActive(false);
        //mapTab.SetActive(false);

        skillUnderbar.SetActive(true);
        itemUnderbar.SetActive(false);
        //mapUnderbar.SetActive(false);
    }
    public void OpenItemTab()
    {
        skillTab.SetActive(false);
        itemTab.SetActive(true);
        //mapTab.SetActive(false);

        skillUnderbar.SetActive(false);
        itemUnderbar.SetActive(true);
        //mapUnderbar.SetActive(false);
    }
    public void OpenMapTab()
    {
        skillTab.SetActive(false);
        itemTab.SetActive(false);
        //mapTab.SetActive(true);

        skillUnderbar.SetActive(false);
        itemUnderbar.SetActive(false);
        //mapUnderbar.SetActive(true);
    }
}
