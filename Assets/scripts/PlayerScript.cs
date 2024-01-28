using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    public float noImage = 0;
    public Sprite defImage;
    public bool isJumping = false;
    public bool stopAtk = false;
    public Vector3 scale;
    bool flip_before = false;
    public int score = 0;

    void Awake()
    {
        animator = GetComponent<Animator>();
        //rd.color = pv.IsMine ? Color.green : Color.red;
        //players.Add(this);

        health = MaxHealth;

        mine.SetActive(pv.IsMine);

        GameManager.instance.players.Add(this);

        defImage = rd.sprite;

        scale = transform.localScale;
    }

    void Update () 
    {
        if (GameManager.instance.doAction) {
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

                if (Input.GetKeyDown(KeyCode.Q)) {
                    Attack();
                }

            }
            else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
            else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
        }

        animator.SetBool("isMoving", isMoving);

        if (noImage >= 0) {
            if (transform.localScale.Equals(scale)) {
                flip_before = rd.flipX;
            }
            transform.localScale = new Vector3(0.08f, 0.08f);
            noImage -= Time.deltaTime;

            if (noImage <= 0) {
                transform.localScale = scale;
                rd.flipX = flip_before;
            }
        }

        animator.SetBool("no_image", noImage > 0);

        if (isJumping) {

            if (rg.velocity.y <= -2) {
                isJumping = false;
            }
        }

        if (transform.localPosition.x < -5.5f) {
            transform.localPosition = new Vector2(-5.5f, transform.localPosition.y);
        } else if (transform.localPosition.x > 5.5f) {
            transform.localPosition = new Vector2(5.5f, transform.localPosition.y);
        }
    }

    public void HandSkill() {
        if (pv.IsMine) {
            if (isJumping) return;

            int facing = rd.flipX ? -1 : 1;

            var target = BattleSystem.GetTargetPlayer(this, 2.5f, new Vector2(transform.position.x + facing * 1f, transform.position.y + 2));
            Debug.Log(target);
            if (target != null) {
                target.Damage(12);
            }

            animator.SetTrigger("hand_skill");

            action = "hand_skill";
            sendAction = true;
        } else {
            animator.SetTrigger("hand_skill");
        }
    }

    public void Attack() {
        if (pv.IsMine) {
            int facing = rd.flipX ? -1 : 1;

            var target = BattleSystem.GetTargetPlayer(this, 3, new Vector2(transform.position.x + facing * 2, transform.position.y + 2));
            Debug.Log(target);
            if (target != null) {
                target.Damage(4);
            }

            animator.SetTrigger("attack");

            action = "attack";
            sendAction = true;
        } else {
            animator.SetTrigger("attack");
        }

        pv.RPC("AtkScale", RpcTarget.All);
    }

    [PunRPC]
    public void AtkScale() {
        StartCoroutine(atkScale());
    }

    IEnumerator atkScale() {
        stopAtk = true;
        transform.DOScale(new Vector3(3, transform.localScale.y), 0.2f);

        yield return new WaitForSeconds(0.2f);

        transform.DOScale(new Vector3(transform.localScale.y, transform.localScale.y), 0.1f);

        stopAtk = false;
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

    public void AddScore(int score) {
        object[] param = {
            score,
        };

        pv.RPC("addedScore", RpcTarget.All, param);
    }

    [PunRPC]
    void addedScore(int score_) {
        score += score_;
    }

    [PunRPC]
    void OnDamaged(int damage){
        health -= damage;

        if (health > 0) {
            GameManager.instance.hurt.Play();
        } else {
            GameManager.instance.death.Play();
        }
    }

    public void Jump() {
        if (isJumping) return;
        rg.velocity = new Vector2(rg.velocity.x, 10);

        isJumping = true;
        noImage = 0.3f;
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
                stream.SendNext(noImage);

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
                noImage = (float)stream.ReceiveNext();

                if (callAction) {
                    if (action == "hand_skill") {
                        HandSkill();
                    }
                    else if (action == "attack") {
                        Attack();
                    }
                }
            }
        
    }
}
