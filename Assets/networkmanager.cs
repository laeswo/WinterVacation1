using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class networkmanager : MonoBehaviourPunCallbacks
{
    public GameObject panel;
    public InputField nickname;

 void Awake()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape)&&PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }
    public void spawn()
    {
        PhotonNetwork.Instantiate("player",Vector3.zero, Quaternion.identity);
    }
    public void connct() =>   PhotonNetwork.ConnectUsingSettings();
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = nickname.text;
        PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions { MaxPlayers = 2 }, null);
    }
    public override void OnJoinedRoom()
    {
        panel.SetActive(false);
        spawn();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        panel.SetActive(true);
    }
}
