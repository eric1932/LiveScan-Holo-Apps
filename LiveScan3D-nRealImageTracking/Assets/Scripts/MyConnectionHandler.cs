using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.SceneManagement;

public class MyConnectionHandler : MonoBehaviour
{
    private static int InstanceCount = 0;
    private static List<bool> targetNotNull = new List<bool>();
    private int instanceID = -1;

    private float nextActionTime = 0f;
    private float nextSmallActionTime = 0f;

    public GameObject pointCloudRenderer;
    private float checkPeriod = 2f;
    private float smallCheckPeriod = 0.5f;
    private GameObject instance = null;

    private int port = -1;
    public int multiID = -1;

    //private TimeSpan timeout = TimeSpan.FromMilliseconds(100);

    //private GameObject TextSystemConnecting;

    // Start is called before the first frame update
    void Start()
    {
        port = Constants.GetPortByMultiID(multiID);  // TODO single mode cannot run

        //TextSystemConnecting = GameObject.Find("/MyTextButtonSystemConnecting");

        if (gameObject.transform.parent.transform.parent == null  // differ Prefab-as-param & cloned instance
            || SceneManager.GetActiveScene().name == "scene_not_nReal")  // for test scene
        {
            // temp fix: MyVisualizer will initialize this class before it shows up in the main view;
            // to exclude this situation, just ensure that this object is under the main scene.
            instanceID = InstanceCount++;

            // update the size
            targetNotNull.Add(false);
            Constants.Vertices.Add(null);
            Constants.Colors.Add(null);
            ++Constants.ArrayCount;

            Debug.Log("true MCH object instantiated");
        } else
        {
            Debug.Log("fake MCH object instantiated");
        }


        // save position to multirenderer
        Pose thisPose = new Pose();
        thisPose.position = pointCloudRenderer.transform.position;  // + transform.localPosition;  // adding this will not get the right outcome
        thisPose.rotation = pointCloudRenderer.transform.rotation;
        MultiRenderer.playerPoseList[multiID] = thisPose;
        Debug.Log(string.Format("Position for MultID {0} saved: position {1}, rotation {2}", multiID, thisPose.position, thisPose.rotation));
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
                    //instance.transform.position += new Vector3(0, 0, 0.6f);  // TODO temp fix: move 0.4m+0.1m+0.1m further
                    instance.SetActive(true);
                    Debug.Log("INST");

                    // recycle previously used meshes
                    Resources.UnloadUnusedAssets();
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
            if (instanceID != -1)
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

    public static bool AllInstancesOffline()
    {
        if (targetNotNull == null)
            return false;
        else if (targetNotNull.Count == 0)
            return false;
        else
        {
            foreach (bool x in targetNotNull)
                if (x)
                    return false;
            return true;
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
