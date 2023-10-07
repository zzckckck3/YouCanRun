using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRPC : MonoBehaviourPun
{
    

    private GameObject[] obstacleToSynchro;
    private PhotonView obstaclePhotonView; // 장애물의 PhotonView 참조
    private void Start()
    {
        obstacleToSynchro = GameObject.FindGameObjectsWithTag("ObstacleToSynchro");
    }
    private void OnTriggerEnter(Collider other)
    {
        // 박스와 부딪혔다면
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            StartGame();
        }
    }
    private void StartGame()
    {
        foreach (GameObject obstacle in obstacleToSynchro)
        {
            obstaclePhotonView = obstacle.GetComponent<PhotonView>();
            obstaclePhotonView.RPC("ResetToInitialState", RpcTarget.All);
        }
    }
}
