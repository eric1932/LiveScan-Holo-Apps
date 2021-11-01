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
    private bool connected = false;
    private bool portOpen = false;
    private GameObject instance;

    private string host = Constants.serverHostName;
    private int port = Constants.port;
    private TimeSpan timeout = TimeSpan.FromMilliseconds(100);
    private Thread threadPortChecker = null;

    // Start is called before the first frame update
    void Start()
    {
        threadPortChecker = new Thread(PortChecker);
    }

    // Update is called once per frame
    void Update()
    {
        // https://answers.unity.com/questions/17131/execute-code-every-x-seconds-with-update.html
        if (Time.time > nextActionTime)
        {
            nextActionTime += checkPeriod;
            // execute block of code here
            if (!connected)
            {
                if (!threadPortChecker.IsAlive)
                {
                    threadPortChecker = new Thread(PortChecker);
                    threadPortChecker.Start();
                }
                if (portOpen)
                {
                    instance = Instantiate(pointCloudRenderer) as GameObject;
                    instance.transform.parent = gameObject.transform.parent;
                    instance.transform.position += new Vector3(0, 0, 0.4f);  // TODO temp fix: move 0.4m further
                    instance.SetActive(true);
                    connected = true;
                }
            }
        }

        if (connected)
        {
            // hide self text
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            // show self text
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void setPrefabActive(bool active)
    {
        Debug.Log("setactive" + active);
        connected = active;
        if (!connected)
        {
            if (instance != null)  // not destroyed yet
                Destroy(instance);
            Debug.Log("destroy instance");
            portOpen = false;
        }
    }

    void PortChecker()
    {
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
            portOpen = false;
            //return;
        }
    }
}
