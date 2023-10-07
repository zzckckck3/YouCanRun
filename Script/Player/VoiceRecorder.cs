using UnityEngine;
using Photon.Voice.Unity;
using Photon.Pun;
using System.IO;

public class VoiceRecorder : MonoBehaviour
{
    private Recorder recorder; // Recorder 컴포넌트를 여기에 연결하세요.
    private GameObject VoiceObject;
    private GameObject cc;
    public PhotonView PV;
    GameObject[] players;   

    // 로그 확인
    //string pathT = @"C:\RPCTest Log\KeyDownRPC_unityin.txt";
    //StreamWriter sw = null;

    private void Start()
    {
        VoiceObject = GameObject.Find("Voice");
        recorder = VoiceObject.GetComponent<Recorder>();
        cc= gameObject;
    }
    void Update()
    {
        
    }
}
