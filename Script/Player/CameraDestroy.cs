using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class CameraDestroy : MonoBehaviourPunCallbacks
{
    public PhotonView pv;

    private void Start()
    {
        Debug.Log("카메라 스크립트 실행");
        if (!pv.IsMine)
        {
            Debug.Log("카메라 파괴");
            Destroy(gameObject);
        }
    }
}
