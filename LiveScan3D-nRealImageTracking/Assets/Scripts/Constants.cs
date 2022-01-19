using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static readonly string localhost = "127.0.0.1";
    public static readonly string VultrSG = "139.180.141.82";
    public static readonly string LANTUF = "192.168.10.14";
    public static readonly string LANEsxi = "192.168.10.10";

    public static readonly string APIHostname = "http://139.180.141.82:8000";

    public static string serverHostName =
#if UNITY_EDITOR
        localhost
        //LANEsxi
        //VultrSG
#else
        VultrSG
        //LANEsxi
        //LANTUF
#endif
        ;

    public static int DefaultPort = 48002;

    public static int NumClients = 3;

    public static int ArrayCount = 0;
    public static readonly List<float[]> Vertices = new List<float[]>();
    public static readonly List<byte[]> Colors = new List<byte[]>();
}
