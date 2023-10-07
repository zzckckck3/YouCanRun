using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceProperties : MonoBehaviour
{
    public float frictionForce = 5f; // 마찰 계수. 이 값은 에디터에서 조정할 수 있습니다.

    private void OnTriggerStay(Collider other)
    {
        // 아이스존 안에 들어왔다면
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            CharacterController characterController = other.gameObject.GetComponent<CharacterController>();
            if (characterController != null )
            {
                // z축방향으로 맞춰주고
                Vector3 iceDir = new Vector3(0, 0, frictionForce);
                // 그 z축방향을 플레이어가 바라보는 방향으로 조정
                iceDir = other.transform.TransformDirection(iceDir);
                // 플레이어를 밀어준다
                characterController.Move(iceDir*Time.deltaTime);
            }
        }
    }
}
