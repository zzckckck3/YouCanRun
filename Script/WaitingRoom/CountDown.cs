using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    // RoomPhotonManager Singleton 적용
    private static CountDown instance;

    public static CountDown Instance
    {
        get { return instance; }
    }

    // 부셔질 벽
    public GameObject wall;

    // 대기방 조명
    public GameObject lights;

    private Vector3 initialScale;
    public TextMeshProUGUI count;
    public GameObject countUI;
    private float timeSinceLastReset;
    private float resetInterval = 1f; // 1초에 한 번 resetAnim() 호출
    private float scaleSpeed = 10.0f; // 크기 변화 속도
    private string[] message = {"4", "3", "2", "1", "START!"};
    private bool isScaling = true; // 크기 변화 중 여부를 나타내는 플래그
    private float scaleDuration = 0.1f; // 크기 변화 지속 시간

    public int gamesignal;
    public int index;
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        // 초기 크기 저장
        index = 0;
        gamesignal = 0;
        initialScale = transform.localScale;
        timeSinceLastReset = resetInterval; // 초기화 후 바로 호출
    }

    private void Update()
    {
        // 1초에 한 번 resetAnim() 호출
        if(gamesignal == 1)
        {
            timeSinceLastReset += Time.deltaTime;
            if (timeSinceLastReset >= resetInterval)
            {
                ResetObject();
                timeSinceLastReset = 0;
            }

            if (isScaling)
            {
                // 크기를 점점 키우기
                float scaleFactor = 1 + scaleSpeed * Time.deltaTime;
                transform.localScale *= scaleFactor;

                // 일정 시간이 지나면 크기 변화를 멈춤
                if (timeSinceLastReset >= scaleDuration)
                {
                    isScaling = false;
                }
            }
        }
    }
    // 카운트 크기 초기화 및 업데이트
    public void ResetObject()
    {
        gamesignal = 1;
        Debug.Log(index);
        if (index == 5)
        {
            index++;
            return;
        }
        if (index == 6)
        {
            countUI.SetActive(false);
            gamesignal = 0;
            return;
        }
        // 초기 크기로 되돌리기
        transform.localScale = initialScale;
        // 다음 카운트 업데이트
        count.text = message[index++];
        isScaling = true; // 크기 변화 재시작
    }
    
}
