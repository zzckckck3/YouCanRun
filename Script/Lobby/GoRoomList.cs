using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoRoomList : MonoBehaviourPunCallbacks
{

    public GameObject myCharacter;
    public PhotonView pv;
    public string myNickname;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("toListPortal"))
        {
            // 현재 씬 정보 가져오기
            string myNickname = GameManager.Instance.GetUserData().nickname;

            if (pv.IsMine && PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer.NickName.Equals(myNickname))
            {
                Debug.Log("룸 리스트로 이동 시도");
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Destroy(pv);
                // 게임방 리스트로
                PhotonNetwork.LoadLevel("RoomList");
            }
        }
    }
}