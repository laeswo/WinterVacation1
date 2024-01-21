using System;
using Photon.Pun;
using UnityEngine;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
   
    public PhotonView pv;
    public Rigidbody2D rg;
    public SpriteRenderer rd;
    public float moveSpeed;
    public string team;
    Vector3 curPos;
    void Awake()
    {
        //rd.color = pv.IsMine ? Color.green : Color.red;
    }

    void Update () 
    {
        if(pv.IsMine)
        {
            float axis = Input.GetAxisRaw("Horizontal");
            transform.Translate(Vector2.right * axis * Time.deltaTime * moveSpeed);

            if (axis >= 1f) rd.flipX = false;
            else if (axis <= -1f) rd.flipX = true;

            if (Input.GetKeyDown(KeyCode.Space)) {
                Jump();
            }
        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }

    public void Jump() {
        rg.velocity = new Vector2(rg.velocity.x, 10);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(rd.flipX);
            }
            else
            {
                curPos = (Vector3)stream.ReceiveNext();
                rd.flipX = (bool)stream.ReceiveNext();
            
            }
        
    }
}
