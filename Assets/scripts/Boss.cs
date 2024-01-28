using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

public class Boss : MonoBehaviour
{
    Animator animator;
    bool angry = false;

    float atkCool = 0;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.round >= 2) {
            angry = true;
        }

        animator.SetBool("angry", angry);

        if (angry && GameManager.instance.doAction) {
            atkCool += Time.deltaTime;

            transform.position = new Vector3(0, transform.position.y);

            if (atkCool >= 6 - GameManager.instance.round) {
                atkCool = 0;

                if (PhotonNetwork.IsMasterClient) PhotonNetwork.Instantiate("boss_atk", new Vector3(0, 1), quaternion.identity);
            }
        }
    }
}
