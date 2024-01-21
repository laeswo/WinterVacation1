using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviour
{
    bool isWaiting;
    public Text cause;
    public Image loading;
    public Image cover;
    void Start()
    {
        isWaiting = true;
        loading.gameObject.SetActive(true);
        cause.text = "상대 플레이어의 로딩이 끝날 때까지 대기합니다...";

        bool ok = PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable(){
            {
                "state",
                "waiting"
            }
        });

        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["state"]);
    }

    void Update()
    {
        if (PhotonNetwork.PlayerListOthers.Length < 1) {
            StartCoroutine(Disconnected());
        }
        if (isWaiting) {
            if (PhotonNetwork.PlayerListOthers.Length > 0) {
                Hashtable ht = PhotonNetwork.PlayerListOthers[0].CustomProperties;
                string state = ht["state"] as string;

                if (state != "loading") {
                    isWaiting = false;

                    loading.gameObject.SetActive(false);
                    cause.text = "";
                    Debug.Log(state);
                    AfterLoading();
                }
            }
        }
    }

    public void spawn()
    {
        if (PhotonNetwork.LocalPlayer.GetHashCode().Equals(PhotonNetwork.PlayerList[0].GetHashCode())) {
            PhotonNetwork.Instantiate("player",new Vector3(-8.3f, -2f, 0), Quaternion.identity);
        } else {
            PhotonNetwork.Instantiate("player",new Vector3(8.3f, -2f, 0), Quaternion.identity);
        }
    }

    void AfterLoading() {
        spawn();
    }

    IEnumerator Disconnected() {
        cover.gameObject.SetActive(true);
        cause.text = "상대 플레이어의 연결이 끊켰습니다.\n게임이 종료됩니다...";
        loading.gameObject.SetActive(true);

        PhotonNetwork.Disconnect();

        yield return new WaitForSeconds(1.5f);

        LoadingController.LoadScene("lobby");
    }
}
