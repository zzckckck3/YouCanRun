using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FirebaseAuthManager;


//PunCallbacks
public class PlayerCloth : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    public GameObject playerCharacter;
    
    void Start()
    {
        PV = GetComponent<PhotonView>();
        FirebaseAuthManager.UserData userData = GameManager.Instance.userData;
        string userHead = userData.head;
        string userBody = userData.body;
        if (PV.IsMine)
        {
            PV.RPC("myCloth", RpcTarget.AllBuffered, userHead, userBody);
        }
    }

    [PunRPC]
    public void myCloth(string userHead, string userBody)
    {

        if (userHead.Length == 1)
        {
            userHead = "0" + userHead;
        }
        if (userBody.Length == 1)
        {
            userBody = "0" + userBody;
        }

        int numOfChild = playerCharacter.transform.childCount;
        for (int i = 0; i < numOfChild; i++)
        {
            if (playerCharacter.transform.GetChild(i).gameObject.name.Contains("Head")
                || playerCharacter.transform.GetChild(i).gameObject.name.Contains("Body"))
            {
                playerCharacter.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        GameObject activeHeadInventory = playerCharacter.transform.Find("Head" + userHead).gameObject;
        GameObject activeBodyInventory = playerCharacter.transform.Find("Body" + userBody).gameObject;
        activeHeadInventory.SetActive(true);
        activeBodyInventory.SetActive(true);

    }
}
