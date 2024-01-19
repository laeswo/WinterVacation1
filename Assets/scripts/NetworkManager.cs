using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject panel;
    public InputField nickname;
    public Text e;
    public Text p;

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
        if (PhotonNetwork.PlayerList.Length > 1) {
            if (PhotonNetwork.PlayerList[1].NickName.Length > 0)
            {
                p.text = PhotonNetwork.PlayerList[0].NickName;
                e.text = PhotonNetwork.PlayerList[1].NickName;
            }
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
