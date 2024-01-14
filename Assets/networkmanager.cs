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
        PhotonNetwork.SendRate = 60;//������ �����͸� �����ϴ½ð��� ���̴°�
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
        PhotonNetwork.LocalPlayer.NickName = nickname.text;//�������ִ� �г��Ӻ������� ������ �г������� �ٲ���
        PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions { MaxPlayers = 2 }, null);//������ ���� �İ� �ִ��ο����� 2������ �س���

    }
    public override void OnJoinedRoom()
    {
        panel.SetActive(false);//�濡 ���������� ���� ����â�� ��
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        panel.SetActive(true);
    }
}
