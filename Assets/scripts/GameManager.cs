using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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
    }

    void Update()
    {
        if (isWaiting) {
            if (PhotonNetwork.PlayerListOthers.Length < 1) {
                StartCoroutine(Disconnected());
            } else {
                Hashtable ht = PhotonNetwork.PlayerListOthers[0].CustomProperties;
                // if (ht["state"] != "loading") {

                // }
            }
        }
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
