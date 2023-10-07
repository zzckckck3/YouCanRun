using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetNickName : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    // 닉네임 Text
    public TextMeshProUGUI textMesh;
    void Start()
    {
        PV = GetComponent<PhotonView>();
        string nickname = GameManager.Instance.userData.nickname;
        if (PV.IsMine)
        {
            PV.RPC("SetNickNameRPC", RpcTarget.AllBuffered, nickname);
        }
    }

    [PunRPC]
    public void SetNickNameRPC(string nickname)
    {
        // 오브젝트 위의 닉네임을 GameManager 에서 가져오기
        textMesh.text = nickname;
    }
}
