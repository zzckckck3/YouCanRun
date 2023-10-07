using Photon.Pun;
using UnityEngine;
using System.Collections;

public class TrampolineProperties : MonoBehaviour
{
    public float bounceCoefficient = 60f; // 탄성 계수.
    // 한번에 하나의 코루틴만 실행 why? 장애물에 의해서 튕겨나가는 코루틴이 실행되다가 트램펄린에 닿으면
    // 트램펄린 코루틴이 실행되어야함.
    private CoroutineManager CM;
    private void Start()
    {
        CM = FindObjectOfType<CoroutineManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 트램펄린과 부딪혔다면
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            other.transform.GetComponent<PlayerController>().anim.SetBool("IsKnockDown", false);
            CharacterController characterController = other.gameObject.GetComponent<CharacterController>();
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            PlayerStatus playerStatus = other.gameObject.GetComponent<PlayerStatus>();
            
            if (characterController != null)
            {
                Vector3 bounceDir = transform.parent.up;
                if (CM.GetCoroutine() != null)
                {
                    CM.NowStopCoroutine(); // 기존 코루틴을 종료
                    
                }
                IEnumerator bounceCoroutine = Bounce(characterController, playerController, playerStatus, bounceDir, bounceCoefficient);
                CM.SetCoroutineInstance(bounceCoroutine);
                CM.StartStoredCoroutine();
            }
        }
    }

    private IEnumerator Bounce(CharacterController characterController,PlayerController playerController,PlayerStatus playerStatus, Vector3 direction, float force)
    {
        playerController.SetNowJumpSpeed(0);
        // 코루틴 해제 추가
        
        while (!playerStatus.IsGrounded() && CM.GetcoroutineInstance()!=null)
        {
            characterController.Move(direction * force * Time.deltaTime);
            yield return null;
        }
        CM.SetCoroutineInstance(null);
    }

}