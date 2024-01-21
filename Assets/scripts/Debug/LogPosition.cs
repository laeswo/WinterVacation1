using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogPosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Position of GameObject " + gameObject.name + ": ");
        Debug.Log(transform.position);
    }
}
