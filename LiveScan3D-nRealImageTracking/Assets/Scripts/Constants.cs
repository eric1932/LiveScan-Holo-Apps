using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static readonly string localhost = "127.0.0.1";
    public static readonly string VultrSG = "139.180.141.82";
    public static readonly string LANTUF = "192.168.10.14";
    public static readonly string LANEsxi = "192.168.10.10";

    public static string serverHostName =
#if UNITY_EDITOR
        //localhost
        LANEsxi
#else
        VultrSG
#endif
        ;

    public static int DefaultPort = 48002;

    public static float[] vert1 = { };
    public static float[] vert2 = { };
    public static float[] vert3 = { };
    public static float[] vert4 = { };

    public static byte[] col1 = { };
    public static byte[] col2 = { };
    public static byte[] col3 = { };
    public static byte[] col4 = { };
}
