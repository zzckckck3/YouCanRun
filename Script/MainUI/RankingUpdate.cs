using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FirebaseAuthManager;

public class RankingUpdate : MonoBehaviour
{
    // 유저 닉네임 text
    public TextMeshProUGUI userNick;
    // 유저 티어 이미지 ui
    public Image userTier;
    // 유저 티어 구간 ui
    public TextMeshProUGUI userSec;
    public string userSecText;
    // 유저 잔여 score ui
    public TextMeshProUGUI userLeftScore;

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

    // 랭크 데이터
    public List<FirebaseAuthManager.UserRank> userRankData;
    // 랭크 데이터 업로드 됬는지
    public bool isRankUpdate = false;

    // 랭크 데이터 출력 ui
    public Transform playerListParent; // 플레이어 목록을 표시할 부모 Transform
    public GameObject playerTextPrefab; // 플레이어 정보를 표시할 UI 프리팹

    // 내 랭크 출력 ui
    public GameObject myTextPrefab; // 플레이어 정보를 표시할 UI 프리팹

    void Start()
    {
        updateUserInfo();
        //userRankData = FirebaseAuthManager.Instance.RetrieveUserData();
    }

    // Update is called once per frame
    void Update()
    {
        if(FirebaseAuthManager.Instance.userRanks != null && !isRankUpdate)
        {
            userRankUpdate();
            isRankUpdate = true;
        }
    }
    public void userRankUpdate()
    {
        Debug.Log("업데이트 ui 출력");
        List<UserRank> ranks = FirebaseAuthManager.Instance.userRanks;
        ranks.Sort((x, y) => y.level.CompareTo(x.level));

        // UI 요소를 동적으로 생성하고 부모에 추가
        int index = 0;
        // 내 정보일때 담을 오브젝트
        TextMeshProUGUI[] MyRankInputs =
            myTextPrefab.GetComponentsInChildren<TextMeshProUGUI>();
        Image[] myTierImg = myTextPrefab.GetComponentsInChildren<Image>();

        foreach (FirebaseAuthManager.UserRank userRank in ranks)
        {
            string nickName = userRank.nickname;
            int level = userRank.level;
    
            GameObject playerCustomInfo = Instantiate(playerTextPrefab, playerListParent);

            // 플레이어 커스텀 정보 담을 TextMeshProUGUI 배열
            TextMeshProUGUI[] RoomInfoInputs =
            playerCustomInfo.GetComponentsInChildren<TextMeshProUGUI>();
            TextMeshProUGUI RankIndex = RoomInfoInputs[0];
            TextMeshProUGUI playerNickname = RoomInfoInputs[1];
            TextMeshProUGUI playerTierText = RoomInfoInputs[2];

            // 플레이어 티어 이미지 담을 Image
            Image[] tierImg = playerCustomInfo.GetComponentsInChildren<Image>();

            // 위치 조정
            RectTransform rectTransform = playerCustomInfo.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -70f * index);

            // userTeer 계산
            int tierIndex = (level) / 300; // 300 단위로 티어 상승

            Debug.Log(userRank + " " + tierIndex);
            string tierName = " ";
            switch (tierIndex)
            {
                case 0:
                    // userTeer가 "iron"인 경우 처리
                    tierImg[3].sprite = iron;
                    if (nickName.Equals(GameManager.Instance.userData.nickname)) myTierImg[3].sprite = iron;
                    tierName = "Iron";
                    break;
                case 1:
                    // userTeer가 "bronze"인 경우 처리
                    tierImg[3].sprite = bronze;
                    if (nickName.Equals(GameManager.Instance.userData.nickname)) myTierImg[3].sprite = bronze;
                    tierName = "Bronze";
                    break;
                case 2:
                    // userTeer가 "silver"인 경우 처리
                    tierImg[3].sprite = silver;
                    if (nickName.Equals(GameManager.Instance.userData.nickname)) myTierImg[3].sprite = silver;
                    tierName = "Silver";
                    break;
                case 3:
                    // userTeer가 "gold"인 경우 처리
                    tierImg[3].sprite = gold;
                    if (nickName.Equals(GameManager.Instance.userData.nickname)) myTierImg[3].sprite = gold;
                    tierName = "Gold";
                    break;
                case 4:
                    // userTeer가 "platinum"인 경우 처리
                    tierImg[3].sprite = platinum;
                    if (nickName.Equals(GameManager.Instance.userData.nickname)) myTierImg[3].sprite = platinum;
                    tierName = "Platinum";
                    break;
                case 5:
                    // userTeer가 "diamond"인 경우 처리
                    tierImg[3].sprite = diamond;
                    if (nickName.Equals(GameManager.Instance.userData.nickname)) myTierImg[3].sprite = diamond;
                    tierName = "Diamond";
                    break;
                case 6:
                    // userTeer가 "master"인 경우 처리
                    tierImg[3].sprite = master;
                    if (nickName.Equals(GameManager.Instance.userData.nickname)) myTierImg[3].sprite = master;
                    tierName = "Master";
                    break;
                case 7:
                    // userTeer가 "grandmaster"인 경우 처리
                    tierImg[3].sprite = grandMaster;
                    if (nickName.Equals(GameManager.Instance.userData.nickname)) myTierImg[3].sprite = grandMaster;
                    tierName = "GrandMaster";
                    break;
                case 8:
                    // userTeer가 "challenger"인 경우 처리
                    tierImg[3].sprite = challenger;
                    if (nickName.Equals(GameManager.Instance.userData.nickname)) myTierImg[3].sprite = challenger;
                    tierName = "Challenger";
                    break;
                default:
                    // userTeer가 어떤 값에도 해당하지 않는 경우 처리
                    tierImg[3].sprite = challenger;
                    if (nickName.Equals(GameManager.Instance.userData.nickname)) myTierImg[3].sprite = challenger;
                    tierName = "Challenger";
                    break;
            }

            // userSection 계산
            int userSection = 3 - (level) / 100 % 3; // 100 단위로 섹션 구분

            // userScore 계산
            int userScore = (level) % 100; // 나머지 값

            // 순위 표시
            RankIndex.text = (index + 1).ToString();
            //닉네임 표시-------------------------------------------
            playerNickname.text = nickName;
            // 티어 텍스트 표시 
            playerTierText.text = tierName + " " + userSection + "    " + userScore+"points";

            // 내 정보라면
            if (nickName.Equals(GameManager.Instance.userData.nickname))
            {
                MyRankInputs[0].text = (index + 1).ToString();
                MyRankInputs[1].text = nickName;
                MyRankInputs[2].text = tierName + " " + userSection + "    " + userScore + "points";
            }
            index++;
        }
    }
    public void updateUserInfo()
    {
        userNick.text = GameManager.Instance.userData.nickname;
        // 랭크 출력--------------------------------------------
        int userRank = GameManager.Instance.userData.level;
        // userTeer 계산
        int tierIndex = (userRank) / 300; // 300 단위로 티어 상승

        Debug.Log(userRank + " " + tierIndex);
        switch (tierIndex)
        {
            case 0:
                // userTeer가 "iron"인 경우 처리
                userTier.sprite = iron;
                userSecText = "Iron";
                break;
            case 1:
                // userTeer가 "bronze"인 경우 처리
                userTier.sprite = bronze;
                userSecText = "Bronze";
                break;
            case 2:
                // userTeer가 "silver"인 경우 처리
                userTier.sprite = silver;
                userSecText = "Silver";
                break;
            case 3:
                // userTeer가 "gold"인 경우 처리
                userTier.sprite = gold;
                userSecText = "Gold";
                break;
            case 4:
                // userTeer가 "platinum"인 경우 처리
                userTier.sprite = platinum;
                userSecText = "Platinum";
                break;
            case 5:
                // userTeer가 "diamond"인 경우 처리
                userTier.sprite = diamond;
                userSecText = "Diamond";
                break;
            case 6:
                // userTeer가 "master"인 경우 처리
                userTier.sprite = master;
                userSecText = "Master";
                break;
            case 7:
                // userTeer가 "grandmaster"인 경우 처리
                userTier.sprite = grandMaster;
                userSecText = "GrandMaster";
                break;
            case 8:
                // userTeer가 "challenger"인 경우 처리
                userTier.sprite = challenger;
                userSecText = "Challenger";
                break;
            default:
                // userTeer가 어떤 값에도 해당하지 않는 경우 처리
                userTier.sprite = challenger;
                userSecText = "Challenger";
                break;
        }

        // userSection 계산
        int userSection = 3 - (userRank) / 100 % 3; // 100 단위로 섹션 구분

        // userScore 계산
        int userScore = (userRank) % 100; // 나머지 값

        // ui 적용
        userSec.text = userSecText + " " + userSection.ToString();

        userLeftScore.text = userScore + "  /  " + 100;
    }
}
