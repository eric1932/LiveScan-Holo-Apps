using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextSystemConnectingController : MonoBehaviour
{
    private float nextActionTime = 0f;
    private float checkPeriod = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += checkPeriod;
            gameObject.transform.GetChild(0).gameObject.SetActive(!MyConnectionHandler.AnyInstanceOnline());
        }
    }
}
