using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalInMoveToRoom : MonoBehaviourPunCallbacks
{
    public GameObject myCharacter;
    public PhotonView pv;
    
    private void OnTriggerEnter(Collider other)
    {
       
        //PhotonView pv = ob.GetComponent<PhotonView>();
        if (other.gameObject.CompareTag("Room3Portal"))
        {
            Debug.Log("3번방 입장 시도");
            toRoom();
        }
    }

    // 게임 대기방으로 이동.
    private void toRoom()
    {
        if (pv.IsMine)
        {
            PhotonNetwork.Destroy(myCharacter);
            PhotonNetwork.LoadLevel("WaitingRoom");
        }
    }

    // 로비로 이동
    private void toLobby()
    {
        if (pv.IsMine)
        {
            PhotonNetwork.Destroy(myCharacter);
            SceneManager.LoadScene("Lobby");
        }
    }

}
