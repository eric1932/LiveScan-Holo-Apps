using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

#if WINDOWS_UWP
using NetworkCommunication;
#else
using System.Net.Sockets;
using System.Threading;
#endif

using NRKernal;


public class PointCloudReceiver : MonoBehaviour
{
#if WINDOWS_UWP
    TransferSocket socket;
#else
    TcpClient socket = null;
#endif
    public int port = 48002;

    PointCloudRenderer pointCloudRenderer;
    bool bReadyForNextFrame = true;
    bool bConnected = false;

    // private System.DateTime time = System.DateTime.Now;

    // threaded receiver
    byte[] dracoBytes;
    private Thread receiverThread = null;
    bool pendingRender = false;
    bool pendingDestroy = false;

    public GameObject ConnectionHandlerPrefab;

    void Start()
    {
        pointCloudRenderer = GetComponent<PointCloudRenderer>();

        receiverThread = new Thread(ThreadReceiver);
        receiverThread.Start();
    }

    async void Update()
    {
        if (!bConnected)
            return;

        if (NRInput.IsTouching()) return;  // If touching trackpad, do not render

        // a lot of code removed here
        // receive in thread

        if (pendingRender)
        {
            await pointCloudRenderer.Render(dracoBytes);
            bReadyForNextFrame = true;
            pendingRender = false;
        }

        if (pendingDestroy)
        {
            Destroy(gameObject);
        }
    }

    public void Connect(string IP)
    {
#if WINDOWS_UWP
        socket = new NetworkCommunication.TransferSocket(IP, port);
#else
        socket = new TcpClient(IP, port);

        // eric code
        // shorten socket timeout
        socket.ReceiveTimeout = 500;
#endif
        bConnected = true;
        //Debug.Log("Connected");
    }

    //Frame receiving for the editor
#if WINDOWS_UWP
#else
    void RequestFrame()
    {
        byte[] byteToSend = new byte[1];
        byteToSend[0] = 0;

        socket.GetStream().Write(byteToSend, 0, 1);
    }

    int ReadInt()
    {
        byte[] buffer = new byte[4];
        int nRead = 0;
        while (nRead < 4)
            nRead += socket.GetStream().Read(buffer, nRead, 4 - nRead);

        return BitConverter.ToInt32(buffer, 0);
    }

    bool ReceiveFrame(out byte[] dracoBytes)
    {
        // int nPointsToRead = ReadInt();
        byte[] buffer0 = new byte[4];
        socket.GetStream().Read(buffer0, 0, 4);
        int nBytesToReceive = BitConverter.ToInt32(buffer0, 0);
        int nBytesRead = 0;

        dracoBytes = new byte[nBytesToReceive];

        while (nBytesRead < nBytesToReceive)
            nBytesRead += socket.GetStream().Read(dracoBytes, nBytesRead, Math.Min(nBytesToReceive - nBytesRead, 64000));

        return true;
    }
#endif

    void ThreadReceiver()
    {
        while (true)
        {
            if (socket == null || pendingRender)  // if socket not connected || pending render -> do not receive
                continue;

            // eric code
            try
            {
                if (bReadyForNextFrame)  // TODO = pendingRender??
                {
                    //Debug.Log("Requesting frame");
                    // TimeSpan ts = System.DateTime.Now.Subtract(time);
                    // if (ts.Seconds < 0.2) {
                    //     return;
                    // }

#if WINDOWS_UWP
                    socket.RequestFrame();
                    socket.ReceiveFrameAsync();
#else
                    RequestFrame();
#endif
                    bReadyForNextFrame = false;
                }

#if WINDOWS_UWP
                if (socket.GetFrame(out vertices, out colors))
#else
                if (ReceiveFrame(out dracoBytes))
#endif
                {
                    //Debug.Log("Frame received");
                    pendingRender = true;
                }
            }
            catch (Exception e)
            {
                Debug.Log("socket or else error; show error text; destroy self");
                // custom error handler
                //ConnectionHandlerPrefab.GetComponent<MyConnectionHandler>().setPrefabActive(false);
                // destroy pointcloudrenderer
                pendingDestroy = true;

                Thread.Sleep(2000);

                Debug.LogError(e);
            }
        }
    }
}
