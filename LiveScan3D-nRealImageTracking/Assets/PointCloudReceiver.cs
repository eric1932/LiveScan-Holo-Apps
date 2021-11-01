﻿using UnityEngine;
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
    TcpClient socket;
#endif
    public int port = 48002;

    PointCloudRenderer pointCloudRenderer;
    bool bReadyForNextFrame = true;
    bool bConnected = false;

    // private System.DateTime time = System.DateTime.Now;

    void Start()
    {
        pointCloudRenderer = GetComponent<PointCloudRenderer>();
    }

    void Update()
    {
        if (!bConnected)
            return;

        float[] vertices;
        byte[] colors;

        // eric code
        if (NRInput.IsTouching()) return;  // If touching trackpad, do not render

        if (bReadyForNextFrame)
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
            pointCloudRenderer.Render(vertices, colors);
            bReadyForNextFrame = true;
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
        //socket.ReceiveTimeout = 250;
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
}
