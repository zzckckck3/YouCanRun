using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using System.Runtime.CompilerServices;

public class PlayerWithObstacles : MonoBehaviourPunCallbacks
{
    private CharacterController characterController;
    private PlayerController playerController;
    private PhotonView PV;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
        PV = GetComponent<PhotonView>();
    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (PV.IsMine)
        {
            if (hit.gameObject.CompareTag("RotateObstacle"))
            {
                Debug.Log("충돌");
                Vector3 collisionNormal = hit.normal;
                Debug.Log(collisionNormal);
                Debug.Log("z축방향" + hit.transform.forward);
            }
        }
    }
}