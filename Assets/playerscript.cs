using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class playerscript : MonoBehaviourPunCallbacks, IPunObservable
{
    public Image hpbar;
    public Text nickname;
    public Text nickname_enemey;
    public PhotonView pv;
    public Rigidbody2D rg;

    void Update()
    {
        nickname.text = PhotonNetwork.NickName;
        nickname.text = pv.Owner.NickName;
    }
        

    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
   
    }
}
