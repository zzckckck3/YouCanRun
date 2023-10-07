using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using System;
using Photon.Pun;

public class MoveToMainUi : MonoBehaviour
{
    private FirebaseAuthManager.UserData userData;
    public GameObject loadingUi;
    public GameObject mainUi;
    public GameObject playerCharacterMainUi;
    public GameObject playerCharacterInventory;

    private bool readyFlag = true;
    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // 마우스 커서 락
        FirebaseAuthManager.Instance.Init();

        // 이제는 로그인이 진행 된 후 Scene이 넘어오기 때문에 바로 실행
        CallUserData();
        // 1초 뒤 CharacterUi를 불러옴
        Invoke("SetCharacterUi", 1f);
        // 2초 뒤 MainUi를 불러옴
        //Invoke("LoadMainUi", 2f);
    }

    private void Update()
    {
        if (PhotonNetwork.InLobby && readyFlag)
        {
            LoadMainUi();
            readyFlag = false;
        }
    }

    // FirebaseAuthManager에서 UserData를 불러옴
    public void CallUserData()
    {
        // 이제 UserData는 GameManager에서 관리하기에 GameManager의 UserData에 넣어줌.
        // 이후 사용이 편리하도록 현재 script의 userData에 넣어줌. 
        userData = GameManager.Instance.GetUserData();

        // 추가된 컬럼 확인
        Debug.Log("처음 플레이 하는 유저인가? : " + userData.isFirstPlay);
        Debug.Log("레벨 : " + userData.level);
    }

    // Character의 코스튬을 Database에서 불러와 적용시켜주는 함수
    public void SetCharacterUi()
    {
        Debug.Log("UserData Load Complete!" + userData.userId);
        // UserData에서 불러온 Head, Body정보를 변수에 담아 한자리 수일 시 01, 02형식으로 만들어줌
        // 사실상 결국 쓰이지 않게 될 코드이지만, 혹시 몰라서 남겨둠
        string userHead = userData.head;
        string userBody = userData.body;
        if(userHead.Length == 1)
        {
            userHead = "0" + userHead;
        }
        if (userBody.Length == 1)
        {
            userBody = "0" + userBody;
        }

        // 모든 자식 오브젝트를 root(위치 정보, head, body 배치 등)를 제외하고 SetActive(false)
        int numOfChild = playerCharacterMainUi.transform.childCount;
        for (int i = 0; i < numOfChild; i++)
        {
            if(playerCharacterMainUi.transform.GetChild(i).gameObject.name != "root")
            {
                playerCharacterMainUi.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < numOfChild; i++)
        {
            if (playerCharacterInventory.transform.GetChild(i).gameObject.name != "root")
            {
                playerCharacterInventory.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        // UserData에서 불러온 Head, Body 정보를 PlayerCharacter에 적용 후 해당 오브젝트 Active
        GameObject activeHeadMainUI = playerCharacterMainUi.transform.Find("Head" + userHead).gameObject;
        GameObject activeBodyMainUI = playerCharacterMainUi.transform.Find("Body" + userBody).gameObject;
        activeHeadMainUI.SetActive(true);
        activeBodyMainUI.SetActive(true);

        GameObject activeHeadInventory = playerCharacterInventory.transform.Find("Head" + userHead).gameObject;
        GameObject activeBodyInventory = playerCharacterInventory.transform.Find("Body" + userBody).gameObject;
        activeHeadInventory.SetActive(true);
        activeBodyInventory.SetActive(true);
    }

    public void LoadMainUi()
    {
        loadingUi.SetActive(false);
        mainUi.SetActive(true);
    }


    public void CheckUserData()
    {
        Debug.Log(userData.body);
    }
}

