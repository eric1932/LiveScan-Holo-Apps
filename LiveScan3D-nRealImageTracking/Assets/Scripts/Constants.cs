using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static readonly string localhost = "127.0.0.1";
    public static readonly string VultrSG_HM0001 = "139.180.141.82";
    public static readonly string LANTUF = "192.168.10.14";
    public static readonly string LANEsxi = "192.168.10.10";

    public static readonly string APIHostname = "http://139.180.141.82:8000";

    public static string serverHostName =
#if UNITY_EDITOR
        localhost
        //LANEsxi
        //VultrSG_HM0001
#else
        VultrSG_HM0001
        //LANEsxi
        //LANTUF
#endif
        ;

    public static readonly int DefaultPort = 48002;  // unused

    public static readonly int NumClients = 3;
    public static readonly int MyOnlineID = 0;  // TODO change this

    public static int ArrayCount = 0;
    public static readonly List<float[]> Vertices = new List<float[]>();
    public static readonly List<byte[]> Colors = new List<byte[]>();

    public static int[] GetPortList()
    {
        return GetPortListByOnlineID(MyOnlineID);
    }

    public static int[] GetPortListByOnlineID(int onlineID)
    {
        return onlineID switch
        {
            0 => new int[] { 48004, 48006, 48008 },
            1 => new int[] { 48002, 48006, 48008 },
            2 => new int[] { 48002, 48004, 48008 },
            3 => new int[] { 48002, 48004, 48006 },
            _ => throw new System.Exception(),
        };
    }

    public static int GetPortByMultiIDAndOnlineID(int multiID, int onlineID)
    {
        return GetPortListByOnlineID(onlineID)[multiID];
    }

    public static int GetPortByMultiID(int multiID)
    {
        return GetPortList()[multiID];
    }
}
