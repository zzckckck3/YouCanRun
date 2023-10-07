using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;
    private bool isPunch;
    private bool isKick;
    private PhotonView pv;

    void Start()
    {
        anim = GetComponent<Animator>();
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
        {
            OneTwo();
            Kick();
        }
        
    }

    void OneTwo()
    {
        if (Input.GetMouseButton(0))
        {
            isPunch = true;
        }
        else
        {
            isPunch = false;
        }
        anim.SetBool("isOneTwo", isPunch);
    }

    void Kick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isKick = true;
        }
        else
        {
            isKick = false;
        }
        anim.SetBool("isKick", isKick);
    }
}
