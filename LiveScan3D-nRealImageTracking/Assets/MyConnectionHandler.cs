using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Threading;

public class MyConnectionHandler : MonoBehaviour
{
    private float nextActionTime = 0.0f;

    public GameObject pointCloudRenderer;
    private float checkPeriod = 2f;
    private bool portOpen = false;
    private GameObject instance = null;
    [HideInInspector]
    public static bool instanceNull { get { return _instanceNull; } }
    private static bool _instanceNull = false;

    private string host = Constants.serverHostName;
    private int port = Constants.port;
    private TimeSpan timeout = TimeSpan.FromMilliseconds(100);
    private Thread threadPortChecker = null;

    //private GameObject TextSystemConnecting;

    private bool FlagShouldRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        threadPortChecker = new Thread(PortChecker);
        //TextSystemConnecting = GameObject.Find("/MyTextButtonSystemConnecting");
    }

    // Update is called once per frame
    void Update()
    {
        // https://answers.unity.com/questions/17131/execute-code-every-x-seconds-with-update.html
        if (Time.time > nextActionTime)
        {
            nextActionTime += checkPeriod;
            // execute block of code here
            if (instance == null)
            {
                if (FlagShouldRunning)
                {
                    portOpen = false;
                    FlagShouldRunning = false;
                }

                if (!threadPortChecker.IsAlive)
                {
                    threadPortChecker = new Thread(PortChecker);
                    threadPortChecker.Start();
                }

                if (portOpen)
                {
                    instance = Instantiate(pointCloudRenderer) as GameObject;
                    instance.transform.parent = gameObject.transform.parent;
                    instance.transform.position += new Vector3(0, 0, 0.6f);  // TODO temp fix: move 0.4m+0.1m+0.1m further
                    instance.SetActive(true);
                    Debug.Log("INST");
                    portOpen = false;
                    FlagShouldRunning = true;
                }
            }
        }

        // status update
        //if (instance != null)
        //{
        //    // hide self text
        //    if (TextSystemConnecting != null)
        //        TextSystemConnecting.SetActive(false);
        //}
        //else
        //{
        //    // show self text
        //    if (TextSystemConnecting != null)
        //        TextSystemConnecting.SetActive(true);
        //}
        _instanceNull = instance == null;
    }

    public void setPrefabActive(bool active)
    {
        Debug.Log("setactive" + active);
        if (!active)
        {
            if (instance != null)  // not destroyed yet
                Destroy(instance);
            Debug.Log("destroy instance");
            portOpen = false;
        }
    }

    void PortChecker()
    {
        portOpen = false;
        try
        {
            using (var client = new TcpClient())
            {
                var result = client.BeginConnect(host, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(timeout);
                client.EndConnect(result);
                Debug.Log("port open");
                portOpen = true;
            }
        }
        catch
        {
            Debug.Log("port not open");
            //return;
        }
    }
}
