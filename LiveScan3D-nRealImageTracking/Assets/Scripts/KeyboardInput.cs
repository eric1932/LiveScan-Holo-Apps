using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StringEvent : UnityEvent<string> { }

public class KeyboardInput : MonoBehaviour {
    public StringEvent keyboardDone;
    public string titleText;
    TouchScreenKeyboard keyboard;

    void Start ()
    {
        // //#if WINDOWS_UWP
        // #if !UNITY_EDITOR
        //     keyboard = TouchScreenKeyboard.Open("139.180.141.82",  // text
        //                                         TouchScreenKeyboardType.Default,  // keyboardType
        //                                         false,  // autocorrection
        //                                         false,  // multiline
        //                                         false,  // secure
        //                                         false,  // alert
        //                                         titleText  // textPlaceholder
        //                                         );
        // #else
        //     //Just for testing in the editor
        //     // if (keyboardDone != null)
        //         keyboardDone.Invoke("127.0.0.1");
        // #endif


        //#if UNITY_EDITOR
        //            if (keyboardDone != null)
        //                keyboardDone.Invoke("127.0.0.1");
        //#else
        //            if (keyboardDone != null)
        //                // ASUS Router
        //                // keyboardDone.Invoke("192.168.1.81");  // MacBook
        //                // keyboardDone.Invoke("192.168.1.77");  // Lenovo
        //                // Huawei Router
        //                // keyboardDone.Invoke("192.168.3.3");  // Lenovo
        //                // Vultr
        //                keyboardDone.Invoke("139.180.141.82");
        //#endif
        keyboardDone.Invoke("127.0.0.1");
    }

    // void Update ()
    // {
    //     if (TouchScreenKeyboard.visible == false && keyboard != null)
    //     {
    //        if (keyboard.done == true)
    //        {
    //            keyboardDone.Invoke(keyboard.text);
    //            keyboard = null;
    //        }
    //     }
    // }
}
