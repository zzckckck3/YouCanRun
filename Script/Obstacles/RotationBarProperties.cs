using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationBarProperties : MonoBehaviour
{
    public Vector3 _rotation;
    public float power = 60f;
    private CoroutineManager CM;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        CM = FindObjectOfType<CoroutineManager>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(_rotation * Time.deltaTime);
    }

    [PunRPC]
    public void ResetToInitialState()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 빙글빙글 장애물과 부딪혔다면
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            CharacterController characterController = other.gameObject.GetComponent<CharacterController>();
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            PlayerStatus playerStatus = other.gameObject.GetComponent<PlayerStatus>();
            if (characterController != null)
            { 
                // 기존에 실행되고 있는 코루틴이 있다면
                if (CM.GetCoroutine() != null)
                {
                    CM.NowStopCoroutine(); // 기존 코루틴을 종료
                }

                Vector3 cylinderDirection = (other.transform.position - transform.position).normalized;
                Vector3 centerDirection = transform.forward.normalized;
                Vector3 bounceDirection = (centerDirection - cylinderDirection).normalized;
                bounceDirection.y = 0;
                IEnumerator bounceCoroutine = Bounce(characterController, playerController, playerStatus, bounceDirection, power);
                CM.SetCoroutineInstance(bounceCoroutine);
                CM.StartStoredCoroutine();

            }
        }
    }

    private IEnumerator Bounce(CharacterController characterController, PlayerController playerController, PlayerStatus playerStatus, Vector3 direction, float force)
    {
        playerController.anim.SetBool("IsKnockDown", true);
        playerStatus.SetIsControll(false);
        playerController.SetNowJumpSpeed(10f);
        bool first = true;

        while ((!playerStatus.IsGrounded() && CM.GetcoroutineInstance() != null)||first)
        {
            characterController.Move(direction * force * Time.deltaTime);
            first = false;
            yield return null;
        }
        playerController.anim.SetBool("IsKnockDown", false);
        CM.SetCoroutineInstance(null);
    }
}
