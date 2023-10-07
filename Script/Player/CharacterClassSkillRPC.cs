using Photon.Pun;
using System;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Firebase.Auth;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;


public class CharacterClassSkillRPC : MonoBehaviour
{
    // 사용자 배열
    GameObject[] players;
    PlayerController PC;
    // 플레이어 상태 스크립트
    PlayerStatus PS;
    // 파티클 오브젝트
    private GameObject effect;
    public GameObject myCamera;

    FrostEffect frostScript=null;
    // 주변 검사용
    public LayerMask layerMask;

    //Voicer용
    private Recorder recorder; // Recorder 컴포넌트를 여기에 연결하세요.
    public GameObject SpeakerPre;

    float MaxDistance = 300f;  // 스킬 사용 최대 거리

    // 코루틴을 사용하기 위한 코루틴 매니저 선언
    private CoroutineManager CM;
    private GameObject VoiceObject;
    private GameObject cc;

    // Saw범위
    public GameObject SawCheck;
    private Vector3 SawBox;

    public AudioSource skillAudio;


    void Start()
    {
        if (myCamera != null)
        {
            frostScript = myCamera.GetComponent<FrostEffect>();
        }
        PC = GetComponent<PlayerController>();
        PS = GetComponent<PlayerStatus>();
        PC = GetComponent<PlayerController>();
        VoiceObject = GameObject.Find("Voice");
        recorder = VoiceObject.GetComponent<Recorder>();
        cc = gameObject;
        CM = FindObjectOfType<CoroutineManager>();
        SawBox = new Vector3(0.75f, 1f, 1.1f);
        
    }

    /// <summary>
    /// SLOTH
    /// </summary>
    public void SkillSlothRPC() { 
        PhotonView myPV = GetComponent<PhotonView>();
        myPV.RPC("SkillSloth", RpcTarget.All);

    }

    [PunRPC]
    public void SkillSloth() {
        PlaySound("SLOTH", 30f);
        PS.Heal(3);
    }

    /// <summary>
    /// Default Attack
    /// </summary>

    public void DefaultAttack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.5f, layerMask);
        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (!hitCollider.transform.parent.GetComponent<PhotonView>().IsMine)
                {
                    Vector3 toTarget = (hitCollider.transform.parent.position - transform.position).normalized;
                    float angleToTarget = Vector3.Angle(transform.forward, toTarget);

                    if (angleToTarget <= 60f) // 120도는 양쪽으로 60도씩입니다.
                    {
                        // 여기서 kick에 대한 상호작용을 수행하세요.
                        hitCollider.transform.parent.GetComponent<PhotonView>().RPC("DefaultAttackRPC", RpcTarget.All);
                        //hitCollider.transform.parent.GetComponent<PhotonView>().RPC("StunEffectRPC", RpcTarget.All);
                        hitCollider.transform.parent.GetComponent<PlayerController>().anim.SetTrigger("isHit");

                    }
                }
            }
        }
    }

    [PunRPC]
    public void DefaultAttackRPC()
    {
        // 기본공격 피격 시
        PS.CantControll(1.5f); // 행동 불능 시간
        PS.Damage(1);    // 데미지
    }

    /// <summary>
    /// Pistol
    /// </summary>
    public void UsePistol(PhotonView tPV)
    {
        tPV.RPC("UsePistolRPC", RpcTarget.All);
    }

    [PunRPC]
    public void UsePistolRPC()
    {
        effect = Resources.Load("Effects/Pistol") as GameObject;  // Resources 폴더 안에서 호출
        Instantiate(effect, transform); // 파티클 생성
        PlaySound("Pistol", 40f);
    }

    public void SkillPistol()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Vector3 halfExtents = new Vector3(0.25f, 0.25f, 0.25f);  // 가로, 세로, 높이의 절반

        RaycastHit hit;
        if (Physics.BoxCast(ray.origin, halfExtents, ray.direction, out hit, Quaternion.identity, MaxDistance))
        {
            Debug.Log(hit.transform.tag); // this를 찍으면 Object 이름이 나옴 // hit.transform == "PlayerClone" // hit.transform.tag == "PlayerCheck"
            if (hit.transform.tag == "PlayerCheck")  // 정중앙 에임에 플레이어가 맞으면
            {
                PhotonView targetPV = hit.transform.parent.GetComponent<PhotonView>();
                targetPV.RPC("SkillPistollRPC", RpcTarget.All,hit.point);
            }
        }
    }

    [PunRPC]
    public void SkillPistollRPC(Vector3 hitPoint)
    {
        // 피닉스 피격시
        PS.CantControll(0.5f); // 행동 불능 시간
        PS.Damage(2);
        effect = Resources.Load("Effects/HitPistol") as GameObject;  // Resources 폴더 안에서 호출
        GameObject effectInstance = Instantiate(effect);
        effectInstance.transform.position = hitPoint; // 월드 좌표로 위치 설정
    }

    /// <summary>
    /// Gunner
    /// </summary>
    public void UseShotGun(PhotonView tPV)
    {
        tPV.RPC("UseShotgunRPC", RpcTarget.All);
    }

    [PunRPC]
    public void UseShotgunRPC()
    {
        effect = Resources.Load("Effects/Shotgun") as GameObject;  // Resources 폴더 안에서 호출
        Instantiate(effect, transform); // 파티클 생성
        PlaySound("Shotgun", 40f);
    }

    public void SkillShotgun()
    {
        Vector3 boxSize = new Vector3(5f, 3f, 6f);
        Vector3 boxCenter = transform.position + transform.forward * (boxSize.z / 2f);  // 박스의 중심을 플레이어가 바라보는 방향으로 설정

        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize / 2f, transform.rotation, layerMask);
        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (!hitCollider.transform.parent.GetComponent<PhotonView>().IsMine)
                {
                    hitCollider.transform.parent.GetComponent<PhotonView>().RPC("SkillShotgunRPC", RpcTarget.All, transform.forward);
                }
            }
        }
    }

    [PunRPC]
    public void SkillShotgunRPC(Vector3 knockbackDir)
    {
        // 피닉스 피격시
        PS.CantControll(1.5f); // 행동 불능 시간
        PS.Damage(3);
        PC.SetMovedVoice(knockbackDir*10,0.5f);
    }

    /// <summary>
    /// Phoenix
    /// </summary>

    public void UsePhoenix(PhotonView tPV)
    {
        tPV.RPC("UsePhoenixRPC", RpcTarget.All);
    }

    [PunRPC]
    public void UsePhoenixRPC()
    {
        effect = Resources.Load("Effects/Phoenix") as GameObject;  // Resources 폴더 안에서 호출
        Instantiate(effect, transform); // 파티클 생성
        PlaySound("Phoenix",40f);
    }

    public void SkillPhoenix()
    {
        Vector3 boxSize = new Vector3(5f, 3f, 12f);
        Vector3 boxCenter = transform.position + transform.forward * (boxSize.z / 2f);  // 박스의 중심을 플레이어가 바라보는 방향으로 설정

        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize / 2f, transform.rotation, layerMask);
        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (!hitCollider.transform.parent.GetComponent<PhotonView>().IsMine)
                {
                    hitCollider.transform.parent.GetComponent<PhotonView>().RPC("SkillPhoenixRPC", RpcTarget.All);

                }
            }
        }
    }

    [PunRPC]
    public void SkillPhoenixRPC()
    {
        // 피닉스 피격시
        PS.Damage(5);
        if(PS.GetHp() > 0)
        {
            PC.anim.SetTrigger("isKnockDownTrigger");
            PS.SetIsControll(false);
        }
    }


    /// <summary>
    /// THUNDER
    /// </summary>

    public void UseThunder(PhotonView tPV)
    {
        // 번개 파티클 생성 RPC 호출
        if (tPV.IsMine)
        {
            tPV.RPC("UseThunderRPC", RpcTarget.All);
            Debug.Log("번개번개");
        }
    }

    [PunRPC]
    public void UseThunderRPC()
    {
        // 번개 파티클 본인 캐릭터 위치에 생성
        //PhotonView myPV=GetComponent<PhotonView>();
        //if (myPV.IsMine)
        //{
            effect = Resources.Load("Effects/Lightning Strike") as GameObject;  // Resources 폴더 안에서 호출
            Instantiate(effect, transform); // 파티클 생성
            PlaySound("Thunder", 40f);
        //}
    }

    public void SkillThunder()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, layerMask);
        PhotonView AroundPV;
        PhotonView myPV = GetComponent<PhotonView>();  // 현재 오브젝트의 PhotonView
        if (colliders.Length > 0) { 
            foreach (Collider col in colliders)
            {
                AroundPV = col.transform.parent.GetComponent<PhotonView>();
                if (!AroundPV.IsMine && AroundPV != myPV)
                {
                    //Debug.Log(AroundPV.Owner.NickName);
                    //Debug.Log(AroundPV.IsMine);
                    AroundPV.RPC("SkillThunderRPC", RpcTarget.All);
                }

            }
        }
    }

    [PunRPC]
    public void SkillThunderRPC()
    {
        // 번개 피격 시
        PS.CantControll(5f); // 행동 불능 시간
        PS.Damage(3);    // 데미지
    }



    /// <summary>
    /// ICEMAN
    /// </summary>
    public void IceEffectFlow(float tt)
    {
        if (myCamera != null)
        {
            if (frostScript == null) {
                frostScript = myCamera.GetComponent<FrostEffect>();
            }
            frostScript.SetFrostAmount(tt);
        }
    }

    public void IcemanEffect(PhotonView tPV)
    {
        tPV.RPC("IcemanEffetcRPC", RpcTarget.All);
    }

    [PunRPC]
    public void IcemanEffetcRPC()
    {
        effect = Resources.Load("Effects/ICEMAN") as GameObject;  // Resources 폴더 안에서 호출
        Instantiate(effect, transform); // 파티클 생성
        PlaySound("ICEMAN", 40f);
    }

    public void UseIceman(Vector3 myPosition)
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        PhotonView mPV;
        // 모든 플레이어를 호출해서 전부 엄청 느려지도록 설정한다.
        foreach (GameObject player in players)
        {
            mPV = player.GetComponent<PhotonView>();
            if (!mPV.IsMine)
            {
                float distance = 0;
                Vector3 playerPosition = player.transform.position;
                distance += (float)Math.Pow(myPosition.x - playerPosition.x, 2);
                distance += (float)Math.Pow(myPosition.y - playerPosition.y, 2);
                distance += (float)Math.Pow(myPosition.z - playerPosition.z, 2);
                distance = (float)Math.Sqrt(distance);
                if (distance <= 20)
                {
                    mPV.RPC("IcemanRPC", RpcTarget.All);
                }
            }
        }
    }

    [PunRPC]
    public void IcemanRPC() // ICEMAN의 스킬을 맞음
    {
        PS.OnOffIceEffect(true);
        PS.Damage(2);
        if (myCamera != null)
        {
            PS.IceState(10f);
            frostScript.SetFrostAmount(0.5f);
        }
    }

    public void UseVoice(Vector3 myPosition) {
        if (recorder != null)
        {
            float inputLevel = recorder.LevelMeter.CurrentAvgAmp;
            
            if (inputLevel > 0.25f)
            {
                //Debug.Log("음성 입력 크기" + inputLevel);
                PlayerController PC= GetComponent<PlayerController>();
                PC.VoiceCool();
                if (PhotonNetwork.PlayerListOthers.Length > 0)
                {
                    // 로그 확인용
                    players = GameObject.FindGameObjectsWithTag("Player");
                    PhotonView sPV;
                    float distance;
                    foreach (GameObject player in players)
                    {
                        sPV = player.GetComponent<PhotonView>();
                        if (!sPV.IsMine)
                        {
                            distance = (myPosition - player.transform.position).magnitude;
                            if (distance <= 30)
                            {
                                sPV.RPC("UseVoiceRPC", RpcTarget.All, myPosition);
                                PC.VoiceCool();
                            }
                        }
                        else // 이펙트 생성용
                        {
                            sPV.RPC("OnVoiceEffect", RpcTarget.All);
                        }
                    }
                }
                else
                {
                    Debug.Log("혼자입니다.");
                }
            }
        }
    }

    [PunRPC]
    public void OnVoiceEffect() {
        SpeakerPre.SetActive(true);
    }


    [PunRPC]
    public void UseVoiceRPC(Vector3 centerPosition)
    {
        PS.Damage(3);
        PS.CantControll(5f); //y축은 그대로 있도록 변경 & 1m 만큼 위에 뜨도록 변경
        Vector3 normalVector = new Vector3(transform.position.x - centerPosition.x, 0, transform.position.z - centerPosition.z).normalized;
        Vector3 moveVector = centerPosition + normalVector * 45 - transform.position;
        moveVector = new Vector3(moveVector.x, 0, moveVector.z);
        //transform.position = new Vector3(centerPosition.x+ normalVector.x* 30,transform.position.y+1, centerPosition.z + normalVector.z * 30);// 30만큼 멀어지도록.
        PC.SetMovedVoice(moveVector, 0.7f);
    }

    public void UseFountainMan()
    {
        
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Vector3 halfExtents = new Vector3(0.25f, 0.25f, 0.25f);  // 가로, 세로, 높이의 절반

        RaycastHit hit;
        if (Physics.BoxCast(ray.origin, halfExtents, ray.direction, out hit, Quaternion.identity, MaxDistance))
        {
            Debug.Log(hit.transform.tag); // this를 찍으면 Object 이름이 나옴 // hit.transform == "PlayerClone" // hit.transform.tag == "PlayerCheck"
            if (hit.transform.tag == "PlayerCheck")  // 정중앙 에임에 플레이어가 맞으면
            {
                PhotonView targetPV = hit.transform.parent.GetComponent<PhotonView>();
                targetPV.RPC("FountainHit", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void FountainHit()
    {
        Debug.Log("Fountain Hit!");
        GameObject effect = Resources.Load("Effects/Fountain") as GameObject;
        PlaySound("Fountain", 40f);

        Vector3 fountainPosition = transform.position;

        GameObject fountain = Instantiate(effect, fountainPosition, Quaternion.identity);
        fountain.transform.Rotate(Vector3.right, 90.0f);
        PS.Damage(3);
        if (PS.GetHp() > 0)
        {
            // 기존에 실행되고 있는 코루틴이 있다면
            if (CM.GetCoroutine() != null)
            {
                CM.NowStopCoroutine(); // 기존 코루틴을 종료
            }
            IEnumerator fountainCoroutine = FountainUp(PC, PS);
            CM.SetCoroutineInstance(fountainCoroutine);
            CM.StartStoredCoroutine();
        }
    }

    public IEnumerator FountainUp(PlayerController playerController, PlayerStatus playerStatus)
    {
        playerController.anim.SetBool("IsKnockDown", true);
        playerStatus.SetIsControll(false);
        playerController.SetNowJumpSpeed(50f);
        bool first = true;

        while ((!playerStatus.IsGrounded() && CM.GetcoroutineInstance() != null) || first)
        {
            first = false;
            yield return null;
        }
        playerController.anim.SetBool("IsKnockDown", false);
        CM.SetCoroutineInstance(null);
    }


    [PunRPC]
    public void AffectBombRPC()
    {
        // 폭발 효과
        PS.CantControll(3f);
        PS.Damage(1);
        PC.SetNowJumpSpeed(20f);
    }

    [PunRPC]
    public void SkillBomber()
    {
        PhotonView myPV = GetComponent<PhotonView>();
        // 이펙트는 모두에게 생성 되어야 함
        effect = Resources.Load("Effects/Bomber") as GameObject;
        GameObject BomberEffect = Instantiate(effect, transform.position, Quaternion.identity);
        PlaySound("Bomb", 40f);
        //GameObject BomberEffect = PhotonNetwork.Instantiate("Effects/CFXR Explosion 2", transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(BomberEffect, 3f);
        Debug.Log("Bomb!");
        if (myPV.IsMine)
        {
            Vector3 myPosition = transform.position;
            players = GameObject.FindGameObjectsWithTag("Player");
            PhotonView sPV;
            float distance;
            foreach (GameObject player in players)
            {
                sPV = player.GetComponent<PhotonView>();
                distance = (myPosition - player.transform.position).magnitude;
                if (distance <= 15)
                {
                    int damage = 75 - (int)(distance * 5);
                    sPV.RPC("BombDamage", RpcTarget.All, damage);
                }
            }

        }
    }
    
    public void SkillBomberStart()
    {
        PhotonView PV = GetComponent<PhotonView>();
        PV.RPC("SkillBomber", RpcTarget.All);
        //PV.RPC("SkillBomber", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void BombDamage(int damage) {
        PS.Damage(damage);
        PS.CantControll(2f);
    }

    public void SkillElectricSaw()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        PhotonView sPV;
        Collider[] hitColliders = Physics.OverlapBox(SawCheck.transform.position, SawBox, transform.rotation, layerMask);
        if (hitColliders.Length > 0)
        {
            foreach (Collider hitCollider in hitColliders)
            {
                sPV = hitCollider.transform.parent.GetComponent<PhotonView>();
                if (!sPV.IsMine)
                {
                    sPV.RPC("SkillElectricSawRPC", RpcTarget.All);
                }
            }
        }
    }

    [PunRPC]
    public void SkillElectricSawRPC()
    {
        PS.Damage(3);
    }

    public void UseElectricSaw(PhotonView tPV)
    {
        tPV.RPC("UseElectricSawRPC", RpcTarget.All);
    }

    [PunRPC]
    public void UseElectricSawRPC()
    {
        PlaySound("ElectricSaw", 40f);
    }

    //// Saw 범위 확인용 기즈모
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.cyan;

    //    // 회전을 적용한 기즈모 그리기
    //    Matrix4x4 originalMatrix = Gizmos.matrix;
    //    Gizmos.matrix = Matrix4x4.TRS(SawCheck.transform.position, transform.rotation, Vector3.one);
    //    Gizmos.DrawWireCube(Vector3.zero, SawBox);
    //    Gizmos.matrix = originalMatrix; // 기존 행렬로 복원
    //}

    //public void UsePILOT() // 범위
    //{
    //    Vector3 myPosition = transform.position;
    //    players = GameObject.FindGameObjectsWithTag("Player");
    //    PhotonView sPV;
    //    float distance;
    //    foreach (GameObject player in players)
    //    {
    //        sPV = player.GetComponent<PhotonView>();
    //        distance = (myPosition - player.transform.position).magnitude;
    //        if (!sPV.IsMine && distance <= 15)
    //        {
    //            PS.SetOtherControll(true);
    //            PS.SetOtherControllTime(5f);
    //            PC.SetOtherPlayerPhotonView(sPV);
    //            PC.SetOtherPlayer(sPV.gameObject);
    //            PC.SetOtherPlayerController(sPV.gameObject.GetComponent<PlayerController>());
    //            sPV.RPC("PilotHit", RpcTarget.All);
    //            PC.PilotCameraArmMove();
    //            return;
    //        }
    //    }
    //    PhotonView myPV = GetComponent<PhotonView>();
    //    myPV.RPC("PilotHitFail", RpcTarget.All);
    //    PC.nowSkillCooldown = 3f;
    //    PC.ResetSkillCoolDown();
    //    PS.CantControll(0.5f);
    //}

    public void UsePILOT() // 에임
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;
        PhotonView myPV = GetComponent<PhotonView>();
        if (Physics.Raycast(ray, out hit, 300f))
        {
            Debug.Log(hit.transform.tag); // this를 찍으면 Object 이름이 나옴 // hit.transform == "PlayerClone" // hit.transform.tag == "PlayerCheck"
            if (hit.transform.tag == "PlayerCheck")  // 정중앙 에임에 플레이어가 맞으면
            {
                PhotonView tPV = hit.transform.parent.GetComponent<PhotonView>();
                if (!tPV.IsMine)
                {
                    PlayerStatus tPS = tPV.gameObject.GetComponent<PlayerStatus>();
                    if (tPS.GetOtherControllToMe()) // 누군가 조종중이면 실패했으므로 데미지 입기, 대신 스킬 쿨 줄여주기
                    {
                        myPV.RPC("PilotHitFail", RpcTarget.All);
                        PC.nowSkillCooldown = 3f;
                        PC.ResetSkillCoolDown();
                        PS.CantControll(0.5f);
                        return;
                    }
                    else
                    {
                        PS.SetOtherControll(true);
                        PS.SetOtherControllTime(5f);
                        PC.SetOtherPlayerPhotonView(tPV);
                        PC.SetOtherPlayer(tPV.gameObject);
                        PC.SetOtherPlayerController(tPV.gameObject.GetComponent<PlayerController>());
                        tPV.RPC("PilotHit", RpcTarget.All);
                        PC.PilotCameraArmMove();
                        return;
                    }
                }
            }
        }
        // 맞추는데 실패하면 데미지 입기
        myPV.RPC("PilotHitFail", RpcTarget.All);
        PC.nowSkillCooldown = 3f;
        PC.ResetSkillCoolDown();
        PS.CantControll(0.5f);
    }

    [PunRPC]
    public void PilotHit() {
        PlaySound("PilotControl", 30f);
        PhotonView pilotPV = gameObject.GetComponent<PhotonView>();
        PS.SetOtherControllToMe(true);
        PS.SetOtherControllToMeTime(5f);
    }

    [PunRPC]
    public void PilotHitFail()
    {
        PS.Damage(1);
        //PC.nowSkillCooldown=3f;
        //PC.ResetSkillCoolDown();
        //PS.CantControll(0.5f);
    }


    // 효과음 재생을 위한 함수
    public void PlaySound(String sound,float maxDistance) // sound에는 soundsEffects에 있는 소리파일을 넣고, maxDistacne는 효과음이 어디까지 들리게할건지 설정 50f가 적절한듯, 0으로 넣으면 거리상관없이 다들림
    {
        // 최대거리보다 거리가 더 길면 소리 안들리게하는 조건문
        if (maxDistance > 0)
        {
            float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
            Debug.Log(distance);

            if (distance > maxDistance)
            {
                return;
            }
        }
        AudioClip effectSound = Resources.Load("EffectsSounds/"+sound) as AudioClip; // 사운드 클립 불러오기

        // AudioSource 컴포넌트가 없으면 추가하고, 있다면 가져옵니다.
        skillAudio.clip = effectSound;
        skillAudio.Play(); // 사운드 재생
    }
}
