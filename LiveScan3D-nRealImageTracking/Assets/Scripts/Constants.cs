using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    /**
     * Client specific
     */
    public static readonly int NumClients = 3;  // DONT CHANGE THIS

    public static readonly int MyOnlineID = 0;  // For example, if NumClients=3, then this can range from [0...3]

    /**
     * IP (room) related settings
     */
    public static readonly string localhost = "127.0.0.1";
    public static readonly string HM_CHN_0001 = "45.77.176.88";
    public static readonly string LANTUF = "192.168.10.14";
    public static readonly string LANEsxi = "192.168.10.10";

    public static readonly string APIHostname = "http://45.77.176.88:8000";

    public static string serverHostName =
#if UNITY_EDITOR
        //localhost
        //LANEsxi
        HM_CHN_0001
#else
        HM_CHN_0001
        //LANEsxi
        //LANTUF
#endif
        ;

    /**
     * other variables & functions
     */
    public static readonly int DefaultPort = 48002;  // unused

    /**
     * Things you shouldn't touch
     */
    public static int ArrayCount = 0;  // TODO redundant
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
