using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpClose : MonoBehaviour
{
    public GameObject popUp;
    public GameObject failPopUp;
    public GameObject TitlePopUp;
    public void ClosePopUp()
    {
        popUp.SetActive(false);
    }
    public void CloseFailPopUp()
    {
        failPopUp.SetActive(false);
    }
    public void CloseNoTitlePopUp()
    {
        TitlePopUp.SetActive(false);
    }
}
