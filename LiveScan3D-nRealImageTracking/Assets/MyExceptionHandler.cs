using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyExceptionHandler : MonoBehaviour
{

    public GameObject ErrorTextPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        Application.logMessageReceived += HandleException;
        DontDestroyOnLoad(gameObject);
    }

    void HandleException(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            //handle here

            //gameObject.SetActive(false);  // disable self
            //Instantiate(ErrorTextPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            //Destroy(gameObject);  // destroy self

            // new method
            ErrorTextPrefab.GetComponent<MyConnectionHandler>().setPrefabActive(false);
        }
    }
}
