using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviour
{
    public List<PlayerScript> players = new();
    bool isWaiting;
    bool isStarted;
    public Text cause;
    public Image loading;
    public Image cover;
    public Slider blue_hp;
    public Text blue_name;
    public Text blue_score;
    public Slider red_hp;
    public Text red_name;
    public Text red_score;
    public Text round_text;
    public Text timer_text;
    public float timer;
    public float round;
    public static GameManager instance;
    void Start()
    {
        instance = this;

        isWaiting = true;
        isStarted = false;
        round = 1;
        timer = 60;

        loading.gameObject.SetActive(true);
        cause.text = "상대 플레이어의 로딩이 끝날 때까지 대기합니다...";

        if (PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable(){
            {
                "state",
                "waiting"
            }
        });

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable(){
            {
                "state",
                "waiting"
            }
        });

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable(){
            {
                "score",
                0
            }
        });
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

                if (state == "waiting") {
                    isWaiting = false;

                    loading.gameObject.SetActive(false);
                    cause.text = "";
                    Debug.Log(state);
                    AfterLoading();
                }
            }
        }

        int? blueScore = PhotonNetwork.PlayerList[0].CustomProperties["score"] as int?;
        if (blueScore != null) {
            blue_score.text = blueScore.ToString();
        }
        blue_name.text = PhotonNetwork.PlayerList[0].NickName;

        var blue = BattleSystem.GetParticularPlayer(Team.Blue);
        if (blue != null) blue_hp.value = blue.health * 100 / blue.MaxHealth;


        int? redScore = PhotonNetwork.PlayerList[1].CustomProperties["score"] as int?;
        if (redScore != null) {
            red_score.text = redScore.ToString();
        }
        red_name.text = PhotonNetwork.PlayerList[1].NickName;

        var red = BattleSystem.GetParticularPlayer(Team.Red);
        if (red != null) red_hp.value = red.health * 100 / red.MaxHealth;

        Hashtable htr = PhotonNetwork.CurrentRoom.CustomProperties;
        string stateR = htr["state"] as string;

        round_text.text = "ROUND " + round.ToString();
        timer_text.text = ((int)timer).ToString();

        if (stateR == "starting" && !isStarted) {
            isStarted = true;
        }

        if (isStarted) {
            timer -= Time.deltaTime;
        }
    }

    public void spawn()
    {
        if (PhotonNetwork.LocalPlayer.GetHashCode().Equals(PhotonNetwork.PlayerList[0].GetHashCode())) {
            GameObject obj = PhotonNetwork.Instantiate("player_blue",new Vector3(-8.3f, 0f, 0), Quaternion.identity);
            obj.GetComponent<PlayerScript>().team = "blue";

        } else {
            GameObject obj = PhotonNetwork.Instantiate("player_red",new Vector3(8.3f, 0f, 0), Quaternion.identity);
            obj.GetComponent<SpriteRenderer>().flipX = true;
            obj.GetComponent<PlayerScript>().team = "red";
        }
    }

    void AfterLoading() {
        spawn();

        StartCoroutine(AfterLoading_());
    }

    IEnumerator AfterLoading_() {   

        yield return new WaitForSeconds(1);
        for (int i = 0; i < 3; i++) {
            cause.text = "게임 시작까지... " + (3 - i).ToString();
            yield return new WaitForSeconds(1);
        }

        cause.text = "START!";

        yield return new WaitForSeconds(0.6f);

        cause.text = "";

        if (PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable(){
            {
                "state",
                "starting"
            }
        });
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
