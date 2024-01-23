using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    private Animator animator;
   public static List<PlayerScript> players;
    public PhotonView pv;
    public Rigidbody2D rg;
    public SpriteRenderer rd;
    public float moveSpeed;
    public string team;
    Vector3 curPos;
    public int MaxHealth = 100;
    public int health;
    public bool canHurt = true;
    void Awake()
    {
        animator = GetComponent<Animator>();
        //rd.color = pv.IsMine ? Color.green : Color.red;
        //players.Add(this);

        health = MaxHealth;
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

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (Input.GetKeyDown(KeyCode.E))
            {
                animator.SetBool("Idle", false);
                animator.SetBool("Hand_s", true);
                pv.RPC("AttackRPC", RpcTarget.All );
            }

            if (stateInfo.IsName("Hand_s") && stateInfo.normalizedTime >= 1.0f)
            {
                animator.SetBool("Idle", true);
                animator.SetBool("Hand_s", false);
            }

                

        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }

    public bool Damage(int damage) {
        if (canHurt) {
            health -= damage;

            if (health <= 0) {
                Debug.Log(team + " is Death");
                PhotonNetwork.Destroy(this.gameObject);
            }

            return true;
        } else {
            return false;
        }
    }

    public void Jump() {
        rg.velocity = new Vector2(rg.velocity.x, 10);
    }
    [PunRPC]
        void AttackRPC()
    {

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
