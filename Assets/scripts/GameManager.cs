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
    public Sprite noImage;
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
    public bool doAction = false;
    void Start()
    {
        instance = this;

        isWaiting = true;
        isStarted = false;
        round = 1;
        timer = 60;

        loading.gameObject.SetActive(true);
        cause.text = "상대 플레이어의 로딩이 끝날 때까지 대기합니다...";

        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable(){
            {
                "state",
                "waiting"
            }
            });
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable(){
            {
                "state",
                "waiting"
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

        blue_name.text = PhotonNetwork.PlayerList[0].NickName;

        var blue = BattleSystem.GetParticularPlayer(Team.Blue);
        if (blue != null) {
            if (blue.health < 0) blue.health = 0;
            blue_hp.value = blue.health * 100 / blue.MaxHealth;
            blue_score.text = blue.score.ToString();
        }



        red_name.text = PhotonNetwork.PlayerList[1].NickName;

        var red = BattleSystem.GetParticularPlayer(Team.Red);
        if (red != null) {
            if (red.health < 0) red.health = 0;
            red_hp.value = red.health * 100 / red.MaxHealth;
            red_score.text = red.score.ToString();
        }

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

        if (doAction) {
            if (timer <= 0 || red.health <= 0 || blue.health <= 0) {
                if (timer <= 0) cause.text = "타임 오버!";
                else cause.text = "라운드 종료!";
                StartCoroutine(NextRound(red, blue));
            }
        }
    }

    IEnumerator NextRound(PlayerScript red, PlayerScript blue) {
        doAction = false;
        if (red.score >= 3 && blue.score >= 3) {
            cause.text = "무승부!";

            yield return new WaitForSeconds(1);

            StartCoroutine(Disconnected());

            yield break;
        }
        else if (red.score >= 3) {
            cause.text = "레드 승리!";

            yield return new WaitForSeconds(1);

            StartCoroutine(Disconnected());

            yield break;
        }
        else if (blue.score >= 3) {
            cause.text = "블루 승리!";

            yield return new WaitForSeconds(1);

            StartCoroutine(Disconnected());

            yield break;
        }
        yield return new WaitForSeconds(1);
        if (red.health > blue.health) {
            cause.text = "레드팀이 1점을 획득합니다!";
            red.score++;
        } else if (blue.health > red.health) {
            cause.text = "블루팀이 1점을 획득합니다!";
            blue.score++;
        } else {
            cause.text = "무승부! 둘 다 1점을 획득합니다!";
            red.score++;
            blue.score++;
        }

        yield return new WaitForSeconds(1.2f);

        round++;
        cause.text = "라운드 " + round;

        blue.transform.position = new Vector3(-8.3f, 0f, 0);
        red.transform.position = new Vector3(8.3f, 0f, 0);

        timer = 60;

        blue.health = blue.MaxHealth;
        red.health = red.MaxHealth;

        yield return new WaitForSeconds(1f);

        cause.text = "START!";

        doAction = true;

        yield return new WaitForSeconds(0.7f);
        cause.text = "";
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

        doAction = true;

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
