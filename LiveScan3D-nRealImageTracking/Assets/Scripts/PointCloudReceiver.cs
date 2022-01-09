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
    float[] vertices;
    byte[] colors;
    private Thread receiverThread = null;
    bool pendingRender = false;
    bool pendingDestroy = false;

    //public GameObject ConnectionHandlerPrefab;

    public int multiID = -1;
    private Vector3 position;
    private Vector3 localPosition;

    void Start()
    {
        pointCloudRenderer = GetComponent<PointCloudRenderer>();

        position = gameObject.transform.position;
        localPosition = gameObject.transform.localPosition;

        receiverThread = new Thread(ThreadReceiver);
        receiverThread.Start();
    }

    void Update()
    {
        if (!bConnected)
            return;

        if (NRInput.IsTouching()) return;  // If touching trackpad, do not render

        // ThreadReceiver keepalive (unused yet)
        //if (receiverThread == null || !receiverThread.IsAlive)
        //{
        //    if (receiverThread != null)
        //        receiverThread.Abort();
        //    receiverThread = new Thread(ThreadReceiver);
        //    receiverThread.Start();
        //}

        // a lot of code removed here
        // receive in thread

        if (pendingRender)
        {
            if (multiID == -1)  // TODO
                pointCloudRenderer.Render(vertices, colors);
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
        try
        {
#if WINDOWS_UWP
            socket = new NetworkCommunication.TransferSocket(IP, port);
#else
            socket = new TcpClient(IP, port);  // Eric1932: socket can also encounter errors; 

            // eric code
            // shorten socket timeout
            socket.ReceiveTimeout = 500;
#endif
            bConnected = true;
            //Debug.Log("Connected");
        }
        catch
        {
            // duplicated code
            switch (multiID)
            {
                case 1:
                    Constants.vert1 = new[] { 0f, 0f, 0f };
                    Constants.col1 = new[] { (byte)0, (byte)0, (byte)0 };
                    break;
                case 2:
                    Constants.vert2 = new[] { 0f, 0f, 0f };
                    Constants.col2 = new[] { (byte)0, (byte)0, (byte)0 };
                    break;
                case 3:
                    Constants.vert3 = new[] { 0f, 0f, 0f };
                    Constants.col3 = new[] { (byte)0, (byte)0, (byte)0 };
                    break;
                case 4:
                    Constants.vert4 = new[] { 0f, 0f, 0f };
                    Constants.col4 = new[] { (byte)0, (byte)0, (byte)0 };
                    break;
            }
            Destroy(gameObject);
        }
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

    bool ReceiveFrame(out float[] lVertices, out byte[] lColors)
    {
        // int nPointsToRead = ReadInt();
        byte[] buffer0 = new byte[4];
        socket.GetStream().Read(buffer0, 0, 4);
        int nPointsToRead = BitConverter.ToInt32(buffer0, 0);

        lVertices = new float[3 * nPointsToRead];
        short[] lShortVertices = new short[3 * nPointsToRead];
        lColors = new byte[3 * nPointsToRead];


        int nBytesToRead = sizeof(short) * 3 * nPointsToRead;
        int nBytesRead = 0;
        byte[] buffer = new byte[nBytesToRead];

        while (nBytesRead < nBytesToRead)
            nBytesRead += socket.GetStream().Read(buffer, nBytesRead, Math.Min(nBytesToRead - nBytesRead, 64000));

        System.Buffer.BlockCopy(buffer, 0, lShortVertices, 0, nBytesToRead);

        for (int i = 0; i < lShortVertices.Length; i++)
            lVertices[i] = lShortVertices[i] / 1000.0f;

        nBytesToRead = sizeof(byte) * 3 * nPointsToRead;
        nBytesRead = 0;
        buffer = new byte[nBytesToRead];

        while (nBytesRead < nBytesToRead)
            nBytesRead += socket.GetStream().Read(buffer, nBytesRead, Math.Min(nBytesToRead - nBytesRead, 64000));

        System.Buffer.BlockCopy(buffer, 0, lColors, 0, nBytesToRead);

        return true;
    }
#endif

    float[] TransPoseVector(float[] v)  // TODO
    {
        float[] outVector = v;
        for (int i = 0; i < v.Length / 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                outVector[i * 3 + j] += position[j] + localPosition[j];
            }
        }
        return outVector;
    }

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
                if (ReceiveFrame(out vertices, out colors))
#endif
                {
                    //Debug.Log("Frame received");
                    pendingRender = true;

                    if (multiID != -1)
                        vertices = TransPoseVector(vertices);
                    System.Threading.Thread.MemoryBarrier();
                    switch (multiID)
                    {
                        case 1:
                            Constants.vert1 = vertices;
                            Constants.col1 = colors;
                            //MultiRenderer.q.Enqueue(multiID);
                            break;
                        case 2:
                            Constants.vert2 = vertices;
                            Constants.col2 = colors;
                            //MultiRenderer.q.Enqueue(multiID);
                            break;
                        case 3:
                            Constants.vert3 = vertices;
                            Constants.col3 = colors;
                            //MultiRenderer.q.Enqueue(multiID);
                            break;
                        case 4:
                            Constants.vert4 = vertices;
                            Constants.col4 = colors;
                            //MultiRenderer.q.Enqueue(multiID);
                            break;
                    }
                    System.Threading.Thread.MemoryBarrier();
                }
            }
            catch (Exception e)
            {
                // Logging
                Debug.Log(String.Format("socket or else error; show error text; destroy self; port {0}; multiID {1}", port, multiID));
                Debug.Log(String.Format("Exception={0}", e));
                
                // custom error handler
                //ConnectionHandlerPrefab.GetComponent<MyConnectionHandler>().setPrefabActive(false);
                
                // in case of multi-targets
                // push empty array to flush display
                switch (multiID)
                {
                    case 1:
                        Constants.vert1 = new[] { 0f, 0f, 0f };
                        Constants.col1 = new[] { (byte) 0, (byte) 0, (byte) 0 };
                        break;
                    case 2:
                        Constants.vert2 = new[] { 0f, 0f, 0f };
                        Constants.col2 = new[] { (byte)0, (byte)0, (byte)0 };
                        break;
                    case 3:
                        Constants.vert3 = new[] { 0f, 0f, 0f };
                        Constants.col3 = new[] { (byte)0, (byte)0, (byte)0 };
                        break;
                    case 4:
                        Constants.vert4 = new[] { 0f, 0f, 0f };
                        Constants.col4 = new[] { (byte)0, (byte)0, (byte)0 };
                        break;
                }

                // destroy pointcloudrenderer
                pendingDestroy = true;
                break;  // kill self (thread)
                //return;
            }
        }
    }
}
