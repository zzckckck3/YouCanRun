using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.XR;

public class UserInfoUpdate : MonoBehaviour
{
    // 유저 닉네임 text
    public TextMeshProUGUI userNick;
    // 유저 티어 이미지 ui
    public Image userTier;
    // 유저 티어 구간 ui
    public TextMeshProUGUI userSec;
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

    void Start()
    {
        updateUserInfo();
    }

    // Update is called once per frame
    void Update()
    {
        
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
                break;
            case 1:
                // userTeer가 "bronze"인 경우 처리
                userTier.sprite = bronze;
                break;
            case 2:
                // userTeer가 "silver"인 경우 처리
                userTier.sprite = silver;
                break;
            case 3:
                // userTeer가 "gold"인 경우 처리
                userTier.sprite = gold;
                break;
            case 4:
                // userTeer가 "platinum"인 경우 처리
                userTier.sprite = platinum;
                break;
            case 5:
                // userTeer가 "diamond"인 경우 처리
                userTier.sprite = diamond;
                break;
            case 6:
                // userTeer가 "master"인 경우 처리
                userTier.sprite = master;
                break;
            case 7:
                // userTeer가 "grandmaster"인 경우 처리
                userTier.sprite = grandMaster;
                break;
            case 8:
                // userTeer가 "challenger"인 경우 처리
                userTier.sprite = challenger;
                break;
            default:
                // userTeer가 어떤 값에도 해당하지 않는 경우 처리
                userTier.sprite = challenger;
                break;
        }

        // userSection 계산
        int userSection = 3 - (userRank) / 100 % 3; // 100 단위로 섹션 구분

        // userScore 계산
        int userScore = (userRank) % 100; // 나머지 값

        // ui 적용
        userSec.text = userSection.ToString();

        userLeftScore.text = userScore + "  /  " + 100;
    }
}
