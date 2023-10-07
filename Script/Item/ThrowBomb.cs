using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBomb : MonoBehaviour
{
    private GameObject effect;
    public LayerMask layerMask;
    PhotonView PV;
    PhotonView MasterPV;

    void Start()
    {

        PV = GetComponent<PhotonView>();
    }

    public void Throw(Vector3 dir, PhotonView masterPV)
    {
        GetComponent<Rigidbody>().AddForce(dir);
        MasterPV = masterPV;
    }

    private void OnCollisionEnter(Collision other)
    {
        UseBomb();
        AffectBomb();
        Destroy(gameObject);
    }

    public void UseBomb()
    {
        // 폭발 파티클 호출
        PV.RPC("UseBombRPC", RpcTarget.All);
    }

    public void AffectBomb()
    {
        // 폭발 효과 호출
        Collider[] colliders = Physics.OverlapSphere(transform.position, 3.5f, layerMask);
        PhotonView AroundPV;
        foreach (Collider col in colliders)
        {
            AroundPV = col.transform.parent.GetComponent<PhotonView>();
            Debug.Log("MasterPV:" + MasterPV);
            if (AroundPV != MasterPV)
            {
                //Vector3 otherTransform = col.transform.parent.position;
                //Vector3 explosionDirection = otherTransform - transform.position;
                //explosionDirection.Normalize();

                //float pushDistance = 10.0f;  // 원하는 밀리는 거리 설정
                //otherTransform += explosionDirection * pushDistance;
                Debug.Log("AroundPV:" + AroundPV);
                AroundPV.RPC("AffectBombRPC", RpcTarget.All);
            }

        }
    }

    [PunRPC]
    public void UseBombRPC()
    {
        // 폭발 파티클
        effect = Resources.Load("Effects/CFXR Explosion 2") as GameObject;
        GameObject instance = Instantiate(effect, transform.position, Quaternion.identity);
        Destroy(instance, 2.5f); // 2.5초 후에 인스턴스 파괴
        
    }
}
