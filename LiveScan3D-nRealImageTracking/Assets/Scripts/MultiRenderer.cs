using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiRenderer : MonoBehaviour
{
    public int maxChunkSize = 65535;
    // public int maxNumPoints = 50000;
    public float pointSize = 0.005f;
    public GameObject pointCloudElem;
    public Material pointCloudMaterial;

    readonly List<List<GameObject>> elemsList = new List<List<GameObject>>();

    int iterCount = 0;
    public static readonly int NumClients = 3;

    public static readonly bool[] flip = new bool[NumClients];
    private static readonly bool[] flop = new bool[NumClients];

    void Start()
    {
        for (int i = 0; i < NumClients; i++)
            elemsList.Add(new List<GameObject>());

        UpdatePointSize();
    }

    void Update()
    {
        if (transform.hasChanged)
        {
            UpdatePointSize();
            transform.hasChanged = false;
        }

        if (Time.smoothDeltaTime >= 0.04)  // 0.016 for 62.5 fps; 0.02 for 50; 0.25 for 40
            return;

        // render ONLY ONE pointcloud each time
        if (Constants.Vertices.Count > iterCount && flip[iterCount] != flop[iterCount])
        {
            //Debug.Log("render");
            Render(Constants.Vertices[iterCount], Constants.Colors[iterCount], iterCount);
            flop[iterCount] = !flop[iterCount];
        }
        //else if (flip[iterCount] == flop[iterCount])
        //{
        //    Debug.Log("save");
        //}

        iterCount += 1;
        if (iterCount >= NumClients)  // range within 0 1 2
            iterCount = 0;
    }

    void UpdatePointSize()
    {
        pointCloudMaterial.SetFloat("_PointSize", pointSize * transform.localScale.x);
    }

    public void Render(float[] arrVertices, byte[] arrColors, int elemsIdx)
    {
        int nPoints, nChunks;
        if (arrVertices == null || arrColors == null)
        {
            nPoints = 0;
            nChunks = 0;
        }
        else
        {
            nPoints = arrVertices.Length / 3;
            nChunks = 1 + nPoints / maxChunkSize;
        }

        // makes elems has Count=nChunks
        if (elemsList[elemsIdx].Count < nChunks)
            AddElems(nChunks - elemsList[elemsIdx].Count, elemsIdx);
        if (elemsList[elemsIdx].Count > nChunks)
            RemoveElems(elemsList[elemsIdx].Count - nChunks, elemsIdx);

        int offset = 0;
        for (int i = 0; i < nChunks; i++)
        {
            int nPointsToRender = System.Math.Min(maxChunkSize, nPoints - offset);

            ElemRenderer renderer = elemsList[elemsIdx][i].GetComponent<ElemRenderer>();
            renderer.UpdateMesh(arrVertices, arrColors, nPointsToRender, offset);

            offset += nPointsToRender;

            // if (offset >= maxNumPoints) break;
        }
        //Debug.Log(offset);
    }

    void AddElems(int nElems, int elemsIdx)
    {
        for (int i = 0; i < nElems; i++)
        {
            GameObject newElem = GameObject.Instantiate(pointCloudElem);
            newElem.transform.parent = transform;
            // #if UNITY_EDITOR
            newElem.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            // #else
            //             newElem.transform.localPosition = new Vector3(0.0f, 0.0f, 0.2f);
            // #endif
            //newElem.transform.localRotation = Quaternion.identity;
            newElem.transform.localRotation = Quaternion.Euler(0, 180, 180);  // Fix rotation
            newElem.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            elemsList[elemsIdx].Add(newElem);
        }
    }

    void RemoveElems(int nElems, int elemsIdx)
    {
        for (int i = 0; i < nElems; i++)
        {
            Destroy(elemsList[elemsIdx][0]);
            elemsList[elemsIdx].Remove(elemsList[elemsIdx][0]);
        }
    }
}
