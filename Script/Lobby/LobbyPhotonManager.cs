using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyPhotonManager : MonoBehaviourPunCallbacks
{
    public static LobbyPhotonManager instance;
    public static LobbyPhotonManager Instance
    {
        get { return instance; }
    }

    public GameObject canvas;

    public void DisconnectFromServer()
    {
        // Photon 서버와의 연결 해제
        PhotonNetwork.Disconnect();
    }

    private void Awake()
    {
        int numOfChild = canvas.transform.childCount;
        for (int i = 0; i < numOfChild; i++)
        {
            canvas.transform.GetChild(i).gameObject.SetActive(false);
        }
        canvas.transform.Find("Loading").gameObject.SetActive(true);

        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;
        PhotonNetwork.JoinOrCreateRoom("Lobby", ro, null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {

    }

    public override void OnLeftRoom()
    {
        if(SceneManager.GetActiveScene().name == "Lobby") 
        {
            SceneManager.LoadScene("UI");
            return;
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Create Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinedRoom()
    {
        GameObject playerCharacter;

        Debug.Log($"In Room = {PhotonNetwork.InRoom}");

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }

        // 캐릭터 배치 코드
        Transform[] points = GameObject.Find("RespawnSpot").GetComponentsInChildren<Transform>();
        int idx = Random.Range(0, points.Length);
        playerCharacter = PhotonNetwork.Instantiate("PlayerCharacter", points[idx].position, points[idx].rotation, 0);

        int numOfChild = canvas.transform.childCount;
        for (int i = 0; i < numOfChild; i++)
        {
            if (canvas.transform.GetChild(i).gameObject.name == "EscapeWindow" ||
                canvas.transform.GetChild(i).gameObject.name == "Loading" ||
                canvas.transform.GetChild(i).gameObject.name == "Dictionary" ||
                canvas.transform.GetChild(i).gameObject.name == "Ranking")
            {
                canvas.transform.GetChild(i).gameObject.SetActive(false);
            } else
            {
                canvas.transform.GetChild(i).gameObject.SetActive(true);
            }
            
        }

        //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //Debug.Log("플레이어 수 확인 : "+players.Length);

    }
}
 