using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPlainFunc : MonoBehaviour
{
    private Vector3 rotationSpeed;
    private Vector3 relativePosition;

    private void Start()
    {
        rotationSpeed = transform.parent.GetComponent<RotationPlainProperties>()._rotation;
    }

    private void OnTriggerStay(Collider other)
    {
        // 회전 존 안에 들어왔다면
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            // 캐릭터 콘트롤러를 가져오고
            CharacterController characterController = other.gameObject.GetComponent<CharacterController>();
            if (characterController != null)
            {
                float rotationAmountThisFrame = rotationSpeed.y * Time.deltaTime; // 예상 회전량

                // 플레이어의 돌림판에 대한 상대적 위치를 구하고
                relativePosition = other.transform.position - transform.parent.position;
                // 그 상대적 위치를 돌림판이 회전한 만큼 회전시킨다.
                relativePosition = Quaternion.Euler(0, rotationAmountThisFrame, 0) * relativePosition;
                // 상대적 위치를 절대적 위치로 변환해주고
                Vector3 newPosition = transform.parent.position + relativePosition;
                // 플레이어가 움직일 방향벡터를 구한다.
                Vector3 movement = newPosition - other.transform.position;

                // 플레이어를 이동
                characterController.Move(movement);
            }
        }
    }
}
