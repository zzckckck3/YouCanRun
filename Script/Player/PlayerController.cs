using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
// using UnityEngine.UIElements;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Photon.Voice.PUN;
using Unity.VisualScripting;
using UnityEngine.UI;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using System;

public class PlayerController : MonoBehaviourPunCallbacks
{
    PlayerGetItem playerGetItem; // 아이템 획득 스크립트
    PlayerUseItem playerUseItem; // 아이템 사용 스크립트

    PlayerStatus PS; // PlayerStatus 스크립트
    public CharacterClass characterClass;

    // 리스폰 스크립트
    Respawn res;
    // 초기 클래스 초기화 변수
    bool firstClass = false;

    private bool RespawnControllTrigger = true; // False인 동안 move 전부 멈춤 => true가 되면 move시작,
    private float RespawnTimer; // Time Delay를 줄 변수
    private bool RespawnTimeDelayTrigger; // False인 동안 Trigger를 바꾸지 않음 True면 Timer가 움직임


    // 조작관련 변수 변수는 전부 
    CharacterController cc;
    private float g;//중력 // 체공시간 1초정도
    private float jumpSpeed;
    private float nowJumpSpeed; // 현재 점프된 스피드 => 공중에서 떨어지는 속도를 계산하기 위해
    private float speed;
    private bool isJumped; // 점프한 상태인지
    private bool isSlided; // 미끄러짐을 가능하게 할지 
    private bool isControll; // 아이템 효과가 적용되는동안 유저가 컨트롤을 할 수 있게 할지 말지

    private int hp = 10; // 초기에 10이 아니면 RespawnControllTrigger가 발동 X 처음 리셋전에 체력이 안다는 이유도 같아보임.

    // 카메라 회전
    public Transform cameraArm;
    // 카메라 회전속도
    private float mouseSensitivity;
    private float rotateTime; // 숫자가 낮아질 수록 속도가 매우 느리게 회전함

    private float h;
    private float v;

    Vector3 nomalVector; // 플레이어가 서 있는 위치의 경사를 판단
    Vector3 moveDir; // 이동할 방향을 저장

    // 애니메이션
    public Animator anim;

    [SerializeField]
    private bool die;

    public PhotonView PV;

    // 개인 별 음성 RPC
    GameObject Voice;

    // X축 최대 시야 제한
    private float maxCamX;

    // 채팅창
    [SerializeField]
    private Transform chatInput;

    // 아이템 정보
    public float itemCooldown; // 아이템 쿨타임 시간
    bool itemReady;  // 아이템 습득 이후에 사용을 나누기 위함

    /// FanTrigger
    private bool isFan = false;
    public int bullet;  // 아이템 탄창
    public Image itemCooldownImg;   //남은 시간을 표시할 이미지
    public Image itemImageImg; // 현재 아이템 이미지

    // 상시 이동
    private Vector3 minMove = new Vector3(0, 0.0001f, 0);

    // 스킬 정보
    public float skillCooldown; // 직업 스킬 쿨타임 시간
    public float nowSkillCooldown; // 현재 스킬 쿨타임 시간
    public Image skillCooldownImg;	// 스킬 남은 시간을 표시할 이미지

    // Voice 이동
    Vector3 standardPosition;

    // 대쉬 정보
    public Image dashCooldownImg; // 대쉬 남은 시간을 표시할 이미지
    public float dashCooldown; // 대쉬 쿨타임 시간
    public float nowDashCooldown; // 현재 대쉬 쿨타임 시간

    // 대쉬 상태값
    public float dashInitial; // 초기 대쉬 속도
    public float dashAccel; // 대쉬 가속도
    public float dashMax; // 대쉬 최대속도
    public float isDashing; // 대쉬하고 있는지

    // 기본공격 정보
    public Image attackCooldownImg; // 기본공격 쿨타임 이미지
    public float attackCooldown = 3f; // 기본공격 쿨타임 시간
    public float nowAttackCooldown; // 현재 기본공격 쿨타임 시간
    Vector3 moveNormalVector;

    // Voicer Flow 변수
    bool moveVoice;
    float voiceMoveTime;
    CharacterClassSkillRPC skillRPC;

    // 죽었는지 체크
    private bool dieCheck;

    // 효과음
    public AudioSource effectSfx;
    private AudioClip slience005sec;
    private int runStep;
    private AudioClip runSound1;
    private AudioClip runSound2;

    // Bomber Timer 변수
    private float BombCool;
    private bool BombTimer;
    public Image ClassImg;
    private AudioClip runSound0;
    public TextMeshProUGUI BomberTimerCanvasText;

    public AudioSource effectSource; // AudioSource 컴포넌트의 참조

    // Pilot 변수
    private bool otherControll;
    private bool otherControllToMe;
    //private float otherControllTime;
    //private float otherControllToMeTime;
    private PhotonView otherPlayerPhotonView;
    private GameObject otherPlayer;
    private PlayerController otherPlayerController;

    // 초기화 Trigger
    private bool FirstResetTrigger;


    public void SetOtherPlayerPhotonView(PhotonView OPPV)
    {
        otherPlayerPhotonView = OPPV;
    }
    public void SetOtherPlayer(GameObject OP) {
        otherPlayer = OP;
    }

    public void SetOtherPlayerController(PlayerController OPC) {
        otherPlayerController = OPC;
    }


    public void SetOtherControll(bool TF) {
        otherControll = TF;
    }

    public void SetOtherControllToMe(bool TF) {
        otherControllToMe = TF;
    }

    public void PilotCameraArmMove() {
        if (otherPlayer != null) {
            cameraArm.transform.position = otherPlayer.transform.position;
        }
        Debug.Log("카메라 암 이동");
    }

    public void PilotCameraArmReset() {
        cameraArm.position = transform.position;
        Debug.Log("카메라 암 위치 원래대로");
    }

    void Start()
    {
        PV = PhotonView.Get(this);
        cc = transform.GetComponent<CharacterController>();
        characterClass = transform.GetComponent<CharacterClass>();
        PS = transform.GetComponent<PlayerStatus>();
        anim = GetComponent<Animator>();
        moveDir = Vector3.zero;
        nomalVector = Vector3.zero;
        playerGetItem = transform.GetComponent<PlayerGetItem>();
        playerUseItem = transform.GetComponent<PlayerUseItem>();
        res = transform.GetComponent<Respawn>();
        chatInput = GameObject.Find("Scroll View").transform.Find("ChatInput");
        itemCooldownImg = GameObject.Find("ItemCooldown").GetComponent<Image>();
        itemImageImg = GameObject.Find("ItemImage").GetComponent<Image>();
        skillCooldownImg = GameObject.Find("SkillCooldown").GetComponent<Image>();
        dashCooldownImg = GameObject.Find("DashCooldown").GetComponent<Image>();
        attackCooldownImg = GameObject.Find("AttackCooldown").GetComponent<Image>();
        attackCooldown = 3f;
        dashCooldown = 3f;
        isDashing = 0f;
        ClassImg = GameObject.Find("ClassImg").GetComponent<Image>();
        skillRPC = GetComponent<CharacterClassSkillRPC>();
        StatusInitailize();
        moveVoice = false;
        dieCheck = false;
        skillRPC = GetComponent<CharacterClassSkillRPC>();
        RespawnControllTrigger=true; // 처음에 조작 가능하도록 설정 
        Debug.Log(RespawnControllTrigger + "제발 트루여라 ㅅ;ㅣㅏㅂ");
        SetRespawnTimeDelayTrigger(false); // 처음에 리스폰 타이머는 안감
        itemReady = false;
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 0.75f);
        // 옮길지..
        otherControll=false;
        otherControllToMe = false;
    // 보이스 찾아오기

    // 사운드 설정
    slience005sec = Resources.Load("EffectsSounds/absolute-silence-005-sec") as AudioClip;

        runStep = 0;
        runSound0 = Resources.Load("EffectsSounds/SFX_Movement_Footstep_Generic_1") as AudioClip;
        runSound1 = Resources.Load("EffectsSounds/SFX_Movement_Footstep_Generic_2") as AudioClip;
        runSound2 = Resources.Load("EffectsSounds/SFX_Movement_Footstep_Generic_3") as AudioClip;
        ResetDefaultStatus();
        GetAllPlayerStatus();
        FirstResetTrigger = true;
    }

    public void GetAllPlayerStatus()
    {
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");
        PhotonView mPV;
        // 모든 플레이어를 호출해서 전부 엄청 느려지도록 설정한다.
        foreach (GameObject player in players)
        {
            mPV = player.GetComponent<PhotonView>();
            mPV.RPC("SendMyStatusRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    public void SendMyStatusRPC() {
        //if (characterClass == null) { 
            
        //}
        characterClass.JoinClassSetting();
    }

    /// </summary>
    public void FanTrigger(bool TF) {
        this.isFan = TF;
    }

    private void Awake()
    { // Voice 오브젝트에 자신의 PV를 할당, RPC를 위해,
        Voice = GameObject.Find("Voice");
        if (PV.IsMine)
        {
            VoiceRecorder vRecorder = Voice.GetComponent<VoiceRecorder>();
            vRecorder.PV = PV;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (!firstClass && Time.time>0.5f)
        //{
        //    dieCheck = true;
        //    res.Res();
        //    firstClass = true;
        //}
        if (FirstResetTrigger) {
            FirstResetTrigger = false;
            res.Res();
        }

        if (!dieCheck)
        {
            MyStatus(); // Status는 항상 업데이트 필요.
            if (RespawnControllTrigger)
            { // 업데이트에 딜레이를 줌
                StatusUpdate();
                SkillTimeFlow();
                if (characterClass.IsBomber())
                { // 항상 타이머는 체크해야함.
                    BombTimeFlow();
                }

                if (otherControll)
                {
                    if (otherPlayerController != null && !otherPlayerController.dieCheck)
                    {
                        // 내 캐릭터는 자동으로 떨어지고 있어야 함.
                        PlayerCantControll();
                        // 남의 캐릭터를 Photon으로 어떻게 움직일지 입력.
                        PilotCameraArmMove(); // CameraArm을 상대 캐릭터에 고정
                        PilotLookAround();
                        PilotMove();
                        if (otherPlayerPhotonView != null)
                        {
                            otherPlayerPhotonView.RPC("PilotUseSkill", RpcTarget.All, Input.GetKeyDown(KeyCode.Q));
                            otherPlayerPhotonView.RPC("PilotUseAttack", RpcTarget.All, Input.GetMouseButtonDown(0));

                        }
                    }
                }
                else if (otherControllToMe)
                {
                    // 아무것도 못하는 상태
                }
                else
                {
                    // 다른사람을 컨트롤하거나 다른사람이 나를 컨트롤 하지 않을 경우만 PV를 조종 할 것
                    if (PV.IsMine)
                    {
                        // 변경을 시켜줘야 하므로 IsMine체크에서 해제함
                        // 다른 캐릭터의 속도에 따라 이펙트를 넣으면 로컬에서도
                        LookAround();
                        // 충돌감지를 위한 Move 이거 안넣으면 가만히 있을 때, 충돌감지를 못함.
                        //cc.Move(minMove);
                        //minMove *= -1f;
                        
                        // 죽었거나 채팅중 움직임 아이템사용 X
                        if (moveVoice) // 목소리에 의해 이동중이면 조작 불가.
                        {
                            MovedVoice();
                        }
                        else
                        {
                            Move();
                            MoveDash();

                            if (isControll)
                            {
                                Item();
                                UseSkill();
                                UseDash();
                                UseAttack();
                            }
                        }
                    }
                }
            }
            RespawnCheck();
        }

        if (RespawnTimeDelayTrigger)
        { // 평소에는 타이머가 가지 않음
            RespawnTimeDelay();
        }

    }


    private void LookAround()
    {
        // 마우스의 X축과 Y축의 값
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))*mouseSensitivity*3;
        // 카메라 앵글
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        maxCamX = camAngle.x - mouseDelta.y;

        if (maxCamX < 180f)
        {
            maxCamX = Mathf.Clamp(maxCamX, -1f, 50f);
        }
        else
        {
            maxCamX = Mathf.Clamp(maxCamX, 335f, 361f);
        }

        cameraArm.rotation = Quaternion.Euler(maxCamX, camAngle.y + mouseDelta.x, camAngle.z);
    }


    private void Move()
    {
        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     Debug.Log("직업 변경!");
        //     ResetStatus();
        // }
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     FanTrigger(false);
        //     PS.ResetState();
        // }
        if (isControll)
        {
            PlayerControll();
        }
        else
        {
            PlayerCantControll();
        }
    }


    public void PlayerControll()
    {
        // PlayerRotate() => 움직이면 1, 안움직이면 0을 반환함
        float moveValue = 0f;
        if (!chatInput.gameObject.activeSelf)
        {
            moveValue = PlayerRotate();
        }


        moveDir = new Vector3(0, 0, moveValue);

        // 이전에 점프하지 않았다면, 공중이지 않을 시에만 점프 가능
        float jumpValue = PlayerJump();

        moveDir = new Vector3(0, jumpValue, moveDir.z * speed);
        moveDir = transform.TransformDirection(moveDir);
        cc.Move(moveDir * Time.deltaTime);
        // 구르기 함수
        Vector3 nowPosition = transform.position;
    }
    public void PlayerCantControll()
    {
        float jumpValue = PlayerFall();
        cc.Move(transform.TransformDirection(new Vector3(0, jumpValue, 0))*Time.deltaTime);
    }
    
    // 플레이어 회전
    public float PlayerRotate()
    {
        float moveForward = 0f;

        int inputH = 0;
        int inputV = 0;
        if (!chatInput.gameObject.activeSelf) // 채팅 입력시 좌우로 못 움직이게 설정
        { 
            inputH = (int)Input.GetAxisRaw("Horizontal");
            inputV = (int)Input.GetAxisRaw("Vertical");
        }
        if (inputH != 0 || inputV != 0)
        {
            anim.SetBool("isRun", true);
            if (PS.IsGrounded() && !effectSfx.isPlaying)
            {
                RunSound();
            }
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
            transform.rotation = Quaternion.Slerp(fromV, toV, rotateTime * Time.deltaTime);
            // 카메라 Arm의 회전을 절대좌표로 막아주는 부분
            cameraArm.rotation = nowV;
        }
        else
        {
            anim.SetBool("isRun", false);
        }
        return moveForward;
    }
    
    private void RunSound()
    {
        switch (runStep)
        {
            case 0:
                effectSfx.PlayOneShot(runSound0);
                runStep = 1;
                break;
            case 1:
                effectSfx.PlayOneShot(slience005sec);
                runStep = 2;
                break;
            case 2:
                effectSfx.PlayOneShot(runSound1);
                runStep = 3;
                break;
            case 3:
                effectSfx.PlayOneShot(slience005sec);
                runStep = 4;
                break;
            case 4:
                effectSfx.PlayOneShot(runSound2);
                runStep = 5;
                break;
            case 5:
                effectSfx.PlayOneShot(slience005sec);
                runStep = 0;
                break;
            default:
                break;
        }

    }


    // 플레이어 점프
    public float PlayerJump()
    {
        if (PS.IsGrounded())
        {
            // 땅이라면 초기화
            PS.NowGround();
            if (!isJumped)
            {
                if (Input.GetButtonDown("Jump") && !chatInput.gameObject.activeSelf) // 채팅창 입력이 안 켜져 있다면
                {
                    JumpSound();
                    nowJumpSpeed = jumpSpeed;
                    isJumped = true;
                }
            }
            anim.SetBool("isJump", false);
        }
        else
        {
            if (nowJumpSpeed > -50f)
            {
                nowJumpSpeed -= g * Time.deltaTime; // 계속해서 중력만큼 속도 빼주기
            }
            anim.SetBool("isJump", true);
        }
        return nowJumpSpeed;
    }

    private void JumpSound()
    {
        AudioClip jumpSound = Resources.Load("EffectsSounds/PP_Jump_1_2") as AudioClip;
        effectSfx.PlayOneShot(jumpSound);
    }

    public float PlayerFall()
    {
        if (PS.IsGrounded())
        {

        }
        else
        {
            if (nowJumpSpeed > -50f)
            {
                nowJumpSpeed -= g * Time.deltaTime; // 계속해서 중력만큼 속도 빼주기
            }
        }
        return nowJumpSpeed;
    }

    void Item() {
        // 아이템 지속시간 경과 호출
        PS.ItemEffect(); // 아이템 효과 시간 적용
        GetItem();
        UseItem();
    }

    void GetItem()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetTrigger("isObtain");
            PS.CanControll();
            Debug.Log("아이템습득");
            playerGetItem.GetItem();
            itemImageImg.sprite = playerGetItem.itemImage;
            StartCoroutine(WaitForNextFrame());
        };
    }

    IEnumerator WaitForNextFrame()
    {
        yield return null;
        itemReady = true;
    }

    void UseItem()
    {
        if (itemCooldown > 0)
        {
            itemCooldown -= Time.deltaTime;
            itemCooldownImg.fillAmount = itemCooldown / playerGetItem.itemCooldown;
        }
        if (itemCooldown <= 0 && bullet != 0)
        {
            if (Input.GetKeyDown(KeyCode.E) && playerGetItem.hasItem && itemReady)
            {
                Vector3 cameraRotate = new Vector3(0, cameraArm.eulerAngles.y, 0);

                Quaternion fromV = transform.rotation;
                Quaternion toV = Quaternion.Euler(cameraRotate);
                Quaternion nowV = cameraArm.rotation;
                transform.rotation = Quaternion.Slerp(fromV, toV, 100f * Time.deltaTime);
                // 카메라 Arm의 회전을 절대좌표로 막아주는 부분
                cameraArm.rotation = nowV;
                playerUseItem.UseItem();
                bullet--;
                if (bullet == 0)
                {
                    Destroy(playerGetItem.myItem.gameObject);
                    playerGetItem.myItem = null;
                    playerGetItem.itemImage = null;
                    itemImageImg.sprite = null;
                    playerGetItem.hasItem = false;
                }
                itemCooldown = playerGetItem.itemCooldown;
            }
        }

        if (bullet == 0 || !playerGetItem.hasItem)
        {
            itemReady = false;
        }
    }

    // 슬라이드하도록
    private Vector3 DirectionToSlope(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, nomalVector);
    }


    void UseSkill()
    {
        if (characterClass.IsDefault() || characterClass.IsJumper())
        {
            return;
        }
        if (nowSkillCooldown <= 0)
        {
            if ( characterClass.IsVoicer()) // 직업이 Voicer면 Q로 스킬 사용 X를 위해
            { 
                characterClass.ClassSkill();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                if (characterClass.IsBomber()) // 직업이 Bomber면 3초 이후 스킬 사용
                {
                    SettingBombTimer(3f);
                    nowSkillCooldown = skillCooldown;
                }
                else { 
                    characterClass.ClassSkill();
                    nowSkillCooldown = skillCooldown;
                }
            }

        }

    }
    private void MoveDash()
    {
        if (nowDashCooldown > 0)
        {
            nowDashCooldown -= Time.deltaTime;
            dashCooldownImg.fillAmount = nowDashCooldown / dashCooldown;

            if (isDashing < dashMax)
            {
                Vector3 dashDir = new Vector3(0, 0, isDashing);
                dashDir = transform.TransformDirection(dashDir);
                cc.Move(dashDir * Time.deltaTime);
                isDashing += dashAccel * Time.deltaTime;
            }
        }
    }
    private void UseDash()
    {
        if (PS.IsGrounded() && nowDashCooldown <= 0 && PS.GetIsControll())
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                DashSound();
                nowDashCooldown = dashCooldown;
                anim.SetTrigger("isDash"); // Dash 애니메이션 발동
                PS.SetIsControll(false);
                isDashing = 0;
            }
        }
    }


    void SkillTimeFlow() {
        if (nowSkillCooldown > 0)
        {
            nowSkillCooldown -= Time.deltaTime;
            if (PV.IsMine) { 
                skillCooldownImg.fillAmount = nowSkillCooldown / skillCooldown;
            }
        }
    }

    private void DashSound()
    {
        AudioClip dashSound = Resources.Load("EffectsSounds/SFX_Movement_Swoosh_Slow_2") as AudioClip;
        
        effectSfx.PlayOneShot(dashSound);
    }

    private void UseAttack()
    {
        if (nowAttackCooldown > 0)
        {
            nowAttackCooldown -= Time.deltaTime;
            attackCooldownImg.fillAmount = nowAttackCooldown / attackCooldown;
        }
        if (nowAttackCooldown <= 0 && PS.GetIsControll())
        {
            if (Input.GetMouseButtonDown(0))
            {
                KickSound();
                nowAttackCooldown = attackCooldown;
                anim.SetTrigger("isKick");
                skillRPC.DefaultAttack();
            }
        }
    }

    public void SetNowJumpSpeed(float NJS) { 
        nowJumpSpeed = NJS;
    }

    public void KickSound()
    {
        AudioClip kickSound = Resources.Load("EffectsSounds/SFX_Movement_Swoosh_Med_2") as AudioClip;
        effectSfx.PlayOneShot(kickSound);
    }

    public float GetGravity()
    {
        return this.g;
    }

    private void MyStatus()
    {
        g = PS.GetGravity();
        jumpSpeed = PS.GetJumpSpeed();
        speed = PS.GetSpeed();
        rotateTime = PS.GetRotateTime();
        isJumped = PS.GetIsJumped();
        isSlided = PS.GetIsSlided();
        isControll = PS.GetIsControll();
        skillCooldown = PS.GetSkillCoolDown();
        dashInitial = 3f;
        dashAccel = PS.GetSpeed() * 5;
        dashMax = PS.GetSpeed() * 3.5f;
        hp = PS.GetHp();
        // 다른 인원이 조종하는지 확인
        otherControll = PS.GetOtherControll();
        otherControllToMe = PS.GetOtherControllToMe();
    }

    public void RespawnCheck() {
        if (hp <= 0)
        {
            //res.Res();
            SetRespawnTimer(); // DelayTrigger 0.5초 세팅
            SetRespawnControllTrigger(false);
            SetRespawnTimeDelayTrigger(true); //
            dieCheck = true;
            anim.SetTrigger("isDie");
            StartCoroutine(RespawnCoroutine());
        }
    }

    IEnumerator RespawnCoroutine()
    {
        while (RespawnTimer > 0)
        {
            yield return null; // 한프레임 대기
        }

        res.Res() ; // 리스폰 처리 함수
    }

    public void ResetSkillCoolDown()
    {
        nowSkillCooldown = 1f;
    }

    public void StatusInitailize() {
        PS.NowGround();
        PS.SetGroundCheck();
        PS.SetIsControll(true);
        PS.SetTimeFlow(false);
    }

    private void StatusUpdate()
    {
        PS.OtherControllCheck();
        PS.OtherControllToMeCheck();
        PS.SpeedCheck();
        PS.JumpSpeedCheck();
        PS.GravityCheck();
        PS.StunCheck();
        PS.IceCheck();
    }

    public void ResetStatus()
    {
        characterClass.GetRandomClass(); // 랜덤직업 얻고
        characterClass.ClassSetting();
        // 랜덤 직업 state를 설정
        dieCheck = false;
    }

    public void ResetDefaultStatus(){
        // 초기화 오류로 인해 처음에 DefaultClass x
        characterClass.GetDefaultClass(); // 랜덤직업 얻고
        //characterClass.GetRandomClass();
        characterClass.ClassSetting();
        RespawnControllTrigger = true;
        // 랜덤 직업 state를 설정
    }

    public void GetNowSkillCool() {
        nowSkillCooldown = skillCooldown;
    }

    public void VoiceCool() {
        Debug.Log("스킬 쿨 확인;");
        nowSkillCooldown = characterClass.GetDefaultskillCoolDown();
    }
    public void SetMovedVoice(Vector3 normalVector, float moveTime)
    {
        moveVoice = true;
        voiceMoveTime = moveTime;
        moveNormalVector = normalVector;
    }

    public void MovedVoice() {
        voiceMoveTime -= Time.deltaTime;
        if (voiceMoveTime < 0) {
            moveVoice = false;
        }
        cc.Move(moveNormalVector*Time.deltaTime);
    }

    public void SettingBombTimer(float BombTime) { 
        BombCool = BombTime;
        BombTimer = true;
    }
    public void SetBombTimer(bool TF) {
        BombTimer = TF;
    }


    /// <summary>
    ///  Respawn 될 때 Animation 종료 후 리스폰 할 때 update에 move가 일어나서 Respawn이 제대로 안되는 것으로 보임
    ///  그래서 Die Animation이 끝난 이후에 Move 함수들이 실행 될 수 있도록 딜레이를 줘서 Respawn을 시켜줌
    /// </summary>
    public void SetRespawnControllTrigger(bool TF)
    {
        RespawnControllTrigger = TF;
        Debug.Log(RespawnControllTrigger + "이거 뭔데 체력이랑 같이 확인해봐");
        Debug.Log("체력 : " + hp);
    }

    void BombTimeFlow()
    {
        if (BombTimer) { 
            if (BombCool > 0)
            {
                BombCool -= Time.deltaTime;
                if (PV.IsMine) { 
                    PV.RPC("SetBombTimerSeconds", RpcTarget.All, (float) BombCool);
                }
                //if (BombCool < 3f) {
                //    ClassImg.fillAmount = BombCool / 3;
                //}
            }
            else {
                BombTimer = false;
                if (PV.IsMine) {
                    PV.RPC("SetBombTimerSeconds", RpcTarget.All, 0f);
                }
                characterClass.ClassSkill();
            }
        }
    }

    [PunRPC]
    public void SetBombTimerSeconds(float sec)
    {
        // 오브젝트 위의 닉네임을 GameManager 에서 가져오기
        BomberTimerCanvasText.text = sec.ToString("#.##");
    }


    public void SetRespawnTimer()
    {
        RespawnTimer = 2.3f; // Delay Time 0.5초
    }
    private void RespawnTimeDelay()
    {
        //Debug.Log("리스폰 타이머 : " + RespawnTimer);
        if (RespawnTimer > 0)
        {
            RespawnControllTrigger = false;
            RespawnTimer -= Time.deltaTime;
        }
        else
        {
            RespawnControllTrigger = true;
            SetRespawnTimeDelayTrigger(false);
            
        }
    }
    public void SetRespawnTimeDelayTrigger(bool TF)
    {
        RespawnTimeDelayTrigger = TF;
    }

    private void OnEnable()
    {
        SetSetting.OnMouseSensitivityChanged += HandleMouseSensitivityChanged;
    }

    private void OnDisable()
    {
        SetSetting.OnMouseSensitivityChanged -= HandleMouseSensitivityChanged;
    }

    private void HandleMouseSensitivityChanged(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
        // 마우스 민감도 값이 변경되었을 때 원하는 동작 수행
    }

    ////////
    ///


    private void PilotLookAround()
    {
        // 마우스의 X축과 Y축의 값
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity * 3;
        // 카메라 앵글
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        maxCamX = camAngle.x - mouseDelta.y;

        if (maxCamX < 180f)
        {
            maxCamX = Mathf.Clamp(maxCamX, -1f, 50f);
        }
        else
        {
            maxCamX = Mathf.Clamp(maxCamX, 335f, 361f);
        }
        cameraArm.rotation = Quaternion.Euler(maxCamX, camAngle.y + mouseDelta.x, camAngle.z); // 내 카메라 화면 절대 회전 각.
        if (otherPlayerPhotonView != null)
        {
            otherPlayerPhotonView.RPC("OtherPlayerSetCameraView", RpcTarget.All, maxCamX, camAngle, mouseDelta); // 타 유저의 카메라 화면을 나와 동기화
        }
    }

    [PunRPC]
    public void OtherPlayerSetCameraView(float maxCamXRPC, Vector3 camAngleRPC, Vector2 mouseDeltaRPC)
    {
        cameraArm.rotation = Quaternion.Euler(maxCamXRPC, camAngleRPC.y + mouseDeltaRPC.x, camAngleRPC.z);
    }
    // 회전 관련 PilotRPC Done


    [PunRPC]
    public void OtherPlayerAnimRun(string strAnim, bool TF)
    {
        if (PV.IsMine)
        {
            anim.SetBool(strAnim, false);
        }
    }

    /// /////////////////////////////////////////////////////// 
    /// Pilot Class 조작 함수
    // 플레이어 이동z
    public void PilotMove()
    {
        float timeRPC = Time.deltaTime; // 내가 입력하는 값과 상대 컴퓨터에서 계산하는 값이 다르기 때문에 값이 튀지 않으려면 time을 같이 보내주어야 함
        if (isControll) // 상태는 똑같을 것이므로 상태 변수는 대부분 로컬값을 사용.
        {
            int inputHRPC = 0;
            int inputVRPC = 0;
            bool jumpRPC = false;
            if (!chatInput.gameObject.activeSelf) // 채팅 입력시 좌우, 점프 사용 불가능
            {
                inputHRPC = (int)Input.GetAxisRaw("Horizontal");
                inputVRPC = (int)Input.GetAxisRaw("Vertical");
                jumpRPC = Input.GetButtonDown("Jump");
            }
            // 포톤으로 값 주기
            if (otherPlayerPhotonView != null)
            {
                otherPlayerPhotonView.RPC("PilotPlayerControll", RpcTarget.All, inputHRPC, inputVRPC, jumpRPC, timeRPC);
            }
        }
        else
        {
            // 포톤으로 상대 함수 실행
            if (otherPlayerPhotonView != null)
            {
                otherPlayerPhotonView.RPC("PilotPlayerCantControll", RpcTarget.All, timeRPC);
            }
        }

    }

    [PunRPC]
    public void PilotPlayerControll(int inputHRPC, int inputVRPC, bool jumpRPC, float timeRPC) // 입력값을 받아서 RPC 호출
    {
        if (PV.IsMine) // 호출을 받았다면,
        {
            float moveValue = 0f;
            moveValue = PilotPlayerRotate(inputHRPC, inputVRPC, timeRPC);
            // PUN으로 input 보내주기
            moveDir = new Vector3(0, 0, moveValue);

            // 이전에 점프하지 않았다면, 공중이지 않을 시에만 점프 가능
            float jumpValue = PilotPlayerJump(jumpRPC, timeRPC);

            moveDir = new Vector3(0, jumpValue, moveDir.z * speed);
            moveDir = transform.TransformDirection(moveDir);
            cc.Move(moveDir * timeRPC);
        }
    }


    //[PunRPC] // 카메라 회전 값이 RPC로 전해졌음. 
    public float PilotPlayerRotate(int inputHRPC, int inputVRPC, float timeRPC)
    {
        float moveForward = 0f;
        int inputH = inputHRPC;
        int inputV = inputVRPC;
        if (inputH != 0 || inputV != 0)
        {
            anim.SetBool("isRun", true);
            if (PS.IsGrounded() && !effectSfx.isPlaying)
            {
                RunSound();
            }
            moveForward = 1f;
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
            transform.rotation = Quaternion.Slerp(fromV, toV, rotateTime * timeRPC);
            // 카메라 Arm의 회전을 절대좌표로 막아주는 부분
            cameraArm.rotation = nowV;
        }
        else
        {
            anim.SetBool("isRun", false);
        }
        return moveForward;
    }

    public float PilotPlayerJump(bool jumpRPC, float timeRPC)
    {
        if (PS.IsGrounded())
        {
            // 땅이라면 초기화
            PS.NowGround();
            if (!isJumped)
            {
                if (jumpRPC && !chatInput.gameObject.activeSelf) // 채팅창 입력이 안 켜져 있다면
                {
                    JumpSound();
                    nowJumpSpeed = jumpSpeed;
                    isJumped = true;
                }
            }
            anim.SetBool("isJump", false);
        }
        else
        {
            if (nowJumpSpeed > -50f)
            {
                nowJumpSpeed -= g * timeRPC; // 계속해서 중력만큼 속도 빼주기
            }
            anim.SetBool("isJump", true);
        }
        return nowJumpSpeed;
    }

    [PunRPC]
    public void PilotPlayerCantControll(float timeRPC)
    {
        if (PV.IsMine)
        { // 포톤 뷰 검증
            float jumpValue = PlayerFall();
            cc.Move(transform.TransformDirection(new Vector3(0, jumpValue, 0)) * timeRPC);
        }
    }
    
    [PunRPC]
    void PilotUseSkill(bool InputSkill)
    {
        if (PV.IsMine){
            if (nowSkillCooldown <= 0)
            {
                if (characterClass.IsVoicer()) // 직업이 Voicer면 Q로 스킬 사용 X를 위해
                {
                    characterClass.ClassSkill();
                }
                else if (InputSkill)
                {
                    if (characterClass.IsBomber()) // 직업이 Bomber면 3초 이후 스킬 사용
                    {
                        SettingBombTimer(3f);
                        nowSkillCooldown = skillCooldown;
                    }
                    else
                    {
                        characterClass.ClassSkill();
                        nowSkillCooldown = skillCooldown;
                    }
                }
            }
        }
    }

    [PunRPC]
    private void PilotUseAttack(bool InputAttack)
    {
        if (PV.IsMine){
            if (nowAttackCooldown > 0)
            {
                nowAttackCooldown -= Time.deltaTime;
                attackCooldownImg.fillAmount = nowAttackCooldown / attackCooldown;
            }
            if (nowAttackCooldown <= 0 && PS.GetIsControll())
            {
                if (InputAttack)
                {
                    KickSound();
                    nowAttackCooldown = attackCooldown;
                    anim.SetTrigger("isKick");
                    skillRPC.DefaultAttack();
                }
            }
        }
    }

    public void SetDieCheck(bool TF) {
        dieCheck = TF;
    }

    public bool GetDieCheck() { 
        return dieCheck;
    }
}
