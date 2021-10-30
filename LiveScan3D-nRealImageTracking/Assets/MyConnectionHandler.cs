using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyConnectionHandler : MonoBehaviour
{
    private float nextActionTime = 0.0f;
    public float period = 2f;
    public GameObject pointCloudRenderer;
    private bool connected = false;
    private GameObject instance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // https://answers.unity.com/questions/17131/execute-code-every-x-seconds-with-update.html
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            // execute block of code here
            if (!connected)
            {
                instance = Instantiate(pointCloudRenderer) as GameObject;
                instance.transform.parent = gameObject.transform.parent;
                instance.SetActive(true);
                Debug.Log("inst instance");
                connected = true;
            }
        }

        if (connected)
        {
            // hide self text
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            // show self text
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void setPrefabActive(bool active)
    {
        Debug.Log("setactive" + active);
        connected = active;
        if (!connected)
        {
            Destroy(instance);
            Debug.Log("destroy instance");
        }
    }
}
