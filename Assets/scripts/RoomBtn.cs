using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomBtn : MonoBehaviour
{
    Text text;
    public Button btn;
    private void Start() {
        text = GetComponent<Text>();
    }
    public void CallJoinRoom() {
        NetworkManager.instance.JoinCustomRoom(text.text);
    }

    public void Hover() {
        text.transform.localScale = new Vector2(0.32f, 0.32f);
    }
    public void UnHover() {
        text.transform.localScale = new Vector2(0.3f, 0.3f);
    }
}
