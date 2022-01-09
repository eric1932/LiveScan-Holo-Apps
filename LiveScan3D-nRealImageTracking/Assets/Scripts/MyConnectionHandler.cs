using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Threading;

public class MyConnectionHandler : MonoBehaviour
{
    private static int InstanceCount = 0;
    private static bool[] targetNotNull = null;
    private int instanceID;

    private float nextActionTime = 0f;
    private float nextSmallActionTime = 0f;

    public GameObject pointCloudRenderer;
    private float checkPeriod = 2f;
    private float smallCheckPeriod = 0.5f;
    private GameObject instance = null;

    public int port = Constants.DefaultPort;

    //private TimeSpan timeout = TimeSpan.FromMilliseconds(100);

    //private GameObject TextSystemConnecting;

    // Start is called before the first frame update
    void Start()
    {
        //TextSystemConnecting = GameObject.Find("/MyTextButtonSystemConnecting");

        if (gameObject.transform.parent.transform.parent == null)
        {
            // temp fix: MyVisualizer will initialize this class before it shows up in the main view;
            // to exclude this situation, just ensure that this object is under the main scene.
            instanceID = InstanceCount++;

            if (targetNotNull == null || targetNotNull.Length != InstanceCount)  // update the size
                targetNotNull = new bool[InstanceCount];
        }
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
                if (PortChecker.GetStatusByPort(port))
                {
                    instance = Instantiate(pointCloudRenderer) as GameObject;
                    instance.transform.parent = gameObject.transform.parent;
                    instance.transform.position += new Vector3(0, 0, 0.6f);  // TODO temp fix: move 0.4m+0.1m+0.1m further
                    instance.SetActive(true);
                    Debug.Log("INST");
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
        
        if (Time.time > nextSmallActionTime)
        {
            nextSmallActionTime += smallCheckPeriod;
            targetNotNull[instanceID] = instance != null;
        }
    }

    public void setPrefabActive(bool active)
    {
        Debug.Log("setactive" + active);
        if (!active)
        {
            if (instance != null)  // not destroyed yet
            {
                Destroy(instance);
                instance = null;
            }
            Debug.Log("destroy instance");
        }
    }

    public static bool AnyInstanceOnline()
    {
        if (targetNotNull == null)
            return false;
        else
        {
            foreach (bool x in targetNotNull)
                if (x)
                    return true;
            return false;
        }
    }

    //void PortChecker()
    //{
    //    portOpen = false;
    //    try
    //    {
    //        using (var client = new TcpClient())
    //        {
    //            var result = client.BeginConnect(host, port, null, null);
    //            var success = result.AsyncWaitHandle.WaitOne(timeout);
    //            client.EndConnect(result);
    //            Debug.Log("port open");
    //            portOpen = true;
    //        }
    //    }
    //    catch
    //    {
    //        Debug.Log("port not open");
    //        //return;
    //    }
    //}
}
