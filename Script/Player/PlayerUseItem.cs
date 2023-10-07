using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUseItem : MonoBehaviour
{
    PlayerGetItem playerGetItem; // 아이템 획득 스크립트
    PlayerStatus playerStatus;  // 아이템으로 플레이어의 상태를 조절하기 위함
    float MaxDistance = 100f;  //아이템 사용 최대 거리
    PhotonView PV;
    PhotonView obstaclePhotonView;
    PlayerStatus PS;
    CharacterClass characterClass;
    CharacterClassSkillRPC skillRPC;
    GameObject[] obstacleToSynchro;

    // 스킬 사운드
    public AudioSource ItemUse;
    public AudioClip SpeedUp;
    public AudioClip Slow;
    public AudioClip Bomb;
    public AudioClip Wall;
    public AudioClip ChangeJob;
    public AudioClip ResetObs;
    public AudioClip ChangePosition;
    
    void Start()
    {
        playerStatus = transform.GetComponent<PlayerStatus>();
        playerGetItem = transform.GetComponent<PlayerGetItem>();
        PV = GetComponent<PhotonView>();
        PS = GetComponent<PlayerStatus>();
        characterClass = GetComponent<CharacterClass>();
        skillRPC=GetComponent<CharacterClassSkillRPC>();
        obstacleToSynchro = GameObject.FindGameObjectsWithTag("ObstacleToSynchro");
    }

    public void UseItem()
    {
        if (playerGetItem.myItem != null)  // 마우스 왼쪽 버튼을 누르고 item을 가지고 있을 때
        {
            Item item = playerGetItem.myItem.GetComponent<Item>();
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));  // 화면 정중앙 에임이 있는 곳
            Vector3 rayOrigin = ray.origin;  // ray의 시작점
            Vector3 rayDirection = ray.direction;  // ray의 방향
            RaycastHit hit;
            switch (item.type)
            {
                case Item.Type.Boost:  // 사용한 유저의 속도가 빨라지는 아이템
                    {
                        float playerSpeed = playerStatus.GetSpeed();
                        playerStatus.ChangeSpeed(playerSpeed * 3, 5f);
                        ItemUse.clip = SpeedUp;
                        ItemUse.Play();
                        break;
                    }
                case Item.Type.Wall:  // 사용한 지점에 벽을 생성하는 아이템
                    {
                        if (Physics.Raycast(ray, out hit, 20f))
                        {
                            Debug.Log("닿음!");
                            Vector3 newPoint = new Vector3(hit.point.x, hit.point.y - 2f, hit.point.z);  // 정중앙 에임 위치에서 y축으로 -5
                            Debug.Log(newPoint);
                            PV.RPC("CreateWall", RpcTarget.MasterClient, newPoint);  // y축으로 -5지점에 벽 생성
                        }

                        else
                        {
                            Debug.Log("안닿음!");
                            Vector3 newPoint = rayOrigin + rayDirection * 20f;  // ray의 끝부분의 좌표
                            newPoint.y = -2f;  // y축으로 0지점에 벽 생성
                            Debug.Log(newPoint);
                            PV.RPC("CreateWall", RpcTarget.All, newPoint);
                        }
                        ItemUse.clip = Wall;
                        ItemUse.Play();
                        break;
                    }
                case Item.Type.Slow:
                    {
                        RaycastHit[] hits = Physics.SphereCastAll(ray, 5f, MaxDistance);
                        foreach (RaycastHit hitInfo in hits)
                        {
                            if (hitInfo.transform.tag == "PlayerCheck")  // 정중앙 에임에 플레이어가 맞으면
                            {
                                PhotonView targetPV = hitInfo.transform.parent.GetComponent<PhotonView>();
                                PhotonView myPV = transform.GetComponent<PhotonView>();
                                Debug.Log(myPV);
                                if (targetPV != myPV)
                                {
                                    targetPV.RPC("ToSlow", RpcTarget.All);
                                }
                            }
                        }
                        ItemUse.clip = Slow;
                        ItemUse.Play();
                        break;
                    }
                case Item.Type.HyperKineticPositionReverser:
                    {
                        RaycastHit[] hits = Physics.SphereCastAll(ray, 5f, MaxDistance);
                        foreach (RaycastHit hitInfo in hits)
                        {
                            if (hitInfo.transform.tag == "PlayerCheck")
                            {
                                PhotonView targetPV = hitInfo.transform.parent.GetComponent<PhotonView>();
                                Vector3 myPosition = transform.position;  // 나의 위치
                                Vector3 targetPosition = hitInfo.transform.parent.position;  // 상대의 위치
                                Debug.Log("나의 현재 위치" + myPosition);
                                Debug.Log("상대의 현재 위치" + targetPosition);
                                PV.RPC("ReversePosition", RpcTarget.All, targetPosition);  // 나의 위치를 상대방의 위치와 바꾸고
                                targetPV.RPC("ReversePosition", RpcTarget.All, myPosition);  // 상대방의 위치를 나의 위치로 바꿈
                                Debug.Log(transform.position);
                            }
                        }
                        ItemUse.clip = ChangePosition;
                        ItemUse.Play();
                        break;
                    }
                case Item.Type.Bomb:
                    {
                        PV.RPC("CreateBomb", RpcTarget.MasterClient, rayDirection);
                        ItemUse.clip = Bomb;
                        ItemUse.Play();
                    }
                    break;
                case Item.Type.ResetClass:
                    {
                        characterClass.GetRandomClass(); // 랜덤직업 얻고
                        characterClass.ClassSetting();
                        skillRPC.PlaySound("ResetClass", 10f);
                        ItemUse.clip = ChangeJob;
                        ItemUse.Play();
                    }
                    break;
                case Item.Type.ObstacleReset:
                    {
                        foreach (GameObject obstacle in obstacleToSynchro)
                        {
                            Debug.Log("초기화");
                            obstaclePhotonView = obstacle.GetComponent<PhotonView>();
                            obstaclePhotonView.RPC("ResetToInitialState", RpcTarget.All);
                        }
                    }
                    break;
            }
        }
    }
    [PunRPC]
    public void CreateWall(Vector3 position)
    {
        GameObject Wall = PhotonNetwork.Instantiate("Item/IceWallPrefab", position, Quaternion.Euler(0, 0, 0));
    }

    [PunRPC]
    public void ToSlow()
    {
        Debug.Log("느려져라");
        PS.ApplyItemEffect(10f, 1f, 5f, 8f, 15f, true, true);
    }

    [PunRPC]
    public void ReversePosition(Vector3 position)
    {
        Debug.Log("위치변환!");
        transform.position = position;
        Debug.Log("나의위치" + position);
    }

    [PunRPC]
    public void CreateBomb(Vector3 rayDirection) 
    {
        Vector3 bombPosition = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z + 2f) ;
        GameObject Bomb = PhotonNetwork.Instantiate("Item/BombPrefab", bombPosition, Quaternion.Euler(0, 0, 0));
        Bomb.GetComponent<ThrowBomb>().Throw(rayDirection.normalized * 2000, PV);
        Debug.Log("생성됨");
    }

}
