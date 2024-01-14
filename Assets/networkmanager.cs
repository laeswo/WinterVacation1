using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.UI;

public class networkmanager : MonoBehaviourPunCallbacks
{
    public Text hp1;
    public Text hp2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void connct() =>   PhotonNetwork.ConnectUsingSettings();
}
