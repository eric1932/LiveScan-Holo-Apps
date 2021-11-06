using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class MyControllerListener : MonoBehaviour
{
    public GameObject exitWindow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (NRInput.GetButton(ControllerButton.HOME) || NRInput.GetButton(ControllerButton.APP)
            || NRInput.GetButtonDown(ControllerButton.APP))  // enhanced detection
        {
            exitWindow.SetActive(true);
        }
    }
}
