using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnListCamera : MonoBehaviour
{
    public static OnListCamera Instance { get; private set; }

    public Camera mainCamera;

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // 이미 인스턴스가 존재하면 새로 생성된 인스턴스를 파괴
            Destroy(gameObject);
        }
    }

    // 카메라 활성화 메서드
    public void ActivateMainCamera()
    {
        if (mainCamera != null)
        {
            mainCamera.enabled = true;
        }
    }
}
