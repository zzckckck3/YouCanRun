using System;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;
using UnityEngine.UI;
public class CharacterClass : MonoBehaviourPunCallbacks
{
    ClassType myClass=ClassType.DEFAULT;
    // System Random을 사용할 경우 Instance를 생성해야하고,
    // Unity Random은 그냥사용해도 되며, Vector와 색상들의 랜덤을 만들기 쉬움
    CharacterClassSkillRPC skillRPC;
    PhotonView PV;
    PlayerController PC;
    // 클래스 이미지
    public Image classImg;
    PlayerStatus PS;
    // 스킬 이미지
    public Image skillImg;
    // 총 오브젝트
    public GameObject SHOTGUN;
    public GameObject SAWGUN;
    public GameObject PISTOL;

    // Enum 멤버 목록 배열
    bool isControll;
    bool canItemApply;
    bool timeFlow;
    float itemEffectTime;
    float speed=10f;
    float jumpSpeed =20f;
    float g = 40f;
    float rotateTime=15f;
    float skillCoolDown;
    int hp =10;

    // Bomber Timer Canvas
    public GameObject BomberTimerCanvas;
    
    

    public enum ClassType
    { // 처음 생성시 DEFAULT로 되도록 설정
        DEFAULT,
        JUMPER,
        BOMBER,
        SLOTH,
        THUNDER,
        ICEMAN,
        VOICER,
        FOUNTAINMAN,
        PHOENIX,
        SHOTGUN,
        ELECTRICSAW,
        PISTOL,
        PILOT
    };

    // 난수 생성
    public ClassType[] ClassTypes = (ClassType[])Enum.GetValues(typeof(ClassType));
    //ClassType randomEnumValue = enumValues [Random.Range(0, enumValues.Length)];

    void Start()
    {
        PV = GetComponent<PhotonView>();
        PC = GetComponent<PlayerController>();
        PS = GetComponent<PlayerStatus>();
        skillRPC=GetComponent<CharacterClassSkillRPC>();
        skillImg = GameObject.Find("SkillClassImage").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void GetRandomClass()
    {
        myClass = (ClassType)Random.Range(0, Enum.GetValues(typeof(ClassType)).Length);
        // 동기화를 위해 PUN으로 SHOTGUN을 Active하는 부분을 옮김

    }

    // Default로 초기화
    public void GetDefaultClass()
    {
        myClass = ClassType.DEFAULT;
    }
    public void SelectClass(int selectNum)
    {
        myClass = ClassTypes[selectNum];
        ClassSetting();
    }

    // 직업별 Status를 설정 
    private void SetMyStatus(bool isControllValue, bool canItemApplyValue, bool timeFlowValue, float itemEffectTimeValue, float speedValue, float jumpSpeedValue, float gValue, float rotateTimeValue, int hpValue, float skillCoolDownValue)
    {
        isControll = isControllValue;
        canItemApply = canItemApplyValue;
        timeFlow = timeFlowValue;
        itemEffectTime = itemEffectTimeValue;
        speed = speedValue;
        jumpSpeed = jumpSpeedValue;
        g = gValue;
        rotateTime = rotateTimeValue;
        hp = hpValue;
        skillCoolDown = skillCoolDownValue;
        SetClassImage();
    }
    public void SetRandomClassStatus()
    {
        switch (myClass) {
            case ClassType.DEFAULT:  // if (조건)
                SetMyStatus(true, true, false, 0f, 10f, 20f, 40f, 15f,10, 10f); //여기에 기본 status를 정의
                break;
            case ClassType.JUMPER:  // if (조건)
                SetMyStatus(true, true, false, 0f, 10f, 40f, 40f, 15f, 10, 5f);
                break;
            case ClassType.BOMBER:  // if (조건)
                SetMyStatus(true, true, false, 0f, 16f, 40f, 40f, 30f, 2, 3f);
                PC.SettingBombTimer(40f);
                break;
            case ClassType.SLOTH:  // if (조건)
                SetMyStatus(true, true, false, 0f, 6f, 15f, 30f, 1.5f, 5, 3f);
                break;
            case ClassType.THUNDER:  // if (조건)
                SetMyStatus(true, true, false, 0f, 10f, 20f, 40f, 15f, 20, 7f);
                break;
            case ClassType.ICEMAN:
                SetMyStatus(true, true, false, 0f, 10f, 20f, 40f, 15f, 20, 20f);
                break;
            case ClassType.VOICER:
                SetMyStatus(true, false, true, 0f, 10f, 20f, 40f, 15f, 15, 10f);
                break;
            case ClassType.FOUNTAINMAN:
                SetMyStatus(true, true, false, 0f, 10f, 20f, 40f, 15f, 8, 6f);
                break;
            case ClassType.PHOENIX:
                SetMyStatus(true, true, false, 0f, 10f, 20f, 40f, 15f, 8, 10f);
                break;
            case ClassType.SHOTGUN:
                SetMyStatus(true, true, false, 0f, 10f, 20f, 40f, 15f, 10, 3f);
                break;
            case ClassType.ELECTRICSAW:
                SetMyStatus(true, true, false, 0f, 12f, 20f, 40f, 15f, 12, 0.5f);
                break;
            case ClassType.PISTOL:
                SetMyStatus(true, true, false, 0f, 12f, 20f, 40f, 15f, 10, 3f);
                break;
            case ClassType.PILOT:
                SetMyStatus(true, true, false, 0f, 11f, 20f, 40f, 15f, 10, 15f);
                break;
            default:     // else
                SetMyStatus(true, true, false, 0f, 10f, 20f, 40f, 15f, 10, 10f);
                break;
        }
        Debug.Log("MyClass : " + myClass);
    }
    public void ClassSkill()
    {
        switch (myClass)
        {
            case ClassType.DEFAULT:  // if (조건)
                Debug.Log("DEFAULT SKILL");
                break;

            case ClassType.JUMPER:  // if (조건)
                Debug.Log("JUMPER SKILL");
                break;

            case ClassType.BOMBER:  // if (조건)
                Debug.Log("BOMBER SKILL"); // 일정 시간 뒤에 터지거나 Q를 눌렀을 때 터지도록 스킬 변경
                skillRPC.SkillBomberStart();
                break;

            case ClassType.SLOTH:  // if (조건)
                Debug.Log("SLOTH SKILL"); // 다른 캐릭터에게 엎혀서 가도록 스킬 변경
                skillRPC.SkillSlothRPC();
                break;
            case ClassType.THUNDER:  // if (조건)
                PC.anim.SetTrigger("useSkill");
                break;
            case ClassType.ICEMAN:
                Debug.Log("ICEMAN SKILL");
                skillRPC.IcemanEffect(PV);
                skillRPC.UseIceman(transform.position);
                break;
            case ClassType.VOICER: // 스킬 이펙트 추가
                skillRPC.UseVoice(transform.position);
                break;
            case ClassType.FOUNTAINMAN:
                Debug.Log("FOUNTAINMAN SKILL");
                skillRPC.UseFountainMan();
                break;
            case ClassType.PHOENIX:
                Debug.Log("Phoenix SKILL");
                PC.anim.SetTrigger("useSkill2");
                break;
            case ClassType.SHOTGUN:
                Debug.Log("GUNNER SKILL");
                PC.anim.SetTrigger("useShotgun");
                break;
            case ClassType.ELECTRICSAW:
                Debug.Log("ELECTRICSAW SKILL");
                PC.anim.SetTrigger("useElectricSaw");
                PS.CanControll();
                skillRPC.UseElectricSaw(PV);
                skillRPC.SkillElectricSaw();
                break;
            case ClassType.PISTOL:
                PC.anim.SetTrigger("usePistol");
                break;
            case ClassType.PILOT:
                Debug.Log("PILOT SKILL");
                skillRPC.UsePILOT();
                break;
            default:     // else
                break;
        }
    }

    public void Thunder()
    {
        if (PV.IsMine) { 
            skillRPC.UseThunder(PV);
            skillRPC.SkillThunder();
            PS.CanControll();
        }
    }

    public void Phoenix()
    {
        skillRPC.UsePhoenix(PV);
        skillRPC.SkillPhoenix();
        PS.CanControll();
    }

    public void Shotgun()
    {
        skillRPC.UseShotGun(PV);
        skillRPC.SkillShotgun();
        PS.SetIsControll(true);
    }

    public void Pistol()
    {
        skillRPC.UsePistol(PV);
        skillRPC.SkillPistol();
        PS.SetIsControll(true);
    }

    public float GetDefaultSpeed()
    {
        return speed;
    }
    public float GetDefaultGravity()
    {
        return g;
    }
    public float GetDefaultJumpSpeed() {
        return jumpSpeed;
    }
    public float GetDefaultRotateTime() { 
        return rotateTime;
    }
    public int GetDefaultHp() {
        return hp;
    }
    public float GetDefaultskillCoolDown() {
        return skillCoolDown;
    }

    public bool IsVoicer() {
        if (myClass == ClassType.VOICER) { 
            return true;
        }
        return false;
    }

    public bool IsDefault()
    {
        if (myClass == ClassType.DEFAULT)
        {
            return true;
        }
        return false;
    }

    public bool IsJumper()
    {
        if (myClass == ClassType.JUMPER)
        {
            return true;
        }
        return false;
    }

    public bool IsBomber()
    {
        if (myClass == ClassType.BOMBER)
        {
            return true;
        }
        return false;
    }

    private void SetClassImage()
    {
        classImg.sprite = Resources.Load("ClassImage/"+myClass.ToString(), typeof(Sprite)) as Sprite;
    }
    
    private void SetSkillImage()
    {
        if (skillImg==null) {
            skillImg = GameObject.Find("SkillClassImage").GetComponent<Image>();
        }
        skillImg.sprite = Resources.Load("ClassImage/"+myClass.ToString(), typeof(Sprite)) as Sprite;
    }

    public void ClassSetting()
    {
        if (PV == null)
        {
            PV = GetComponent<PhotonView>();
        }
        // CharacterClass의 Start와 PlayerStatus의 Start가 호출되는 순서의 차이가 있어 null 오류가 있음
        if (PV.IsMine)
        { 
            PV.RPC("ClassRPC", RpcTarget.All, this.myClass);
            SetSkillImage();
        }
    }

    [PunRPC]
    public void ClassRPC(ClassType setMyClass) {
        myClass = setMyClass;
        // 무기 이미지 Setting 하는 부분
        if (myClass == ClassType.SHOTGUN)
        {
            SHOTGUN.SetActive(true);
            SAWGUN.SetActive(false);
            PISTOL.SetActive(false);
        }
        else if (myClass == ClassType.ELECTRICSAW)
        {
            SHOTGUN.SetActive(false);
            SAWGUN.SetActive(true);
            PISTOL.SetActive(false);
        }
        else if (myClass == ClassType.PISTOL)
        {
            SHOTGUN.SetActive(false);
            SAWGUN.SetActive(false);
            PISTOL.SetActive(true);
        }
        else
        {
            SAWGUN.SetActive(false);
            SHOTGUN.SetActive(false);
            PISTOL.SetActive(false);
        }

        if (myClass == ClassType.BOMBER)
        {
            BomberTimerCanvas.SetActive(true);
        }
        else
        {
            BomberTimerCanvas.SetActive(false);
        }

        SetRandomClassStatus(); // 직업 state를 설정
        // 호출 순서 문제로 null 검증이 필요함..
        if (PS == null)
        {
            PS = GetComponent<PlayerStatus>();
        }
        PS.ResetCharacterClassStatus(); // 모든 상태를 초기화 하고, 애니메이션도 없애고, 직업 스탯을 주는 함수
        Debug.Log("Class Setting" + myClass);
    }

    // ClassSetting과 다른 이유 => 이미 들어와 있는 플레이어들의 정보를 나와 동기화 시키기 위해, 구분이 필요함. 다르기 때문 => 추후 최적화 : 한명의 로컬 정보로 부하 줄이기 
    public void JoinClassSetting()
    {
        if (PV == null)
        {
            PV = GetComponent<PhotonView>();
        }
        // CharacterClass의 Start와 PlayerStatus의 Start가 호출되는 순서의 차이가 있어 null 오류가 있음
        if (PV.IsMine)
        {
            PV.RPC("JoinClassRPC", RpcTarget.All, this.myClass);
            PS.JoinCharacterClassStatus();
            SetSkillImage();
        }
    }

    [PunRPC]
    public void JoinClassRPC(ClassType setMyClass)
    {
        myClass = setMyClass;
        // 무기 이미지 Setting 하는 부분
        if (myClass == ClassType.SHOTGUN)
        {
            SHOTGUN.SetActive(true);
            SAWGUN.SetActive(false);
            PISTOL.SetActive(false);
        }
        else if (myClass == ClassType.ELECTRICSAW)
        {
            SHOTGUN.SetActive(false);
            SAWGUN.SetActive(true);
            PISTOL.SetActive(false);
        }
        else if (myClass == ClassType.PISTOL)
        {
            SHOTGUN.SetActive(false);
            SAWGUN.SetActive(false);
            PISTOL.SetActive(true);
        }
        else
        {
            SAWGUN.SetActive(false);
            SHOTGUN.SetActive(false);
            PISTOL.SetActive(false);
        }

        if (myClass == ClassType.BOMBER)
        {
            BomberTimerCanvas.SetActive(true);
        }
        else
        {
            BomberTimerCanvas.SetActive(false);
        }

        classImg.sprite = Resources.Load("ClassImage/" + myClass.ToString(), typeof(Sprite)) as Sprite;
        //SetRandomClassStatus(); // 직업 state를 설정
        // 호출 순서 문제로 null 검증이 필요함..
        if (PS == null)
        {
            PS = GetComponent<PlayerStatus>();
        }
        PS.JoinCharacterClassStatus(); // 모든 상태를 가져와 동기화 하는 함수
        Debug.Log("Class Setting" + myClass);
    }




}
