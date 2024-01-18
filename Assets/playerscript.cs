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
    Vector3 curPos;
    void Awake()
    {
       
        rd.color = pv.IsMine ? Color.green : Color.red;
    }

    void Update () 
    {
        if(pv.IsMine)
        {
            float axis = Input.GetAxisRaw("Horizontal");
            rg.velocity = new Vector2(4 * axis, rg.velocity.y);
        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
         
            }
            else
            {
                curPos = (Vector3)stream.ReceiveNext();
            
            }
        
    }
}
