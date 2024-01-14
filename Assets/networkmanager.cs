using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField nickname;
    public GameObject panel;

    void Awake()
    {
        PhotonNetwork.SendRate = 60;//서버로 데이터를 전송하는시간을 줄이는거
        PhotonNetwork.SerializationRate = 30;
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape)&&PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }
    public void Connect() => PhotonNetwork.ConnectUsingSettings();
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = nickname.text;//서버에있는 닉네임변수에다 지정한 닉네임으로 바꿔줌
        PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions { MaxPlayers = 2 }, null);//서버에 방을 파고 최대인원수를 2명으로 해놓음

    }
    public override void OnJoinedRoom()
    {
        panel.SetActive(false);//방에 참가했을때 게임 접속창을 끔
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        panel.SetActive(true);
    }
}
