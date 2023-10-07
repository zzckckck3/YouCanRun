using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string version = "1.0f"; 
    private string userId;
    public GameObject loadingCanvas; // 로딩 캔버스를 참조하기 위한 변수
    public override void OnLeftRoom()
    {
        Debug.Log("마스터 서버 콜백 호출");
        if (PhotonNetwork.IsConnectedAndReady)
        {
            OnConnectedToMaster();
        }
    }

    private void Awake()
    {
        loadingCanvas.SetActive(true); // 로딩 캔버스 활성화
        userId = Random.Range(1, 10).ToString();
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.NickName = userId;


        Debug.Log(PhotonNetwork.SendRate);
        PhotonNetwork.ConnectUsingSettings();
        FirebaseAuthManager.Instance.RetrieveUserData(); 
        FirebaseAuthManager.Instance.GetUserDataFromFireBase();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create Room");
    }

    public override void OnConnectedToMaster()
    {
        // 로비에서 바로 실행 시 로그인 페이지가 없어서 테스트가 안됨 주의!
        PhotonNetwork.NickName = GameManager.Instance.GetUserData().nickname;

        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("로비 입장 유무: " + PhotonNetwork.InLobby);
        loadingCanvas.SetActive(false); // 로딩 캔버스 비활성화

    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Create Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    public void DisconnectFromServer()
    {
        // Photon 서버와의 연결 해제
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // 연결이 해제되면 호출될 콜백 함수
        Debug.Log("연결이 해제되었습니다. 원인: " + cause.ToString());
    }

}
