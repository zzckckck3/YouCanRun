using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{
    public Vector3 _position;

    private float truckSpeed=30f;

    public float power = 60f;
    private CoroutineManager CM;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // 이동 방향을 public으로 설정해주기
    public float xDir;
    public float yDir;
    public float zDir;

    // 나중에 범위 조정이 로컬에서 필요할 시 사용
    //public float xVariableLow;
    //public float xVariableHigh;
    //public float yVariableLow;
    //public float yVariableHigh;
    //public float zVariableLow;
    //public float zVariableHigh;


    private void Start()
    {
        CM = FindObjectOfType<CoroutineManager>();
        initialPosition = transform.position; // 맵에 초기에 설정한 위치 정보와 회전정보를 가져옴
        initialRotation = transform.rotation;
        _position = new Vector3(xDir, yDir, zDir);
    }
    // Update is called once per frame
    void Update()
    {
        TruckMove();
    }

    public void TruckMove()
    {
        transform.localPosition += _position * truckSpeed * Time.deltaTime;
        if (transform.position.x > -20f)
        {
            transform.position = new Vector3(-120f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -120f) {
            Debug.Log(transform.position.x);
            transform.position = new Vector3(-20f, transform.position.y, transform.position.z);
        }

    }


    [PunRPC]
    public void ResetToInitialState()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerCheck") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            CharacterController characterController = other.gameObject.GetComponent<CharacterController>();
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            PlayerStatus playerStatus = other.gameObject.GetComponent<PlayerStatus>();
            if (characterController != null)
            {
                if (CM.GetCoroutine() != null)
                {
                    CM.NowStopCoroutine(); // 기존 코루틴을 종료
                }

                Vector3 ForceDirection = (other.transform.position - transform.position).normalized; // 이동 방향
                ForceDirection = new Vector3(ForceDirection.x, 0, ForceDirection.z);
                IEnumerator bounceCoroutine = Bounce(characterController, playerController, playerStatus, ForceDirection, power);
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

        while ((!playerStatus.IsGrounded() && CM.GetcoroutineInstance() != null) || first)
        {
            characterController.Move(direction * force * Time.deltaTime);
            first = false;
            yield return null;
        }
        playerController.anim.SetBool("IsKnockDown", false);
        CM.SetCoroutineInstance(null);
    }
}
