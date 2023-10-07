using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneToLobby : MonoBehaviour
{
    public void GotoLobby()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            Debug.Log("서버에 연결중");
        }
            //SceneManager.LoadScene("Lobby");
            //SceneManager.LoadScene(2);
    }
}
