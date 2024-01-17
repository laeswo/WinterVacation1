using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class playerscript : MonoBehaviourPunCallbacks, IPunObservable
{
   
    public PhotonView pv;
    public Rigidbody2D rg;
    public SpriteRenderer rd;

    void Awake()
    {
       
        rd.color = pv.IsMine ? Color.green : Color.red;
    }
        

    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
   
    }
}
