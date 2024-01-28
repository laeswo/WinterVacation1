using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class BossAtk : MonoBehaviourPunCallbacks, IPunObservable
{
    float randX;
    public PhotonView pv;
    void Start()
    {
        randX = 6 - GameManager.instance.timer / 10;
        if (randX % 2 == 0) {
            randX *= -1;
        }

        StartCoroutine(Attack());
    }

    IEnumerator Attack() {
        transform.DOMove(new Vector3(randX, -4), 0.33f);
        yield return new WaitForSeconds(0.33f);

        var pls = BattleSystem.GetPlayersOfCenter(transform.position, 1.5f);

        foreach (var p in pls) {
            p.Damage(30);
        }

        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
