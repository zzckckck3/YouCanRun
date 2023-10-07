using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;

public class ChatManager : MonoBehaviourPunCallbacks
{
    // 채팅 입력창
    public TMP_InputField inputField;
    // 텍스트 내용 프리펩
    public GameObject textChatPrefab;
    // 채팅창
    public Transform parentContent;
    private string nickname;
    public PhotonView pv;
    // 채팅 입력, 보내기 버튼 부모 오브젝트
    public GameObject chatInput;

    private void Start()
    {
        // 닉네임 호출
        nickname = GameManager.Instance.userData.nickname;
        Debug.Log(nickname);
    }

    private void Update()
    {
        KeyDownEnter();
    }

    
    // Input 창 옆에 버튼 누를 때 호출되는 함수
    public void OnEndEditEventMethod()
    {
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UpdateChat();
        }
    }

    public void UpdateChat()
    {
        // 채팅창에 아무것도 입력 안할 시 종료
        if (inputField.text.Equals(""))
        {
            return;
        }
        // 닉네임 : 대화내용
        string msg = $"{nickname} : {inputField.text}";
        Debug.Log(msg + " 입력 메세지~~~");
        // 채팅 RPC 호출
        pv.RPC("RPC_Chat", RpcTarget.All, msg);
        // 채팅 입력창 내용 초기화
        inputField.text = "";
    }

    void AddChatMessage(string message)
    {
        Debug.Log("채팅애드챗메세지");
        // 채팅 내용 출력을 위해 TEXT UI 생성
        GameObject clone = Instantiate(textChatPrefab, parentContent);
        // 채팅 입력창에 있는 내용 채팅창에 출력
        clone.GetComponent<TextMeshProUGUI>().text = message;
    }

    public void KeyDownEnter()
    {
        // 채팅창 비활성화 시
        if (Input.GetKeyDown(KeyCode.Return) && !chatInput.activeSelf)
        {
            // 채팅 입력 창 초기화
            inputField.text = "";
            // 부모 오브젝트 활성화
            chatInput.SetActive(true);
            // 채팅 입력창에 커서 활성화
            inputField.ActivateInputField();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && chatInput.activeSelf) // 채팅 부모 오브젝트 활성화 후 엔터 시 
        {
            // 부모 오브젝트 비활성화
            chatInput.SetActive(false);
        } 
        else if (!inputField.isFocused) // 채팅창에서 커서가 옮겨질 시 ex 화면 클릭
        {
            // 부모 오브젝트 비활성화
            chatInput.SetActive(false);
        }
    }

    [PunRPC]
    void RPC_Chat(string message)
    {
        Debug.Log("RPC메세지");
        AddChatMessage(message);
    }

}
