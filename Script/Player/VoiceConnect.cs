using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class VoiceConnect : MonoBehaviourPunCallbacks
{
    public bool AutoConnect = true;
    public byte Version = 1;
    // Start is called before the first frame update
    //public void Start()
    //{
    //    if (this.AutoConnect)
    //    {
    //        this.ConnectNow();
    //    }
    //}

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room in region [" + PhotonNetwork.CloudRegion + "]. Game is now running.");
    }
    // Update is called once per frame


    //public void ConnectNow()
    //{
    //    Debug.Log("ConnectAndJoinRandom.ConnectNow() will now call: PhotonNetwork.ConnectUsingSettings().");


    //    PhotonNetwork.ConnectUsingSettings();
    //}

}
