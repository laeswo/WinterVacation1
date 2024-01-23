using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;

public class attack : MonoBehaviour
{
    public PhotonView pv;
    public Collider2D col;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (col.tag == "ground") pv.RPC("DestroyRPC", RpcTarget.AllBuffered);
        if (!pv.IsMine && col.tag == "player" && col.GetComponent<PhotonView>().IsMine) // 느린쪽에 맞춰서 Hit판정
        {
            col.GetComponent<PlayerScript>().Damage(5);
            pv.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }   
    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);
}
