using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;

public class PortChecker : MonoBehaviour
{
    public int[] PortChecklist = { 48002, 48004, 48006, 48008 };
    public bool[] PortStatus { get; private set; } = null;
    private Thread[] CheckerThreadList = null;

    // Start is called before the first frame update
    void Start()
    {
        PortStatus = new bool[PortChecklist.Length];
        CheckerThreadList = new Thread[PortChecklist.Length];
        for (int i = 0; i < PortChecklist.Length; ++i)
        {
            int closureJ = i;
            Thread t = new Thread(() => ThreadWrapper(closureJ, Constants.serverHostName, PortChecklist[closureJ]));
            CheckerThreadList[i] = t;
            t.Start();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(string.Format("test check: {0}", CheckPort(Constants.serverHostName, Constants.port)));
        //Debug.Log(string.Join(" ", PortStatus));
    }

    void ThreadWrapper(int idx, string host, int port)
    {
        bool result;
        while (true)
        {
            result = CheckPort(host, port);
            lock (PortStatus)
            {
                PortStatus[idx] = result;
            }

            Thread.Sleep(2000);  // thread sleep for 2 secs
        }
    }

    bool CheckPort(string host, int port)
    {
        // https://stackoverflow.com/a/63840590/8448191
        TcpClient client = null;
        try
        {
            client = new TcpClient();
            if (client.ConnectAsync(host, port).Wait(500))
                if (client.Connected)
                    return true;  // reachable
                else
                    return false;  // refused
            else
                return false;  // timeout
        }
        catch
        {
            return false;  // connection failed
        }
        finally
        {
            if (client != null)
                client.Close();
        }
    }
}
