using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlaneProperties : MonoBehaviour
{
    // 움직이는 방향
    public Vector3 movingDir = new Vector3(1, 0, 0);
    // 움직이는 속도
    public float movingSpeed = 5f;
    // 움직이는 범위
    public float movingRange = 3f;

    // 캐릭터도 같이 움직여야 하기 때문에 선언
    private CharacterController characterController;
    // 동기화를 위한 초기값 저장
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float movement;
    private float accumulatedTime = 0f;
    private Vector3 plainDelta;
    private Vector3 lastPlatformPosition;
    private bool onMovingPlane;


    private void Start()
    {
        movingDir = movingDir.normalized;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        lastPlatformPosition = initialPosition;
        onMovingPlane = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        // 들어온 물체가 플레이어고 내 플레이어라면
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            // 캐릭터도 같이 움직여야 하기 때문에 controller 할당
            characterController = other.gameObject.GetComponent<CharacterController>();
            onMovingPlane = true;
            
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 들어온 물체가 플레이어고 내 플레이어라면
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            // 캐릭터도 같이 움직여야 하기 때문에 controller 할당
            onMovingPlane = false;
        }
    }


    ////Update is called once per frame
    //private void OnTriggerStay(Collider other)
    //{
    //    // 들어온 물체가 플레이어고 내 플레이어라면
    //    if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
    //    {
    //        // 캐릭터를 바람방향으로 이동시킴
    //        characterController.Move(plainDelta);
    //        Debug.Log(plainDelta);
    //    }
    //}

    [PunRPC]
    public void ResetToInitialState()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        accumulatedTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        accumulatedTime += Time.deltaTime * movingSpeed;
        movement = Mathf.Sin(accumulatedTime) * movingRange;
        Vector3 nextPlainPosition = initialPosition + (movingDir * movement);
        plainDelta = nextPlainPosition - lastPlatformPosition;

        transform.position = nextPlainPosition;
        lastPlatformPosition = transform.position;
        if (onMovingPlane)
        {
            characterController.Move(plainDelta);
        }
    }
}
