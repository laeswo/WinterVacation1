using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    private Animator animator;
    public PhotonView pv;
    public Rigidbody2D rg;
    public SpriteRenderer rd;
    public float moveSpeed;
    public string team;
    Vector3 curPos;
    public int MaxHealth = 100; 
    public int health;
    public bool canHurt = true;
    public GameObject mine;
    public string action;
    public bool sendAction = false;
    public bool isMoving = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        //rd.color = pv.IsMine ? Color.green : Color.red;
        //players.Add(this);

        health = MaxHealth;

        mine.SetActive(pv.IsMine);

        GameManager.instance.players.Add(this);
    }

    void Update () 
    {
        if(pv.IsMine)
        {
            float axis = Input.GetAxisRaw("Horizontal");
            transform.Translate(Vector2.right * axis * Time.deltaTime * moveSpeed);

            isMoving = axis != 0;

            if (axis >= 1f) rd.flipX = false;
            else if (axis <= -1f) rd.flipX = true;

            if (Input.GetKeyDown(KeyCode.Space)) {
                Jump();
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                HandSkill();
            }

        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);

        animator.SetBool("isMoving", isMoving);

        var ht = pv.Owner.CustomProperties;
        var damage = ht["damage"] as int?;

        if (damage.HasValue && damage.Value > 0 && pv.IsMine) {
            damageSelf(damage.Value);

            Debug.Log(damage);

            pv.Owner.SetCustomProperties(new Hashtable(){
                {
                    "damage",
                    -1
                }
            });
        }
    }

    public void HandSkill() {
        if (pv.IsMine) {
            int facing = rd.flipX ? -1 : 1;

            var target = BattleSystem.GetTargetPlayer(this, 4, new Vector2(transform.position.x + facing * 2, transform.position.y + 1));
            Debug.Log(target);
            if (target != null) {
                target.Damage(10);
            }

            animator.SetTrigger("hand_skill");

            action = "hand_skill";
            sendAction = true;
        } else {
            animator.SetTrigger("hand_skill");
        }
    }

    void damageSelf(int damage_) {
        health -= damage_;

        if (health <= 0) {
            Debug.Log(team + " is Death");
        }
    }

    public bool Damage(int damage_) {
        if (canHurt) {
            object[] param = {
                damage_,
            };
            pv.RPC("OnDamaged", RpcTarget.All, param);
            // pv.Owner.SetCustomProperties(new Hashtable(){
            //     {
            //         "damage",
            //         damage_
            //     }
            // });

            return true;
        } else {
            return false;
        }
    }

    [PunRPC]
    void OnDamaged(int damage){
        health -= damage;
    }

    public void Jump() {
        rg.velocity = new Vector2(rg.velocity.x, 10);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(health);
                stream.SendNext(rd.flipX);
                stream.SendNext(action);
                stream.SendNext(sendAction);
                stream.SendNext(team);
                stream.SendNext(isMoving);

                if (sendAction) {
                    action = "";
                    sendAction = false;
                }
            }
            else
            {
                curPos = (Vector3)stream.ReceiveNext();
                health = (int)stream.ReceiveNext();
                rd.flipX = (bool)stream.ReceiveNext();
                var action = (string)stream.ReceiveNext();
                var callAction = (bool)stream.ReceiveNext();
                team = (string)stream.ReceiveNext();
                isMoving = (bool)stream.ReceiveNext();

                if (callAction) {
                    if (action == "hand_skill") {
                        HandSkill();
                    }
                }
            }
        
    }
}
