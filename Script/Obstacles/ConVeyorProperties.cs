using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConVeyorProperties : MonoBehaviour
{
    // 바람의 세기
    public float conveyorPower = 5f;
    // 바람의 방향
    private Vector3 conveyorDir;
    private PlayerController playerController;
    private CharacterController characterController;
    private void OnTriggerEnter(Collider other)
    {
        // 들어온 물체가 플레이어고 내 플레이어라면
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            // 컨베이어의 방향은 x축의 반대
            conveyorDir = - transform.right;
            // 변수 초기화 해주고
            playerController = other.gameObject.GetComponent<PlayerController>();
            characterController = other.gameObject.GetComponent<CharacterController>();
        }
    }

    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
        // 들어온 물체가 플레이어고 내 플레이어라면
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            // 캐릭터를 바람방향으로 이동시킴
            characterController.Move(conveyorDir * conveyorPower * Time.deltaTime);
            // 팬이 지표면과 수평이 아니라면
        }
    }
}
