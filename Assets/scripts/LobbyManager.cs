using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using DG.Tweening;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviour
{
    NetworkManager netManager;
    public Text countdown;
    public Text player1;
    public Text player2;
    public Image player2_background;
    bool player2_connected = false;

    float count = 0;
    void Start()
    {
        netManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        netManager.Lobby();
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.PlayerList.Length > 0 && PhotonNetwork.InRoom) {
            player1.text = PhotonNetwork.PlayerList[0].NickName;
            if (PhotonNetwork.PlayerList.Length > 1) {
                player2.text = PhotonNetwork.PlayerList[1].NickName;

                if (!player2_connected) {
                    player2_connected = true;

                    count = 0;

                    player2_background.transform.DOLocalMoveY(100, 0);

                    player2_background.transform.DOLocalMoveY(26.4f, 0.3f);
                }
            }
            else {
                player2_background.transform.localPosition = new Vector3(163.9f, 800, 0);
                player2_connected = false;

                countdown.text = "";
            }
        }

        if (player2_connected) {
            countdown.text = "게임 시작까지... " + Mathf.Floor(8 - count) + "s";

            count += Time.deltaTime;

            if (count >= 8) {
                LoadingController.LoadScene("Game");
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable(){
                    {
                        "state",
                        "loading"
                    }
                });
            }
        }
    }
}
