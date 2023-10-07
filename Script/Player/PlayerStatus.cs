using Photon.Pun;
using TMPro;
using UnityEditor;
using UnityEngine;
using DamageNumbersPro;
using UnityEngine.UI;
using System.Security.Cryptography;

public class PlayerStatus : MonoBehaviourPunCallbacks
{
    public PlayerController PC;
    CharacterClass characterClass;
    public PhotonView PV;
    public CharacterClassSkillRPC characterClassSkillRPC;
    public DamageNumber damageNum;
    public DamageNumber healNum;
    public Respawn res;

    private float g = 40f;//중력 // 체공시간 1초정도
    private float jumpSpeed = 20f;
    //private float nowJumpSpeed;// 현재 점프된 스피드 => 공중에서 떨어지는 속도를 계산하기 위해
    private float speed = 8.0f;
    private bool isJumped; // 점프한 상태인지
    private bool isSlided; // 미끄러짐을 가능하게 할지 

    // 아이템 변수
    private float itemEffectTime = 0f; // 아이템 효과 시간
    private bool canItemApply; // 아이템의 적용을 허용할지 판단하는 변수 => True면 아이템 효과 덮어쓰기 가능 False면 덮어쓰기 불가능, 기본값은 True
    private bool isControll; // 아이템 효과가 적용되는동안 유저가 컨트롤을 할 수 있게 할지 말지
    private bool timeFlow; // 아이템이 적용되는 동안 시간이 흘러감을 알려줄 변수, False일 경우 아이템 적용X이므로 시간 연산 X
    private float rotateTime = 15f; // 숫자가 낮아질 수록 속도가 매우 느리게 회전함
    Transform groundCheck;
    //private int groundLayer = 1 << LayerMask.NameToLayer("Plane"); // LayerMask가 안먹혀서 직접가져옴
    private int groundLayer = 1 << 3 | 1 << 6;

    // 땅 체크
    Vector3 boxSize;

    // 체력
    public int hp = 10;
    public int maxHp = 10;
    public TextMeshProUGUI hpUI; // 체력 UI
    public Image hpBar;
    // 스킬 쿨
    float skillCoolDown=10f;


    // 캐릭터 상태 관리
    //speed
    bool isChangeSpeed;
    float changeSpeedTimeFlow;
    //jumpSpeed
    bool isChangeJumpSpeed;
    float changeJumpSpeedTimeFlow;
    //g
    bool isChangeGravity;
    float changeGravityTimeFlow;
    //isControll
    bool isStun;
    float stunTimeFlow;
    // 스킬
    private float IceTimeFlow = 0f;
    public GameObject IceEffect;

    private GameObject stunEffectInstance; // 스턴 시간이 끝나면 스턴이펙트를 종료시켜야 하므로 변수 선언
    // Start is called before the first frame update

    //// Pilot 변수
    private bool otherControll;
    private bool otherControllToMe;
    private float otherControllTime;
    private float otherControllToMeTime;
    public GameObject pilotControllCanvas;

    public void SetOtherControll(bool TF)
    {
        otherControll = TF;
    }
    public bool GetOtherControll() { 
        return otherControll;
    }
    public void SetOtherControllTime(float CT) {
        otherControllTime = CT;
    }

    public void SetOtherControllToMe(bool TF)
    {
        otherControllToMe = TF;
        pilotControllCanvas.SetActive(TF);
    }
    public bool GetOtherControllToMe() { 
        return otherControllToMe;
    }
    public void SetOtherControllToMeTime(float CT)
    {
        otherControllToMeTime = CT;
    }

    public void OtherControllCheck() {
        if (otherControll)
        {
            otherControllTime -= Time.deltaTime;
            if (otherControllTime <= 0f)
            {
                ResetOtherControll();
            }
        }
    }

    public void OtherControllToMeCheck() {
        if (otherControllToMe)
        {
            otherControllToMeTime -= Time.deltaTime;
            if (otherControllToMeTime <= 0f)
            {
                SetOtherControllToMe(false);
                ResetOtherControllToMe();
            }
        }
    }

    public void ResetOtherControll() {
        SetOtherControll(false);
        PC.SetOtherPlayerPhotonView(null);
        PC.SetOtherPlayer(null);
        PC.SetOtherPlayerController(null);
        PC.PilotCameraArmReset(); //카메라 위치 원위치
        otherControllTime = 0;
    }

    public void ResetOtherControllToMe() {
        SetOtherControllToMe(false);
        otherControllToMeTime = 0;
    }


    void Start()
    {
        PC = transform.GetComponent<PlayerController>();
        characterClass = transform.GetComponent<CharacterClass>();
        PV = transform.GetComponent<PhotonView>();
        characterClassSkillRPC = transform.GetComponent<CharacterClassSkillRPC>();
        res = transform.GetComponent<Respawn>();
        boxSize = new Vector3(1.5f, 0.15f, 1.5f);
        NowGround();
        SetGroundCheck();
        SetIsControll(true);
        SetTimeFlow(false);
        otherControll=false;
        otherControllToMe=false;
        otherControllTime = 0;
        otherControllToMeTime =0;
        hpUI = GameObject.Find("Text_HP").GetComponent<TextMeshProUGUI>();
        SetHPUI();
    }

    // 아이템 지속시간 체크

    // 맨 처음 기본 Status로 초기화
    public void ResetState() //자신의 직업에 따른 State를 초기화 한다.
    {
        canItemApply = true;
        timeFlow = false;
        itemEffectTime = 0;
        speed = characterClass.GetDefaultSpeed();
        jumpSpeed = characterClass.GetDefaultJumpSpeed();
        g = characterClass.GetDefaultGravity();
        rotateTime = characterClass.GetDefaultRotateTime();
        PC.SetDieCheck(false);
        //PC.SetRespawnTimer(); // DelayTrigger 0.5초 세팅
        PC.SetRespawnTimeDelayTrigger(true); // DelayTrigger 시작,
    }

    public void ResetDebuff()
    {
        ResetSpeed();
        ResetJumpSpeed();
        ResetGravity();
        CanControll();
        ResetIce();
        ResetOtherControll();
        ResetOtherControllToMe();
    }

    public void ResetCharacterClassStatus() { // 캐릭터가 Response 될 때 초기화를 위해 모든 디버프 해제를 추가
        // 디버프 해제
        ResetDebuff();

        // 죽고나서 애니메이션 초기화
        skillCoolDown = characterClass.GetDefaultskillCoolDown();
        Debug.Log("dkdh PC : " +PC);
        Debug.Log("아오 : anim : "+PC.anim);
        if (PC.anim != null) { 
            PC.anim.SetBool("isStun", false);
            PC.anim.SetBool("IsKnockDown", false);
        }
        PC.ResetSkillCoolDown();
        SetHP(characterClass.GetDefaultHp());
        ResetState();
    }

    public void JoinCharacterClassStatus()
    {
        PV.RPC("JoinCharacterClassStatusRPC", RpcTarget.All,this.canItemApply, this.timeFlow, this.itemEffectTime, this.speed,
            this.jumpSpeed, this.g, this.rotateTime, this.hp, this.maxHp, this.otherControll, this.otherControllToMe, PC.GetDieCheck());
    }

    [PunRPC]
    public void JoinCharacterClassStatusRPC(bool canItemApplyRPC, bool timeFlowRPC, float itemEffectTimeRPC, float speedRPC, float jumpSpeedRPC, float gRPC
        ,float rotateTimeRPC, int hpRPC, int maxHpRPC, bool otherControllRPC, bool otherControllToMeRPC, bool dieCheckRPC )
    {
        canItemApply = canItemApplyRPC;
        timeFlow = timeFlowRPC;
        itemEffectTime = itemEffectTimeRPC;
        speed = speedRPC;
        jumpSpeed = jumpSpeedRPC;
        g = gRPC;
        rotateTime = rotateTimeRPC;
        hp = hpRPC;
        maxHp = maxHpRPC;
        otherControll = otherControllRPC;
        otherControllToMe = otherControllToMeRPC;
        PC.SetDieCheck(dieCheckRPC);
        SetHPUI();
    }


    public bool IsGrounded()
    {
        // 박스의 충돌을 감지, 충돌했으면 땅위라고 가정

        // CheckBox사용시 주의해야할 점 : CheckBox가 Player도 check돼서 자꾸 땅으로 인식함;
        // 해결 => groundLayer로 구분해줌

        return Physics.CheckSphere(groundCheck.position, 0.5f, groundLayer);
    }

    private void OnDrawGizmos() //goundCheck 범위확인용
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(groundCheck.position, boxSize);
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheck.position, 0.5f);
    }

    //private void OnDrawGizmos()
    //{

    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawWireCube(groundCheck.position, boxSize);
    //}

    public void NowGround() // 현재 Status를 땅으로 변경
    {
        isJumped = false;
        isSlided = false;
        if (PC != null)
        {
            PC.SetNowJumpSpeed(0);
        }
    }

    // 자기에게만 적용되는 아이템 => 추후 로직에 따라 수정할 계획
    // 현재 바로 할당인데 곱연산으로 수정할 수도 있음
    public void ItemEffect()
    {
        if (timeFlow)
        {
            itemEffectTime -= Time.deltaTime;
            if (itemEffectTime <= 0f)
            {
                ResetState();
            }
        }
    }
    public void ApplyItemEffect(float itemTime, float characterSpeed, float jump = 20f, float gravity = 40f, float rotateSpeed = 3.5f, bool itemApply = true, bool canControll = true)
    { // isItemApply가 True라면 다른 아이템의 적용이 가능하게 한다.
        if (canItemApply)
        {
            canItemApply = itemApply;
            timeFlow = true;
            itemEffectTime = itemTime;
            speed = characterSpeed;
            jumpSpeed = jump;
            g = gravity;
            rotateTime = rotateSpeed;
            isControll = canControll;
        }
    }

    public float GetGravity()
    {
        return g;
    }
    public float GetJumpSpeed()
    {
        return jumpSpeed;
    }
    public float GetSpeed()
    {
        return speed;
    }
    public float GetRotateTime()
    {
        return rotateTime;
    }
    public bool GetIsJumped()
    {
        return isJumped;
    }
    public bool GetIsSlided()
    {
        return isSlided;
    }
    public bool GetIsControll()
    {
        return isControll;
    }

    public float GetSkillCoolDown() {
        return skillCoolDown;
    }



    // 초기 Status Setting
    public void SetGroundCheck()
    {
        groundCheck = transform.Find("GroundCheck").gameObject.transform;
    }
    public void SetIsControll(bool controllTF)
    {
        isControll = controllTF;
    }
    public void SetTimeFlow(bool timeTF)
    {
        timeFlow = timeTF;
    }

    public int GetHp() {
        return hp;
    }

    public void SetHP(int hp)
    {
        // 캐릭터 생성 시 hp 설정
        Debug.Log("아오 셋 hp");
        Debug.Log(hp);
        Debug.Log(characterClass.GetDefaultHp());
        this.hp = hp;
        maxHp = hp;
        SetHPUI();
    }
    
    public void Damage(int damage)
    {
        // 캐릭터의 데미지
        hp -= damage;
        // 데미지 텍스트 효과
        DamageNumber damageNumber = damageNum.Spawn(transform.position, damage);
        
        // 캐릭터 체력 확인 후 리스폰 할지 결정
        SetHPUI();
        
    }
    
    public void Heal(int heal)
    {
        // 캐릭터의 힐
        if (hp < maxHp)
        {
            hp += heal;
            if (hp > maxHp) { 
                hp = maxHp;
            }
        }
        // 회복 텍스트 효과
        DamageNumber damageNumber = healNum.Spawn(transform.position, heal);
        // 캐릭터 체력 확인 후 리스폰 할지 결정
        SetHPUI();
    }

    public void SetHPUI()
    {
        // HP UI에 hp 동기화
        if (PV.IsMine)
        {
            if (hpUI == null)
            {
                hpUI = GameObject.Find("Text_HP").GetComponent<TextMeshProUGUI>();
            }
            hpUI.text = hp.ToString();
        }
        // 체력바 표시
        if (maxHp == 0) { // 처음에만 동기화가 안되므로
            maxHp = 10;
        }
        this.hpBar.fillAmount = (float)hp / (float)maxHp;
        //Debug.Log("hp : " + hp +" | maxHP : "+ maxHp);


    }
    public void SetCanItemApply(bool itemTF){
        canItemApply = itemTF;
    }

    /// Speed TimeFlow
    public void SpeedCheck()
    {
        if (isChangeSpeed)
        {
            changeSpeedTimeFlow -= Time.deltaTime;
            if (changeSpeedTimeFlow <= 0f)
            {
                ResetSpeed();
            }
        }
    }
    public void ResetSpeed()
    {
        changeSpeedTimeFlow=0f;
        isChangeSpeed = false;
        // Start 호출 순서에 따른 선 할당 필요
        if (characterClass == null) {
            characterClass = gameObject.GetComponent<CharacterClass>();
        }
        speed = characterClass.GetDefaultSpeed(); // => 점프스피드 가져오기
    }
    public void ChangeSpeed(float CS, float timeFlow)
    {
        isChangeSpeed = true;
        speed = CS;
        changeSpeedTimeFlow = timeFlow;
    }

    /// JumpSpeed TimeFlow
    public void JumpSpeedCheck()
    {
        if (isChangeJumpSpeed)
        {
            changeJumpSpeedTimeFlow -= Time.deltaTime;
            if (changeJumpSpeedTimeFlow <= 0f)
            {
                ResetJumpSpeed();
            }
        }
    }
    public void ResetJumpSpeed()
    {
        changeJumpSpeedTimeFlow=0f;
        isChangeJumpSpeed = false;
        jumpSpeed = characterClass.GetDefaultJumpSpeed(); // => 점프스피드 가져오기
    }
    public void ChangeJumpSpeed(float CJS, float timeFlow)
    {
        isChangeJumpSpeed = true;
        jumpSpeed = CJS;
        changeJumpSpeedTimeFlow = timeFlow;
    }

    /// Gravity TimeFlow
    public void GravityCheck()
    {
        if (isChangeGravity)
        {
            changeGravityTimeFlow -= Time.deltaTime;
            if (changeGravityTimeFlow <= 0f)
            {
                ResetGravity();
            }
        }
    }
    public void ResetGravity()
    {
        changeGravityTimeFlow=0f;
        isChangeGravity = false;
        g = characterClass.GetDefaultGravity(); // => 점프스피드 가져오기
    }
    public void ChangeGravity(float CG, float timeFlow)
    {
        isChangeGravity = true;
        g = CG;
        changeGravityTimeFlow = timeFlow;
    }

    /// Stun TimeFlow
    public void StunCheck()
    {
        if (isStun)
        {
            PC.anim.SetBool("isStun", true);
            stunTimeFlow -= Time.deltaTime;
            if (stunTimeFlow <= 0f)
            {
                PC.anim.SetBool("isStun", false);
                CanControll();
            }
        }
    }
    public void CantControll(float stunTime)
    {
        PV.RPC("StunEffectRPC", RpcTarget.All);
        isStun = true;
        stunTimeFlow = stunTime;
        isControll = false;
    }
    public void CanControll()
    {
        stunTimeFlow = 0f;
        isStun = false;
        isControll = true;
        PC.anim.SetBool("isStun", false);
        PV.RPC("DestroyStunEffectRPC", RpcTarget.All);
    }

    [PunRPC]
    public void StunEffectRPC()
    {
        // 스턴 이펙트가 이미 존재하는지 확인
        if (stunEffectInstance != null)
        {
            return; // 이미 스턴 이펙트가 있으면 함수를 종료
        }

        GameObject effect = Resources.Load("Effects/stun") as GameObject;
        stunEffectInstance = Instantiate(effect, transform);
    }

    [PunRPC]
    public void DestroyStunEffectRPC()
    {
        if (stunEffectInstance != null)
        {
            Destroy(stunEffectInstance);
        }
    }


    // IceManTimeFlow
    public void IceCheck()
    {
        if (IceTimeFlow > 0f)
        {
            IceTimeFlow -= Time.deltaTime;
            characterClassSkillRPC.IceEffectFlow(IceTimeFlow/20f+0.25f); // 이부분에서 오류
            if (IceTimeFlow <= 0f)
            {
                ResetIce();
            }
        }
    }
    public void IceState(float iceTime)
    {
        IceTimeFlow = iceTime;
        speed = 0.5f;
        jumpSpeed = 5f;
        g = 10f;
        rotateTime = 0.75f;
    }
    public void ResetIce()
    {
        IceTimeFlow = 0f;
        characterClassSkillRPC.IceEffectFlow(0f);
        speed = characterClass.GetDefaultSpeed();
        jumpSpeed = characterClass.GetDefaultJumpSpeed();
        rotateTime = characterClass.GetDefaultRotateTime();
        g = characterClass.GetDefaultGravity();
    }
    public float GetIceTime()
    {
        return IceTimeFlow;
    }

    public void OnOffIceEffect(bool TF) {
        IceEffect.SetActive(TF);
    }

    // 안쓰는 함수
    public void ResponseCharacter()
    {
        Transform[] points = GameObject.Find("CteatePlayerGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(0, points.Length);
        transform.position = points[idx].position;
        transform.rotation = points[idx].rotation;
        Debug.Log("리스폰!");
        characterClass.GetRandomClass(); // 랜덤직업 얻고
        characterClass.ClassSetting();
         // 직업 state를 설정
         // 직업에 따른 내 Status 초기화
    }

    [PunRPC]
    public void SetHPBar()
    {
        // 체력바 출력
        this.hpBar.fillAmount = (float)hp / (float)maxHp;
    }
}
