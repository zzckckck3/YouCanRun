using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using ExitGames.Client.Photon;
using System.Reflection;

public class ListPhotonManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI lobbyPlayerCnt;
    public Transform Rooms;

    public GameObject makeRoomUi; // 방만들기 팝업
    public GameObject noTitlePopUp; // 방제 없을때 팝업

    public GameObject failUi; // 입장실패 팝업
    public TextMeshProUGUI failInfo; // 실패 이유

    // 방 생성 입력
    public TextMeshProUGUI title;
    public TextMeshProUGUI maxPlayerCnt;
    public TextMeshProUGUI gameMode;
    public TextMeshProUGUI mapName;
    public TextMeshProUGUI Round;

    public Sprite noRoomImg; // 변경할 이미지
    public Sprite yesRoomImg; // 변경할 이미지
    public Sprite startRoomImg; // 변경할 이미지

    public List<RoomData> RoomDatas;
    void Start()
    {
        RoomDatas = new List<RoomData>();
        Cursor.lockState = CursorLockMode.None; // 마우스 커서 락
        Cursor.visible = true;
        if (PhotonNetwork.IsConnected)
        {
            OnConnectedToMaster();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("로비 입장 유무: " + PhotonNetwork.InLobby);
        // 플레이어 커스텀 옵션: ready 상태, 바퀴 수 초기화.
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["IsReady"] = "0"; // 공통 CustomProperties : Ready 정보 설정
        customProperties["RoundCnt"] = 0;  // 현재 round 수 설정
        customProperties["RaceTime"] = 0f;  // 특정 라운드 클리어 및 완주 시간 설정
        customProperties["Rank"] = GameManager.Instance.userData.level;
        PhotonNetwork.LocalPlayer.CustomProperties = customProperties;
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 엡데이트된 방 개수를 출력
        Debug.Log($"업데이트방 개수: {roomList.Count}\n\n");

        // 각 방의 정보 업데이트
        foreach (RoomInfo room in roomList)
        {
            // 로비방에 대한 정보는 인원수만 파악
            if (room.Name == "Lobby")
            {
                lobbyPlayerCnt.text = room.PlayerCount.ToString();
                continue;
            }

            // 업데이트 방인지
            bool isUpdate = false;

            for(int i = 0; i<RoomDatas.Count; i++)
            {
                // 기존에 존재하던 방 : 방 업데이트 또는 삭제
                if (RoomDatas[i].RoomName.Equals(room.Name))
                {
                    UpdateOrDeletRoomDatas(i, room);
                    isUpdate = true;
                    break;
                }
            }

            // 새로 추가된 방이며 존재하는 방일때(인게임 플레이어가 1명 이상)
            if (!isUpdate && room.PlayerCount>0)
            {
                addRoomDatas(room);
            }
        }
        UpdateRoomList();
    }

    // 방 정보 추가
    public void addRoomDatas(RoomInfo room)
    {
        RoomData roomData = new RoomData
        {
            RoomName = room.Name,
            GameMode = (string)room.CustomProperties["mode"],
            Round = (string)room.CustomProperties["round"],
            Map = (string)room.CustomProperties["map"],
            Status = room.IsOpen,
            CurrentPlayers = room.PlayerCount,
            MaxPlayers = room.MaxPlayers
        };
        RoomDatas.Add(roomData);
    }

    // 방 정보 업데이트 : 데이터 변경 또는 삭제
    public void UpdateOrDeletRoomDatas(int index, RoomInfo room)
    {
        // 플레이어가 0명이면 사라진 방
        if(room.PlayerCount == 0)
        {
            RoomDatas.RemoveAt(index);
            return;
        }

        // 플레이거가 존재하는 방 정보 변경 : 플레이어 입장, 플레이어 퇴장, 게임 시작 등
        RoomData roomData = new RoomData
        {
            RoomName = room.Name,
            GameMode = (string)room.CustomProperties["mode"],
            Round = (string)room.CustomProperties["round"],
            Map = (string)room.CustomProperties["map"],
            Status = room.IsOpen,
            CurrentPlayers = room.PlayerCount,
            MaxPlayers = room.MaxPlayers
        };
        RoomDatas[index] = roomData;
    }

    // 방정보 UI 출력 업데이트
    public void UpdateRoomList()
    {
        Debug.Log("업데이트룸리스트");
        Debug.Log(RoomDatas.Count);

        // 방의 개수만큼 ui 업데이트
        for(int i = 0; i<RoomDatas.Count; i++)
        {
            // i번째 방 정보 출력 열 오브젝트
            TextMeshProUGUI[] RoomInfoInputs = 
            Rooms.GetChild(i).GetComponentsInChildren<TextMeshProUGUI>();
            // i번째 방 버튼 오브젝트
            Button button = Rooms.GetChild(i).GetComponentInChildren<Button>();

            
            RoomInfoInputs[0].text = RoomDatas[i].RoomName; // 방제
            RoomInfoInputs[1].text = RoomDatas[i].GameMode + " : " // 게임모드 + 바퀴 수
                + RoomDatas[i].Round + " "+ "libs";
            RoomInfoInputs[2].text = RoomDatas[i].Map; // 맵 이름
            // 방 상태
            RoomInfoInputs[3].text = RoomDatas[i].Status ? "대기중인 방입니다." : "게임중인 방입니다.";

            // 버튼의 Image 컴포넌트 가져오기p
            Image buttonImage = button.image;
            if(RoomDatas[i].Status)
            {
                // 새 이미지로 버튼의 이미지 변경
                buttonImage.sprite = yesRoomImg;
                // 버튼 텍스트 
                RoomInfoInputs[7].text = "들어가기";
                }
            else
            {
                buttonImage.sprite = startRoomImg;
                RoomInfoInputs[7].text = "입장불가";
            }
            
            RoomInfoInputs[4].text = RoomDatas[i].CurrentPlayers.ToString(); // 현재 유저 수
            RoomInfoInputs[6].text = RoomDatas[i].MaxPlayers.ToString();     // 최대 유저 수
        }

        // 기존 방 ui 초기화 : 3개에서 2개로 줄어든 경우 3번째 방 정보는 사라지지 않음(최대 출력 5개)
        for(int i = RoomDatas.Count; i<5; i++)
        {
            // i번째 방 정보 출력 열 오브젝트
            TextMeshProUGUI[] RoomInfoInputs =
            Rooms.GetChild(i).GetComponentsInChildren<TextMeshProUGUI>();
            // i번째 방 버튼 오브젝트
            Button button = Rooms.GetChild(i).GetComponentInChildren<Button>();

            RoomInfoInputs[0].text = "Room"+(i+1);//타이틀
            RoomInfoInputs[1].text = " "; // 게임모드+바퀴 수
            RoomInfoInputs[2].text = " "; // 맵 이름
            RoomInfoInputs[3].text = "현재 방이 비었습니다."; // 방 상태
            RoomInfoInputs[7].text = "방 만들기"; // 버튼 텍스트
            RoomInfoInputs[4].text = "0";
            RoomInfoInputs[6].text = "0";

            // 버튼의 Image 컴포넌트 가져오기
            Image buttonImage = button.image;

            // 새 이미지로 버튼의 이미지 변경
            buttonImage.sprite = noRoomImg;
        }
    }
    
    public void EnterLobby()
    { 
        SceneManager.LoadScene("Lobby");
    }

    public void EnterRoom(int roomNum)
    {
        TextMeshProUGUI[] clickedRoom =
                Rooms.GetChild(roomNum).GetComponentsInChildren<TextMeshProUGUI>();
        // 방이 없는 경우
        if (clickedRoom[6].text.Equals("0"))
        {
            makeRoomUi.SetActive(true);
        }
        //else if (clickedRoom[7].text == "입장불가") return;
        else
        {
            string roomName = clickedRoom[0].text;
            PhotonNetwork.JoinRoom(roomName);
        }
    }
    // 방 입장 실패 처리
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("방 입장 실패: " + message);
        failUi.SetActive(true);
        failInfo.text = "방 입장 실패: " + message;
        // 입장 실패에 대한 사용자 지정 처리 추가
        PhotonNetwork.JoinLobby();
    }

    public void MakeRoom()
    {
        // 방제 입력 확인
        if(title.text.Length == 1)
        {
            noTitlePopUp.SetActive(true);
            Debug.Log("방제를 입력하세요");
            return;
        }
        if (PhotonNetwork.InLobby)
        {
            RoomOptions ro = new RoomOptions();
            ro.MaxPlayers = int.Parse(maxPlayerCnt.text);
            ro.IsOpen = true;
            ro.IsVisible = true;

            // 커스텀 속성 추가
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties["map"] = mapName.text;// mapNum 커스텀 속성을 1로 설정
            customProperties["captain"] = GameManager.Instance.userData.nickname; // 방장정보 커스텀 속성
            customProperties["mode"] = "개인전"; // 게임모드 커스텀 속성 : 현재 개인전만 있음
            customProperties["round"] = Round.text; // 게임 라운드 수 : 몇 바퀴 돌건지
            ro.CustomRoomProperties = customProperties;
            ro.CustomRoomPropertiesForLobby = new string[] { "map", "captain", "mode", "round" }; // 로비에서 mapNum 속성을 볼 수 있도록 설정

            PhotonNetwork.JoinOrCreateRoom(title.text, ro, null);
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Create Room!");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
        if (mapName.text.Equals("점심 먹으러 가는 길"))
        {
            SceneManager.LoadScene("NewImHyeonRacingMap");
        }
        else if (mapName.text.Equals("스카이 퐁퐁"))
        {
            SceneManager.LoadScene("MinWooRacingMap");
        }
        else if (mapName.text.Equals("우정 파괴"))
        {
            SceneManager.LoadScene("Makgora");
        }
        else if (mapName.text.Equals("돌돌이맵"))
        {
            SceneManager.LoadScene("SpinningRacingMap");
        }
        //SceneManager.LoadScene("DemoRacingGame");
    }
}
