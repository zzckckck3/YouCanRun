using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class ChangeCostume : MonoBehaviour
{

    public GameObject playerCharacterMainUi;
    public GameObject playerCharacterInventory;
    public TextMeshProUGUI playerHeadText;
    public TextMeshProUGUI playerBodyText;

    private void Start()
    {
        FirebaseAuthManager.Instance.Init();
    }

    // 타겟이 되는 HeadXX GameObject를 읽어온 뒤, Head를 삭제한 뒤 FireBase에 업데이트
    public void UpdateCharacterHead(GameObject targetHead)
    {
        string headNumber = targetHead.name.Replace("Head", "");

        // Firebase에 정보를 넘김과 동시에 GameManager의 userData를 업데이트
        FirebaseAuthManager.Instance.UpdateHeadToFireBase(headNumber);
        GameManager.Instance.UpdateHead(headNumber);


        // MainUI와 Inventory의 playerCharacter 하위 오브젝트 중 Head를 포함한 오브젝트를 모두 비활성화
        int numOfChild = playerCharacterMainUi.transform.childCount;
        for (int i = 0; i < numOfChild; i++)
        {
            if (playerCharacterMainUi.transform.GetChild(i).gameObject.name.Contains("Head"))
            {
                playerCharacterMainUi.transform.GetChild(i).gameObject.SetActive(false);
            }
            if (playerCharacterInventory.transform.GetChild(i).gameObject.name.Contains("Head"))
            {
                playerCharacterInventory.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        // 그 후 해당하는 오브젝트만 활성화
        GameObject activeHeadMainUI = playerCharacterMainUi.transform.Find("Head" + headNumber).gameObject;
        activeHeadMainUI.SetActive(true);

        GameObject activeHeadInventory = playerCharacterInventory.transform.Find("Head" + headNumber).gameObject;
        activeHeadInventory.SetActive(true);    
    }

    // 위와 동일하나, Head가 아닌 Body를 업데이트
    public void UpdateCharacterBody(GameObject targetBody)
    {
        string bodyNumber = targetBody.name.Replace("Body", "");

        // Firebase에 정보를 넘김과 동시에 GameManager의 userData를 업데이트
        FirebaseAuthManager.Instance.UpdateBodyToFireBase(bodyNumber);
        GameManager.Instance.UpdateBody(bodyNumber);

        // MainUI와 Inventory의 playerCharacter 하위 오브젝트 중 Body를 포함한 오브젝트를 모두 비활성화
        int numOfChild = playerCharacterMainUi.transform.childCount;
        for (int i = 0; i < numOfChild; i++)
        {
            if (playerCharacterMainUi.transform.GetChild(i).gameObject.name.Contains("Body"))
            {
                playerCharacterMainUi.transform.GetChild(i).gameObject.SetActive(false);
            }
            if (playerCharacterInventory.transform.GetChild(i).gameObject.name.Contains("Body"))
            {
                playerCharacterInventory.transform.GetChild(i).gameObject.SetActive(false);
            }
        }


        // 그 후 해당하는 오브젝트만 활성화
        GameObject activeHeadMainUI = playerCharacterMainUi.transform.Find("Body" + bodyNumber).gameObject;
        activeHeadMainUI.SetActive(true);

        GameObject activeHeadInventory = playerCharacterInventory.transform.Find("Body" + bodyNumber).gameObject;
        activeHeadInventory.SetActive(true);
    }
}

