using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject panel;
    public GameObject onRoom;
    public GameObject loading;
    public InputField nickname;
    public Button room_btn;

    public static NetworkManager instance = null;

    void Awake()
    {
        if (instance) {
            instance.panel = this.panel;
            instance.nickname = this.nickname;
            instance.onRoom = this.onRoom;
            instance.loading = this.loading;

            DestroyImmediate(this.gameObject);

            return;
        }

        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        instance = this;

        DontDestroyOnLoad(gameObject);

        if (panel != null) panel.SetActive(false);
    }

    public void Lobby() {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        Transform list = GameObject.Find("rmList").transform;
        Transform content = list.Find("Viewport").Find("Content");

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        int padding = 0;
        foreach (RoomInfo rm in roomList) {
            if (rm.MaxPlayers < 1) continue;
            
            Button btn = Instantiate(room_btn, new Vector3(0, 123.17f - padding * 55, 0), Quaternion.identity);

            btn.transform.SetParent(content, false);

            btn.transform.localScale = new Vector3(1, 1, 1);

            btn.name = rm.Name;

            if (rm.PlayerCount >= rm.MaxPlayers) {
                btn.transform.Find("can").GetComponent<Image>().color = Color.red;
            }

            btn.transform.Find("name").GetComponent<Text>().text = rm.Name;
            btn.transform.Find("count").GetComponent<Text>().text = rm.PlayerCount + "/" + rm.MaxPlayers;

            padding++;
        }
    }

    public override void OnConnectedToMaster()
    {
        panel.SetActive(true);
        loading.SetActive(false);
        Debug.Log(PhotonNetwork.JoinLobby());
        // PhotonNetwork.LocalPlayer.NickName = nickname.text;
        // PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions { MaxPlayers = 2 }, null);
    }
    public void CreateRoom() {
        if (nickname.text.Length < 1) return;

        PhotonNetwork.LocalPlayer.NickName = nickname.text;
        PhotonNetwork.JoinOrCreateRoom(nickname.text + "#" + Random.Range(1000, 9999), new RoomOptions { MaxPlayers = 2 }, null);

        panel.SetActive(false);
    }
    public void RandMatch() {
        if (nickname.text.Length < 1) return;

        PhotonNetwork.LocalPlayer.NickName = nickname.text;
        bool ok = PhotonNetwork.JoinRandomRoom();

        if (ok) {
            panel.SetActive(false);
            loading.SetActive(true);
        }
    }
    public void JoinCustomRoom(string name) {
        if (nickname.text.Length < 1) return;

        PhotonNetwork.LocalPlayer.NickName = nickname.text;
        bool ok = PhotonNetwork.JoinRoom(name);

        if (ok) {
            panel.SetActive(false);
            loading.SetActive(true);
        }
    }
    public override void OnJoinedRoom()
    {
        onRoom.SetActive(true);
        loading.SetActive(false);
        //spawn();
       
        
    }
    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();

        loading.SetActive(true);
        onRoom.SetActive(false);
    }

    public override void OnLeftRoom()
    {
        loading.SetActive(false);
        panel.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (panel != null) panel.SetActive(true);
    }
}
