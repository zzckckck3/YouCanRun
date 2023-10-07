using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class Respawn : MonoBehaviour
{
    public Transform[] points;
    public Transform characterTransform;
    public CoroutineManager coroutineManager;
    public GameObject finishLineObject;
    public Transform finishLine;
    public PhotonView pv;
    private PlayerController PC;

    //리스폰 사운드
    public AudioSource RespawnSound;

    // 도착지 정보
    public float xMin;
    public float xMax;
    public float yMin;
    public float zMin;

    void Start()
    {
        PC = GetComponent<PlayerController>();
        finishLine = finishLineObject.transform;       
        //xMin = finishLine.transform.position.x - 20.5f;
        //xMax = finishLine.transform.position.x + 20.5f;
        if(SceneManager.GetActiveScene().name == "NewImHyeonRacingMap")
        {
            xMin = -70f;
            xMax = -51.5f;
            yMin = 9.2f;
            zMin = -150f;
        }
        else if (SceneManager.GetActiveScene().name == "MinWooRacingMap")
        {
            xMin = -21f;
            xMax = -11f;
            yMin = 82f;
            zMin = -335f;
        }
        else if (SceneManager.GetActiveScene().name == "Makgora")
        {
            xMin = -22.8f;
            xMax = -12.8f;
            yMin = -0.5f;
            zMin = -254.5f;
        }
        else if (SceneManager.GetActiveScene().name == "SpinningRacingMap")
        {
            xMin = -33.45f;
            xMax = 33.69f;
            yMin = -0.5f;
            zMin = -280f;
        }


        coroutineManager = GameObject.Find("CoroutineManager").GetComponent<CoroutineManager>();
        if (SceneManager.GetActiveScene().name != "Lobby")
        {
            points = GameObject.Find("RespawnSpot").GetComponentsInChildren<Transform>();
            finishLineObject = GameObject.Find("FinishLine");
}
        int idx = Random.Range(0, points.Length);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)){
            if(SceneManager.GetActiveScene().name == "Lobby")
            {
                Res();
            }
        }

        //if (Input.GetKeyDown(KeyCode.P) && pv.IsMine)
        //{
        //    characterTransform.position = new Vector3(-60f, 83f, -176f);
        //}

        if (transform.position.y < -50)
        {
            Res();
        }
        if (transform.position.x >= xMin && transform.position.x <= xMax &&
                transform.position.y >= yMin &&
                transform.position.z >= zMin)
        {
            
            // 완주하지 않았을때만 리스폰aw
            if(pv.IsMine && RoomPhotonManager.Instance.isClear == 0 && RoomPhotonManager.Instance.countdownTimeMs > 0)
            {
                // 플레이어 리스폰
                Res();
                RoomPhotonManager.Instance.UpdatePlayerRound();
            }
        }
    }




    public void Res()
    {
        coroutineManager.NowStopCoroutine();
        transform.GetComponent<PlayerController>().anim.SetBool("IsKnockDown", false);
        points = GameObject.Find("RespawnSpot").GetComponentsInChildren<Transform>();
        RespawnSound.Play();
        if (pv.IsMine) {
            PC.ResetStatus();
            int idx = Random.Range(0, points.Length);
            pv.RPC("RespawnSeat",RpcTarget.All, points[idx].position);
        }
        //Debug.Log(characterTransform.position);
    }

    [PunRPC]
    public void RespawnSeat(Vector3 myPosition) { // 리스폰 위치를 RPC로 동기화해 공중에 없도록 수정
        characterTransform.position = myPosition;
    }

}
