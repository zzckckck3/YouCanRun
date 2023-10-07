using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishAndOut : MonoBehaviour
{

    public PhotonView pv;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FinishAndLeaveRoom()
    {
        string myNickname = GameManager.Instance.GetUserData().nickname;

        if (PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer.NickName.Equals(myNickname))
        {
            Debug.Log("룸 리스트로 이동 시도");
            PhotonNetwork.LeaveRoom();
            // 게임방 리스트로
            PhotonNetwork.LoadLevel("RoomList");
        }
    }
}
