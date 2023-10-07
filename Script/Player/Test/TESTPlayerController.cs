using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class TESTPlayerController : MonoBehaviour
{
    private float g = 40f;//중력 // 체공시간 1초정도
    private float jumpSpeed = 20f;
    private float nowJumpSpeed; // 현재 점프된 스피드 => 공중에서 떨어지는 속도를 계산하기 위해
    private float Speed = 8.0f;
    private float h;
    private float v;

    //[SerializeField]
    Transform groundCheck;
    private bool isJumped; // 점프한 상태인지
    private bool isSlided; // 미끄러짐을 가능하게 할지

    // Ground Check 변수
    //private int groundLayer = 1 << LayerMask.NameToLayer("Plane"); // LayerMask가 안먹혀서 직접가져옴
    private int groundLayer = 1 << 3 | 1 << 6;

    // Slope 관련 bool
    RaycastHit slopeHit; // 경사각 감지용
    private const float RAY_DISTANCE = 2f; //
    private const float slopeAngle = 50f; // 급경사 판단 angle

    Vector3 nomalVector; // 플레이어가 서 있는 위치의 경사를 판단
    Vector3 boxSize; // Ground를 체크할 box
    Vector3 moveDir; // 이동할 방향을 저장
    CharacterController cc;

    // 카메라 회전
    public Transform cameraArm;
    private float timeCount = 3.5f;

    // 아이템 변수
    private float itemEffectTime = 0f; // 아이템 효과 시간
    private bool isItemApply; // 아이템의 적용을 허용할지 판단하는 변수 => True면 아이템 효과 덮어쓰기 가능 False면 덮어쓰기 불가능, 기본값은 True
    private bool isControll; // 아이템 효과가 적용되는동안 유저가 컨트롤을 할 수 있게 할지 말지
    private bool timeFlow; // 아이템이 적용되는 동안 시간이 흘러감을 알려줄 변수, False일 경우 아이템 적용X이므로 시간 연산 X

    void Start()
    {
        cc = GetComponent<CharacterController>();
        NowGround(); // 처음 시작은 땅일 것.
        moveDir = Vector3.zero;
        nomalVector = Vector3.zero;
        groundCheck = transform.Find("GroundCheck").gameObject.transform;
        boxSize = new Vector3(1.5f, 0.15f, 1.5f);
        isItemApply = true;
        isControll = true;
        timeFlow = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeFlow)
        {
            itemEffectTime -= Time.deltaTime;
            //Debug.Log(itemEffectTime);
            if (itemEffectTime <= 0f)
            {
                ResetState();
            }
        }
        Move();

    }
    private void FixedUpdate()
    {
        //Move();
    }


    // 플레이어 이동
    private void Move()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ApplyItemEffect(10, 3, 40, 1, 5);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            ResetState();
        }
        if (isControll)
        {
            PlayerControll();
        }
    }

    // 플레이어 점프가능하도록 초기화
    void NowGround()
    {
        isJumped = false;
        nowJumpSpeed = 0f;
        isSlided = false;
    }

    // 슬라이드하도록
    private Vector3 DirectionToSlope(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, nomalVector);
    }

    public bool IsGrounded()
    {
        // 박스의 충돌을 감지, 충돌했으면 땅위라고 가정

        // CheckBox사용시 주의해야할 점 : CheckBox가 Player도 check돼서 자꾸 땅으로 인식함;
        // 해결 => groundLayer로 구분해줌

        //return Physics.CheckBox(groundCheck.position, boxSize * 0.7f, Quaternion.identity, groundLayer);
        return Physics.CheckSphere(groundCheck.position, 0.5f, groundLayer);
    }

    // 아이템 효과를 주는 방법 //아이템에서 호출해서 쓸 함수 Setter
    public void ApplyItemEffect(float itemTime = 0f, float characterSpeed = 8.0f, float jump = 20f, float gravity = 40f, float rotateSpeed = 3.5f, bool itemApply = true)
    {
        if (isItemApply)
        {
            isItemApply = itemApply;
            timeFlow = true;
            itemEffectTime = itemTime;
            Speed = characterSpeed;
            jumpSpeed = jump;
            g = gravity;
            timeCount = rotateSpeed;
        }
    }


    public void ResetState()
    { // 아이템 효과를 없애주는 함수
        isItemApply = true;
        timeFlow = false;
        itemEffectTime = 0;
        Speed = 8.0f;
        jumpSpeed = 20f;
        g = 40f;
        timeCount = 3.5f;
    }

    public void PlayerControll()
    {
        float moveForward = 0f;
        int inputH = (int)Input.GetAxisRaw("Horizontal");
        int inputV = (int)Input.GetAxisRaw("Vertical");
        if (inputH != 0 || inputV != 0)
        {
            moveForward = 1f;
            //Debug.Log("입력들어옴");
            Vector3 cameraRotate = new Vector3(0, cameraArm.eulerAngles.y, 0);
            if (inputH != 0)
            { //clockwise =plus
                if (inputH < 0)
                {
                    cameraRotate.y += -90f;
                    if (inputV < 0)
                    {
                        cameraRotate.y += -45f;
                    }
                    else if (inputV > 0)
                    {
                        cameraRotate.y += 45f;
                    }
                }
                else if (inputH > 0)
                {
                    cameraRotate.y += 90f;
                    if (inputV < 0)
                    {
                        cameraRotate.y += 45f;
                    }
                    else if (inputV > 0)
                    {
                        cameraRotate.y -= 45f;
                    }
                }

            }
            else
            {
                if (inputV < 0)
                {
                    cameraRotate.y += 180f;
                }
            }
            // 구형 선형 보간으로 회전 Lerp => ...
            Quaternion fromV = transform.rotation;
            Quaternion toV = Quaternion.Euler(cameraRotate);
            Quaternion nowV = cameraArm.rotation;
            transform.rotation = Quaternion.Slerp(fromV, toV, 15f * Time.deltaTime);
            // 카메라 Arm의 회전을 절대좌표로 막아주는 부분
            cameraArm.rotation = nowV;
        }

        // 움직임 구현

        moveDir = new Vector3(0, 0, moveForward);

        // 이전에 점프하지 않았다면, 공중이지 않을 시에만 점프 가능

        if (IsGrounded())
        {
            // 땅이라면 초기화
            NowGround();
            if (!isJumped)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    nowJumpSpeed = jumpSpeed;
                    isJumped = true;
                }
            }
        }
        else
        {
            nowJumpSpeed -= g * Time.deltaTime; // 계속해서 중력만큼 속도 빼주기
        }


        // normalized한 값으로 항상 일정한x, z 스피드를 갖게 한다. => 대각선으로 이동해도 속도가 일정하다는 의미
        moveDir = new Vector3(0, nowJumpSpeed, moveDir.z * Speed);
        moveDir = transform.TransformDirection(moveDir);
        cc.Move(moveDir * Time.deltaTime);
        // 카메라 이동
        Vector3 asd = transform.position;
        cameraArm.position = asd + new Vector3(0, 0.5f, 0);
    }


    //private void OnDrawGizmos() //goundCheck 범위확인용
    //{
    //    //Gizmos.color = Color.red;
    //    //Gizmos.DrawWireCube(groundCheck.position, boxSize);
    //    // Draw a yellow sphere at the transform's position
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(groundCheck.position, 0.5f);
    //}


    // Slope인지 return 해줌
    //private bool IsOnSlope()
    //{
    //    Ray slopeRay = new Ray(transform.position, Vector3.down);

    //    if (Physics.Raycast(slopeRay, out slopeHit, RAY_DISTANCE)) // 레이캐스트 실행
    //    {
    //        // 충돌 발생한 경우
    //        GameObject hitObject = slopeHit.collider.gameObject; // 충돌한 오브젝트 가져오기
    //        int hitLayer = hitObject.layer; // 충돌한 오브젝트의 레이어 ID 가져오기
    //        string hitLayerName = LayerMask.LayerToName(hitLayer); // 레이어 ID를 레이어 이름으로 변환
    //        if (hitLayerName != "Player")
    //        {
    //            var angle = Vector3.Angle(Vector3.up, slopeHit.normal);
    //            //if angle()
    //            //return angle != 0f && angle < slopeAngle;
    //        }
    //        else if (hitLayerName == "Player")
    //        {
    //            return 0;
    //        }
    //        Debug.Log("충돌한 레이어 이름: " + hitLayerName);
    //    }
    //    // 공중이면 미끌림X
    //    return false;
    //}
}