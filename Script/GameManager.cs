using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using static FirebaseAuthManager;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public FirebaseAuthManager.UserData userData;
    public AudioMixer mixer;

    // GameManager Singleton 적용
    public static GameManager Instance
    {
        get { return instance; }
    }

    public int RoomNum;
    private void Awake()
    {
        if (instance == null)
        {
            // 이 GameManager 오브젝트가 다른 씬으로 전환될 때 파괴되지 않도록 함
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            // 이미 존재하는 GameManager 오브젝트가 있으므로 이 인스턴스를 파괴
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        FirebaseAuthManager.Instance.Init();
        mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("MasterSound", 0.75f)) * 20);
        mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("MusicSound", 0.75f)) * 20);
        mixer.SetFloat("Effect", Mathf.Log10(PlayerPrefs.GetFloat("EffectSound", 0.75f)) * 20);
    }

    private void OnApplicationQuit()
    {
        GameObject photon = GameObject.Find("PhotonManager");
        if (photon != null)
        {
            if(photon.GetComponent<PhotonManager>() != null)
            {
                photon.GetComponent<PhotonManager>().DisconnectFromServer();
            }
            if(photon.GetComponent<LobbyPhotonManager>() != null)
            {
                photon.GetComponent<LobbyPhotonManager>().DisconnectFromServer();
            }
            if (photon.GetComponent<RoomPhotonManager>() != null)
            {
                photon.GetComponent<RoomPhotonManager>().DisconnectFromServer();
            }
        }
        FirebaseAuthManager.Instance.LogOut();
    }

    // userData를 서버에서 받아오고, 또한 GameManager의 userData에 적용 후 이를 return
    public FirebaseAuthManager.UserData GetUserData()
    {
        userData = FirebaseAuthManager.Instance.GetUserData();
        return userData;
    }

    public string GetNickname()
    {
        string myNickname = GetUserData().nickname;
        return myNickname;
    }

    public void UpdateNickname(string nickname)
    {
        userData.nickname = nickname;
    }

    public void UpdateLevel(int level)
    {
        userData.level = level;
    }

    public void UpdatePoint(long point)
    {
        userData.point = point;
    }

    public void UpdateColor(string color)
    {
        userData.color = color;
    }

    public void UpdateHead(string head)
    {
        userData.head = head;
    }

    public void UpdateBody(string body)
    {
        userData.body = body;
    }

    public void UpdateIsFirstPlay(int isFirstPlay)
    {
        userData.isFirstPlay = isFirstPlay;
    }
}