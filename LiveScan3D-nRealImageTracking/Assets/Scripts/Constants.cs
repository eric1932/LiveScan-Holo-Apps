using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
#if UNITY_EDITOR
    public static string serverHostName = "127.0.0.1";
#else
    // Vultr
    public static string serverHostName = "139.180.141.82";
    // LAN
    //public static string serverHostName = "192.168.10.14";
#endif

    public static int port = 48002;

    public static float[] vert1 = { };
    public static float[] vert2 = { };
    public static float[] vert3 = { };
    public static float[] vert4 = { };

    public static byte[] col1 = { };
    public static byte[] col2 = { };
    public static byte[] col3 = { };
    public static byte[] col4 = { };
}
