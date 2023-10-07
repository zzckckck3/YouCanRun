using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanProperties : MonoBehaviour
{
    // 바람의 세기
    public float fanPower = 10f; 
    // 바람의 방향
    private Vector3 windDirection;
    private float zRotation;
    private PlayerController playerController;
    private CharacterController characterController;
    // 팬이 땅과 수평인가
    private Boolean fanHorizontal;
    private void OnTriggerEnter(Collider other)
    {
        // 들어온 물체가 플레이어고 내 플레이어라면
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            // 바람의 방향은 팬 시스템의 x축
            windDirection = transform.right;
            // 지면과 수편한지
            fanHorizontal = true;
            // 회전각 체크
            float zRotation = transform.eulerAngles.z;
            // 변수 초기화 해주고
            playerController = other.gameObject.GetComponent<PlayerController>();
            characterController = other.gameObject.GetComponent<CharacterController>();
            if (playerController != null)
            {
                // 회전각이 지표면과 수평이 아니라면
                if (zRotation > 0 && zRotation < 180)
                {
                    // false채크
                    fanHorizontal = false;
                }
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        // 들어온 물체가 플레이어고 내 플레이어라면
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            // 캐릭터를 바람방향으로 이동시킴
            characterController.Move(windDirection * fanPower * Time.deltaTime);
            // 팬이 지표면과 수평이 아니라면
            if (!fanHorizontal)
            {
                // 캐릭터가 두둥실 떠야하므로 nowJumpSpeed는 0
                playerController.SetNowJumpSpeed(0);
            }
        }
    }
}
