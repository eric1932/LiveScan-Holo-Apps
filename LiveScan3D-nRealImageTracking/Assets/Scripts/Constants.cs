using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
#if UNITY_EDITOR
    public static string serverHostName = "127.0.0.1";
#else
    //public static string serverHostName = "139.180.141.82";
    public static string serverHostName = "127.0.0.1";
#endif

    public static int port = 48002;
}
