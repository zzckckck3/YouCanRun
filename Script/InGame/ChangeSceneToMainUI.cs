using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneToMainUI : MonoBehaviour
{
    void Start()
    {
        FirebaseAuthManager.Instance.Init();
    }
    public void GotoMainUI()
    {
        //SceneManager.LoadScene("UI");
        /*GameObject photon = GameObject.Find("PhotonManager");
        if (photon != null)
        {
            if (photon.GetComponent<PhotonManager>() != null)
            {
                photon.GetComponent<PhotonManager>().DisconnectFromServer();
            }
            if (photon.GetComponent<LobbyPhotonManager>() != null)
            {
                photon.GetComponent<LobbyPhotonManager>().DisconnectFromServer();
            }
        }
        FirebaseAuthManager.Instance.LogOut();*/

        PhotonNetwork.LeaveRoom();
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {

        }
        else if(SceneManager.GetActiveScene().name != "Lobby")
        {
            PhotonNetwork.LoadLevel("RoomList");
        }
    }
}
