using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class RoomPhotonManager : MonoBehaviourPunCallbacks
{
    // RoomPhotonManager Singleton 적용
    private static RoomPhotonManager instance;

    public static RoomPhotonManager Instance
    {
        get { return instance; }
    }

    private GameObject playerCharacter;
    static Room roomInfo;

    // 레디 안내를 띄울 ui 오브젝트
    public GameObject InfoUi;
    public TextMeshProUGUI playerStatusInfo;

    // 방 정보 담을 ui 오브젝트
    public GameObject roomInfoUi;
    public TextMeshProUGUI title;
    public TextMeshProUGUI mapName;
    public TextMeshProUGUI playerCnt;
    public TextMeshProUGUI maxPlayerCnt;
    public TextMeshProUGUI captain;
    public TextMeshProUGUI mode;

    // 티어 이미지
    public Sprite iron;
    public Sprite bronze;
    public Sprite silver;
    public Sprite gold;
    public Sprite platinum;
    public Sprite diamond;
    public Sprite master;
    public Sprite grandMaster;
    public Sprite challenger;
    // 카운트 다운 ui 오브젝트
    public GameObject countUI;

    public Transform playerListParent; // 플레이어 목록을 표시할 부모 Transform
    public GameObject playerTextPrefab; // 플레이어 정보를 표시할 UI 프리팹

    // 부셔질 벽
    public GameObject wall;

    // 대기방 조명
    public GameObject lights;

    private GameObject[] obstacleToSynchro; // 장애물 리스트
    private PhotonView obstaclePhotonView; // 장애물의 PhotonView 참조

    // 타이머
    private float startTime;
    private bool isTimerRunning = false;
    private float elapsedTime;
    private float playTime = 420.0f;

    // 카운트 다운
    public float countdownTimeMs = 10.0f * 1000; // 밀리초로 표현한 카운트 다운 시간 (30초)
    private bool isCounting = false; // 카운트 다운 중 여부
    public TextMeshProUGUI timerText; // UI 텍스트 요소

    // 로컬 플레이어 완주 여부
    public int isClear = 0;

    // 게임 종료 팝업
    public GameObject gameOverPopUp;

    // 포탈 오브젝트
    public GameObject moveToListPortal;

    // 게임 결과창
    public GameObject recordUi;// 기록창 ui
    public GameObject myRecordUi; // 내 기록 창
    public GameObject playerRecordUi; // 플레이어 기록 창
    public Transform playerRecordParent; // 플레이어 기록을 표시할 부모 Transform
    public Sprite silverMedal;
    public Sprite bronzeMedal;
    public GameObject dataSotoreInfo;
    public GameObject GoListBtn;

    // 카운트 다운 오디오
    AudioSource countDownAudio;
    AudioSource FinishCountDownAudio;
    // 대기중 배경음
    public AudioSource backgroundAudioSource;
    public AudioClip newAudioClip; // 새로운 오디오 클립을 저장할 변수
    // 게임오버 오디오
    AudioSource gameOverAudio;
    // 안내창 오디오
    public AudioSource InfoTextAudio;

    void Start()
    {
        // 게임 사운드
        countDownAudio = GetComponentsInChildren<AudioSource>()[0];
        FinishCountDownAudio = GetComponentsInChildren<AudioSource>()[1];
        gameOverAudio = GetComponentsInChildren<AudioSource>()[2];

        obstacleToSynchro = GameObject.FindGameObjectsWithTag("ObstacleToSynchro");
        if (instance == null)
        {
            instance = this;
        }
        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
        }
    }
    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime = Time.time - startTime;
            // 남은 시간이 10초 일때 카운트 다운 시작
            if(playTime - elapsedTime <= 10.0f && !isCounting)
            {
                isCounting = true;
            } 
            // elapsedTime에 경과 시간이 저장됩니다.
        }

        // 카운트 다운
        if (isCounting)
        {
            // 밀리초를 감소시키고 UI에 표시
            countdownTimeMs -= Time.deltaTime * 1000;

            // 밀리초가 0 이하로 떨어지면 카운트 다운 종료
            if (countdownTimeMs <= 0)
            {
                countdownTimeMs = 0;
                isCounting = false;
            }

            // UI에 밀리초를 분:초:밀리초 형식의 문자열로 표시
            UpdateTimerText();

            // 밀리초가 0 이하로 떨어지면 원하는 작업 수행 (예: 게임 종료)
            if (!isCounting && countdownTimeMs <= 0)
            {
                isCounting = false;
                Debug.Log("게임 끝 신호 보내기");
                GameFinish();
                isTimerRunning = false;
                // 여기에 원하는 작업을 추가합니다.
                // 예를 들어, 게임 종료 또는 특정 이벤트를 실행할 수 있습니다.
            }
        }
    }

    public void DisconnectFromServer()
    {
        // Photon 서버와의 연결 해제
        PhotonNetwork.Disconnect();
    }

    // 게임 시작시 타이버 작동
    public void StartTimer()
    {
        startTime = Time.time;
        isTimerRunning = true;
    }

    //public void StopTimer()
    //{
    //    isTimerRunning = false;
    //}

    // 방 조인 또는 생성이 성공하면 호출되는 콜백
    public override void OnJoinedRoom()
    {
        Debug.Log("입장 성공");

        // 방 정보 업데이트
        roomInfo = PhotonNetwork.CurrentRoom;
        RoonInfoUpdate();
        UpdatePlayerList();

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }

        // 캐릭터 배치 코드
        Transform[] points = GameObject.Find("CteatePlayerGroup").GetComponentsInChildren<Transform>();
        int idx = UnityEngine.Random.Range(0, points.Length);
        playerCharacter = PhotonNetwork.Instantiate("PlayerCharacter", points[idx].position, points[idx].rotation, 0);

        // 방장의 상태창 출력
        if (PhotonNetwork.CurrentRoom.CustomProperties["captain"].Equals(PhotonNetwork.LocalPlayer.NickName))
        {
            playerStatusInfo.text = "아직 플레이어가 준비되지 않았습니다!";
            InfoTextAudio.Play();
        }
        // 일반 플레이어의 상태창 출력
        else
        {
            playerStatusInfo.text = "F5 를 눌러 READY를 완료해 주세요!";
            InfoTextAudio.Play();
        }
    }
    // 방 조인 또는 생성 실패 시 호출되는 콜백
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // 방 조인 또는 생성 실패 시 에러 메시지 표시
        Debug.Log("방 입장에 실패했습니다: " + message);
    }
    
    // 플레이어 입장 콜백
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoonInfoUpdate();
        UpdatePlayerList();
    }
    // 플레이어 퇴장 콜백
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 게임 시작 전에 나갔을 때.
        if(PhotonNetwork.CurrentRoom.IsOpen)
        {
            // 만약 나간 사람이 현재 방장이라면 
            if (otherPlayer.NickName.Equals(PhotonNetwork.CurrentRoom.CustomProperties["captain"])) {
                // 다음으로 들어온 사람을 반장으로 임명.
                var firstPlayer = PhotonNetwork.CurrentRoom.Players.Values.First();
                string newCaptaionNickName = firstPlayer.NickName;

                ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
                customProperties["captain"] = newCaptaionNickName; // 원하는 값을 여기에 넣으세요.

                // Hashtable을 사용하여 커스텀 속성을 업데이트합니다.
                PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
                // 방 정보가 변경이 완료된 후 ui 업데이트 : 코루틴
                StartCoroutine(WaitForCustomPropertiesToBeSetAndThenUpdate(newCaptaionNickName));
            }

            // 방장이 나간 경우가 아니라면 바로 호출.
            else
            {
                RoonInfoUpdate();
                UpdatePlayerList();
            }
            return;
        }

        // 게임 시작 후 나갔을 때
        UpdateInGamePlayerList();
        RoonInfoUpdate();
    }

    // 방 정보 ui 업데이트 코루틴 : 유저 나갔을 때.
    IEnumerator WaitForCustomPropertiesToBeSetAndThenUpdate(string newCaptaionNickName)
    {
        // SetCustomProperties가 완료될 때까지 대기합니다.
        yield return new WaitUntil(() => PhotonNetwork.CurrentRoom.CustomProperties["captain"].ToString() == newCaptaionNickName);

        // 대기 후 UI 업데이트 함수 호출
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["IsReady"] = "0"; // 업데이트
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        RoonInfoUpdate();
        UpdatePlayerList();
    }

    // 방 정보 업데이트
    public void RoonInfoUpdate()
    {
        roomInfo = PhotonNetwork.CurrentRoom;

        title.text = roomInfo.Name;
        mapName.text = "맵 : " + roomInfo.CustomProperties["map"].ToString();
        playerCnt.text = roomInfo.PlayerCount.ToString();
        maxPlayerCnt.text = roomInfo.MaxPlayers.ToString();
        captain.text = "방장 : " + roomInfo.CustomProperties["captain"];
        mode.text = roomInfo.CustomProperties["mode"].ToString()+" : "+
            roomInfo.CustomProperties["round"].ToString()+" "+"laps";
    }

    // 플레이어 리스트 ui 업데이트 : 게임 시작 전
    public int UpdatePlayerList()
    {
        int readyCnt = 0;
        roomInfo = PhotonNetwork.CurrentRoom;
        Debug.Log("리스트업데이트");
        // 기존 UI 요소를 모두 삭제
        foreach (Transform child in playerListParent)
        {
            Destroy(child.gameObject);
        }

        if (PhotonNetwork.InRoom)
        {
            int index = 0;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                // UI 요소를 동적으로 생성하고 부모에 추가
                GameObject playerCustomInfo = Instantiate(playerTextPrefab, playerListParent);

                // 플레이어 커스텀 정보 담을 TextMeshProUGUI 배열
                TextMeshProUGUI[] RoomInfoInputs =
                playerCustomInfo.GetComponentsInChildren<TextMeshProUGUI>();
                TextMeshProUGUI playerNickname = RoomInfoInputs[0];
                TextMeshProUGUI playerReadtStatus = RoomInfoInputs[1];

                // 플레이어 티어 이미지 담을 Image
                Image[] tierImg = playerCustomInfo.GetComponentsInChildren<Image>();

                // 위치 조정
                RectTransform rectTransform = playerCustomInfo.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -80f * index);
                
                // 랭크 출력--------------------------------------------
                int userRank = (int) player.CustomProperties["Rank"];
                // userTeer 계산
                //string[] tiers = { "iron", "bronze", "silver", "gold", "platinum", "diamond", "master", "grandmaster", "challenger" };
                int tierIndex = (userRank) / 300; // 300 단위로 티어 상승
                //tierIndex = Mathf.Clamp(tierIndex, 0, tiers.Length - 1); // 인덱스가 배열 범위 내에 있도록 보장
                //string userTeer = tiers[tierIndex];
                Debug.Log(userRank + " " + tierIndex);
                switch (tierIndex)
                {
                    case 0:
                        // userTeer가 "iron"인 경우 처리
                        tierImg[1].sprite = iron;
                        break;
                    case 1:
                        // userTeer가 "bronze"인 경우 처리
                        tierImg[1].sprite = bronze;
                        break;
                    case 2:
                        // userTeer가 "silver"인 경우 처리
                        tierImg[1].sprite = silver;
                        break;
                    case 3:
                        // userTeer가 "gold"인 경우 처리
                        tierImg[1].sprite = gold;
                        break;
                    case 4:
                        // userTeer가 "platinum"인 경우 처리
                        tierImg[1].sprite = platinum;
                        break;
                    case 5:
                        // userTeer가 "diamond"인 경우 처리
                        tierImg[1].sprite = diamond;
                        break;
                    case 6:
                        // userTeer가 "master"인 경우 처리
                        tierImg[1].sprite = master;
                        break;
                    case 7:
                        // userTeer가 "grandmaster"인 경우 처리
                        tierImg[1].sprite = grandMaster;
                        break;
                    case 8:
                        // userTeer가 "challenger"인 경우 처리
                        tierImg[1].sprite = challenger;
                        break;
                    default:
                        // userTeer가 어떤 값에도 해당하지 않는 경우 처리
                        tierImg[1].sprite = challenger;
                        break;
                }

                // userSection 계산
                //int userSection = 3 - (userRank) / 100 % 3; // 100 단위로 섹션 구분

                // userScore 계산
                //int userScore = (userRank) % 100; // 나머지 값

                //닉네임 표시-------------------------------------------
                playerNickname.text = " "+player.NickName;
                // 내 닉네임은 색표시
                if (player.NickName.Equals(PhotonNetwork.LocalPlayer.NickName))
                {
                    Debug.Log("색상 변경 ");
                    Color newColor = new Color(97.0f / 255.0f, 255.0f / 255.0f, 66.0f / 255.0f); // 예: RGB(97, 255, 66)
                    playerNickname.color = newColor;
                }
                string isReady = (string) player.CustomProperties["IsReady"];

                // 방장의 경우
                if (roomInfo.CustomProperties["captain"].Equals(player.NickName))
                {
                    Image captainImg = playerCustomInfo.GetComponentInChildren<Image>();
                    captainImg.gameObject.SetActive(true);
                    playerReadtStatus.text = "방 장";

                    if(isReady == "1") readyCnt++;
                    
                    index++;
                    continue;
                }
                playerReadtStatus.text = isReady == "1" ? "R E A D Y" : " ";
                if(isReady =="1")
                {
                    readyCnt++;
                }
                Debug.Log(player.NickName + "의 레디정보 : " + isReady);
                index++;
            }
        }
        return readyCnt;
    }

    public void Ready()
    {
        if (PhotonNetwork.InRoom)
        {
            // 방장 시작 반영
            if (PhotonNetwork.CurrentRoom.CustomProperties["captain"].Equals(PhotonNetwork.LocalPlayer.NickName))
            {
                // 방 인원이 4명 미만이면 시작 불가능
                if(PhotonNetwork.CurrentRoom.PlayerCount < 4) {
                    playerStatusInfo.text = "최소 4명의 플레이어가 필요합니다.";
                    InfoTextAudio.Play();
                    return;
                }

                Debug.Log("방장의 레디");
                int readyCnt = UpdatePlayerList();
                if (readyCnt == PhotonNetwork.CurrentRoom.PlayerCount - 1)
                {
                    Debug.Log("게임 시작 신호 보내기");
                    ExitGames.Client.Photon.Hashtable CaptaincustomProperties = new ExitGames.Client.Photon.Hashtable();
                    CaptaincustomProperties["IsReady"] = "1"; // 업데이트
                    PhotonNetwork.LocalPlayer.SetCustomProperties(CaptaincustomProperties);
                    return;
                }
                else
                {
                    playerStatusInfo.text = "아직 레디가 완료되지 않았습니다.";
                    InfoTextAudio.Play();
                    Debug.Log("아직 레디가 완료되지 않았습니다.");
                    return;
                }
            }
            // 플레이어 레디 반영
            string isReady = (string) PhotonNetwork.LocalPlayer.CustomProperties["IsReady"];
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties["IsReady"] = isReady == "0" ? "1" : "0"; ; // 업데이트
            playerStatusInfo.text = isReady == "0" ? " " : "F5 를 눌러 READY를 완료해 주세요";
            InfoTextAudio.Play();
            if (isReady == "0") InfoUi.SetActive(false); 
            else InfoUi.SetActive(true);
            //readyInfo.SetActive(isReady == "1");
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        }
    }

    // 플레이어 커스텀 옵션 변경 감지 콜백
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // 게임 대기중일 상태일 때
        if (PhotonNetwork.CurrentRoom.IsOpen)
        {
            // 전체 레디 수
            int readyCnt = UpdatePlayerList();

            Debug.Log("레디정보 업데이트 완료");
        
            // 만약 반장의 시작 신호가 들어온다면 
            if (targetPlayer.NickName.Equals(PhotonNetwork.CurrentRoom.CustomProperties["captain"].ToString())
                && (string) targetPlayer.CustomProperties["IsReady"] == "1" && readyCnt == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                Debug.Log("게임시작");
                PhotonNetwork.CurrentRoom.IsOpen = false; // 이제 방은 못들어오게 설정.
                StartGmae();
                return;
            }

            // 게임 시작이 가능한 조건일때 : 전부 ready 상태인 경우
            if (readyCnt >= PhotonNetwork.CurrentRoom.PlayerCount - 1 && PhotonNetwork.CurrentRoom.PlayerCount >= 4
                && PhotonNetwork.CurrentRoom.CustomProperties["captain"].Equals(PhotonNetwork.LocalPlayer.NickName))
            {
                playerStatusInfo.text = "모든 플레이어가 준비되었습니다. 방장은 F5 를 눌러 게임을 시작하세요!";
                InfoTextAudio.Play();
                Debug.Log("게임 시작 가능");
            }
            else
            {
                // 방장의 상태창 초기화
                if (PhotonNetwork.CurrentRoom.CustomProperties["captain"].Equals(PhotonNetwork.LocalPlayer.NickName))
                {
                    playerStatusInfo.text = "아직 플레이어가 준비되지 않았습니다!";
                    InfoTextAudio.Play();
                }
           
                Debug.Log("게임 시작 불가능");
            }
        }

        // 인게임 상태일 때
        else
        {
            string roundCnt = PhotonNetwork.CurrentRoom.CustomProperties["round"].ToString();
            //민약 업데이트 된 유저가 완주했을때 : 현재 라운드 수 == 설정 라운드 수
            // 첫 번째 완주자일때 : isCounting이 false -> 카운트다운 시작 x
            if ((int)changedProps["RoundCnt"] == int.Parse(roundCnt) && !isCounting)
            {
                InfoUi.SetActive(true);
                playerStatusInfo.text = "<color=#021873>" + targetPlayer.NickName + "</color>" + " 님이 완주하였습니다. 카운트 다운이 시작됩니다!";
                InfoTextAudio.Play();
                StartCountdown();
                FinishCountDownAudio.Play();
            }
            //만약 업데이트 된 유저의 남은 round가 1일때 : 1바퀴 남음
            else if((int)changedProps["RoundCnt"]+1 == int.Parse(roundCnt))
            {
                InfoUi.SetActive(true);
                playerStatusInfo.text = "<color=#021873>" + targetPlayer.NickName + "</color>"+" 님이 곧 완주 합니다..! 어서 방해하세요!!";
                InfoTextAudio.Play();
            }

            // ui 업데이트
            UpdateInGamePlayerList();
        }
    }

    // 게임 시작 신호가 왔을때 모든 플레이어에게 일어날 작업
    public void StartGmae()
    {
        foreach (GameObject obstacle in obstacleToSynchro)
        {
            Debug.Log("초기화");
            obstaclePhotonView = obstacle.GetComponent<PhotonView>();
            obstaclePhotonView.RPC("ResetToInitialState", RpcTarget.All);
        }
        CountDown.Instance.ResetObject();
        backgroundAudioSource.Pause();
        countDownAudio.Play();
        // 3초 후에 벽을 허물고 불을 끄는 메서드 호출
        Invoke("OpenSignal", 3.0f);
    }
    private void OpenSignal()
    {
        backgroundAudioSource.clip = newAudioClip;
        backgroundAudioSource.Play();

        playerStatusInfo.text = " "; // 상태창 초기화
        InfoUi.SetActive(false);

        // F5 이벤트 파괴 : 오브젝트 파괴
        GameObject objectToDestroy = GameObject.Find("ReadyFlagg");
        Destroy(objectToDestroy);

        StartCoroutine(OpenWall()); // 벽이 무너짐

        lights.SetActive(false); // 라이트 조절

        UpdateInGamePlayerList(); // 인게임 상태창으로 변경

        StartTimer(); // 타이머 시작

        //roomInfoUi.SetActive(false);a
        // 포탈 비활성화
        moveToListPortal.SetActive(false);
    }
    private IEnumerator OpenWall()
    {
        Quaternion currentRotation = wall.transform.rotation;
        while (wall.transform.rotation.x >= -0.8f)
        {
            currentRotation = wall.transform.rotation;
            wall.transform.Rotate(new Vector3(45, 0, 0)*Time.deltaTime);
            yield return null;
        }
    }

    // 플레이어 리스트 ui 업데이트 : 게임 시작 후
    public void UpdateInGamePlayerList()
    {
        roomInfo = PhotonNetwork.CurrentRoom;
        Debug.Log("리스트업데이트");
        // 기존 UI 요소를 모두 삭제
        foreach (Transform child in playerListParent)
        {
            Destroy(child.gameObject);
        }

        if (PhotonNetwork.InRoom)
        {
            int index = 0;
            // 플레이어 목록 가져오기
            Player[] players = PhotonNetwork.PlayerList;

            // "RoundCnt" 사용자 지정 속성을 기반으로 내림차순 정렬
            players = players.OrderByDescending(player => 
                (int)player.CustomProperties["RoundCnt"]).ThenBy(player => 
                (float)player.CustomProperties["RaceTime"])
                   .ToArray();

            foreach (Player player in players)
            {
                // UI 요소를 동적으로 생성하고 부모에 추가
                GameObject playerCustomInfo = Instantiate(playerTextPrefab, playerListParent);

                // 플레이어 커스텀 정보 담을 TextMeshProUGUI 배열
                TextMeshProUGUI[] RoomInfoInputs =
                playerCustomInfo.GetComponentsInChildren<TextMeshProUGUI>();
                TextMeshProUGUI playerIndex = RoomInfoInputs[2];
                TextMeshProUGUI playerNickname = RoomInfoInputs[0];
                TextMeshProUGUI playerRoundCnt = RoomInfoInputs[1];// 플레이어 현재 바퀴 수

                // 위치 조정
                RectTransform rectTransform = playerCustomInfo.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -80f * index);

                // 티어 이미지 삭제
                Image[] tierImg = playerCustomInfo.GetComponentsInChildren<Image>();
                tierImg[1].enabled = false;

                playerIndex.text = index + 1+" ";
                playerNickname.text = player.NickName;
                // 내 닉네임은 색표시
                if(player.NickName.Equals(PhotonNetwork.LocalPlayer.NickName))
                {
                    Debug.Log("색상 변경 ");
                    Color newColor = new Color(97.0f / 255.0f, 255.0f / 255.0f, 66.0f / 255.0f); // 예: RGB(97, 255, 66)
                    playerNickname.color = newColor;
                }

                int RoundCnt = (int)player.CustomProperties["RoundCnt"];
                string MapRoundCnt = PhotonNetwork.CurrentRoom.CustomProperties["round"].ToString();
                if (RoundCnt == int.Parse(MapRoundCnt))
                {
                    playerRoundCnt.text = "<color=red>"+ (index + 1) + "  st" + "</color>";
                }
                else
                {
                    playerRoundCnt.text = RoundCnt.ToString()+ " / " +  PhotonNetwork.CurrentRoom.CustomProperties["round"];
                }
                index++;
            }
        }
    }
    // 플레이어 라운드 수 업데이트 : finishLine 도착
    public void UpdatePlayerRound() 
    {
        if (PhotonNetwork.LocalPlayer.NickName == GameManager.Instance.GetUserData().nickname)
        {
            int nowRound = (int)PhotonNetwork.LocalPlayer.CustomProperties["RoundCnt"] + 1;
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties["RoundCnt"] = nowRound; // 라운드 수 업데이트
            customProperties["RaceTime"] = elapsedTime; // 플레이 타임 업데이트
            Debug.Log(elapsedTime);
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
            // 나의 완주 표시
            Debug.Log(nowRound+"내rc");
            if(nowRound == int.Parse(PhotonNetwork.CurrentRoom.CustomProperties["round"].ToString()))
            {
                isClear = 1;
            }
        }
    }

    // 카운트 다운 시작
    public void StartCountdown()
    {
        isCounting = true;
    }
    private void UpdateTimerText()
    {
        // 밀리초를 분:초:밀리초 형식의 문자열로 변환
        int seconds = Mathf.FloorToInt(countdownTimeMs / 1000);
        int milliseconds = Mathf.FloorToInt(countdownTimeMs % 1000);
        string timeString = string.Format("{0:00}:{1:000}", seconds, milliseconds);

        // UI 텍스트 업데이트
        timerText.text = timeString;
    }

    public void GameFinish()
    {
        // 게임 종료 ui
        FinishCountDownAudio.Pause();
        backgroundAudioSource.Pause();
        gameOverAudio.Play();
        playerStatusInfo.text = " ";
        InfoTextAudio.Play();
        InfoUi.SetActive(false);

        gameOverPopUp.SetActive(true);
        // 결과 데이터 저장.

        // ui 변경 : 결과창 변환
        Invoke("UpdateRecordUi", 5f);
        Debug.Log("게임끝 신호");
    }

    public void UpdateRecordUi()
    {
        gameOverAudio.Pause();
        recordUi.SetActive(true); // 기록창 on
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // 마우스 커서 락
        int myPoint = 0;
        if (PhotonNetwork.InRoom)
        {
            int index = 0;
            // 플레이어 목록 가져오기
            Player[] players = PhotonNetwork.PlayerList;

            // "RoundCnt" 사용자 지정 속성을 기반으로 내림차순 정렬
            players = players.OrderByDescending(player =>
                (int)player.CustomProperties["RoundCnt"]).ThenBy(player =>
                (float)player.CustomProperties["RaceTime"])
                   .ToArray();

            foreach (Player player in players)
            {
                // 내 기록 출력 ui
                TextMeshProUGUI[] MyRecordInfo =
                myRecordUi.GetComponentsInChildren<TextMeshProUGUI>();

                // UI 요소를 동적으로 생성하고 부모에 추가
                GameObject playerRecordInfo = Instantiate(playerRecordUi, playerRecordParent);

                // 플레이어 커스텀 정보 담을 TextMeshProUGUI 배열
                TextMeshProUGUI[] RecordInfoInputs =
                playerRecordInfo.GetComponentsInChildren<TextMeshProUGUI>();
                TextMeshProUGUI playerRank = RecordInfoInputs[1];
                TextMeshProUGUI playerNickname = RecordInfoInputs[0];
                TextMeshProUGUI playerTime = RecordInfoInputs[2]; // 플레이어 완주 시간
                TextMeshProUGUI playerPoint = RecordInfoInputs[3]; // 플레이어같 받을 포인트 

                // 위치 조정
                RectTransform rectTransform = playerRecordInfo.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -170f * index);

                playerNickname.text = player.NickName;
                // 내 닉네임은 색표시
                if (player.NickName.Equals(PhotonNetwork.LocalPlayer.NickName))
                {
                    Debug.Log("색상 변경 ");
                    Color newColor = new Color(97.0f / 255.0f, 255.0f / 255.0f, 66.0f / 255.0f); // 예: RGB(97, 255, 66)
                    playerNickname.color = newColor;
                }

                
                
                // 플레이 타임
                float PlayTime = (float)player.CustomProperties["RaceTime"] * 1000;

                // 돈 바퀴 수
                int RoundCnt = (int)player.CustomProperties["RoundCnt"];
                // 방 설정 바퀴 수
                string MapRoundCnt = PhotonNetwork.CurrentRoom.CustomProperties["round"].ToString();
                Image[] imageComponents = playerRecordInfo.GetComponentsInChildren<Image>();
                if (RoundCnt == int.Parse(MapRoundCnt)) // 완주한 사람만 시간 출력
                {
                    // 밀리초를 분:초:밀리초 형식의 문자열로 변환
                    int minutes = Mathf.FloorToInt(PlayTime / (60 * 1000));
                    int seconds = Mathf.FloorToInt((PlayTime % (60 * 1000)) / 1000);
                    int milliseconds = Mathf.FloorToInt(PlayTime % 1000);
                    string timeString = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);


                    playerTime.text = timeString;

                    int point = 10*players.Length - index * 10;
                    if (index == 0) point += (players.Length/2)*10;
                    playerPoint.text = "+" + point;

                    // 등수별 이미지 넣기.
                    if (index == 1)
                    {
                        imageComponents[1].sprite = silverMedal;
                    }
                    else if (index == 2)
                    {
                        imageComponents[1].sprite = bronzeMedal;
                    }
                    else if (index > 2)
                    {
                        imageComponents[1].enabled = false;
                        playerRank.text = (index+1).ToString();
                    }

                    // 내 정보일때
                    if (player.NickName.Equals(PhotonNetwork.LocalPlayer.NickName))
                    {
                        MyRecordInfo[0].text = (index + 1).ToString();
                        MyRecordInfo[1].text = player.NickName;
                        MyRecordInfo[2].text = timeString;
                        MyRecordInfo[3].text = "+" + point;
                        myPoint = point;
                    }

                }
                else
                {
                    imageComponents[1].enabled = false;
                    // 등수 
                    playerRank.text = "-";
                    // 포인트 처리
                    playerTime.text = "RETIRE";
                    playerPoint.text = "-10";
                    // 내 정보일때
                    if (player.NickName.Equals(PhotonNetwork.LocalPlayer.NickName))
                    {
                        MyRecordInfo[0].text = "-";
                        MyRecordInfo[1].text = player.NickName;
                        MyRecordInfo[2].text = "RETIRE";
                        MyRecordInfo[3].text = "-10";
                        myPoint = -10;
                    }
                }

                index++;
            }
        }
        // 데이터 저장
        UpdateIsFirstPlayAndLevelToFireBase(myPoint);
        // 30초 후엔 강제이동
        Invoke("AfterFinish", 30.0f);
    }

    public async Task<bool> UpdateIsFirstPlayAndLevelToFireBase(int myPoint)
    {
        Task<bool> updateIsFirstPlayTask = FirebaseAuthManager.Instance.UpdateIsFirstPlayToFireBase(1);

        Task<bool> updateLevelTask = FirebaseAuthManager.Instance.UpdateLevelToFireBase(
            GameManager.Instance.userData.level + myPoint < 0 ? 0 : myPoint + GameManager.Instance.userData.level
            );

        bool isFirstPlaySuccess = await updateIsFirstPlayTask;
        bool levelSuccess = await updateLevelTask;

        // 두 작업이 모두 완료되었을 때 실행할 코드
        if (isFirstPlaySuccess && levelSuccess)
        {
            // 두 작업이 모두 성공한 경우
            Debug.Log("db저장 성공");
            WaitingDataSave();
            return true;
        }
        else
        {
            // 두 작업 중 하나라도 실패한 경우
            Debug.Log("db저장 실패");
            WaitingDataSave();
            return false;
        }
    }

    public void WaitingDataSave()
    {
        dataSotoreInfo.SetActive(false);
        GoListBtn.SetActive(true);
    }
    // 결과 창 보인 후 30초 이후 자동 이동
    public void AfterFinish()
    {
        PhotonNetwork.LeaveRoom();
        // 게임방 리스트로
        PhotonNetwork.LoadLevel("RoomList");
    }
}
