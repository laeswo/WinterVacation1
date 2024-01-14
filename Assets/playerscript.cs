using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class playerscript : MonoBehaviourPunCallbacks, IPunObservable
{
    public Text nickname;

    public PhotonView pv;
    public Rigidbody2D rg;
    public SpriteRenderer rd;

    void Awake()
    {
        nickname.text = pv.IsMine? PhotonNetwork.NickName: pv.Owner.NickName;

        rd.color = pv.IsMine ? Color.green : Color.red;
    }
        

    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
   
    }
}
