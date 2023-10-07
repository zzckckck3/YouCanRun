using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public GameObject Player;

    void Start()
    {
    }

    void Update()
    {
        Debug.Log(LobbyPhotonManager.instance);
    }
}
