using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;

public class MultiRenderer : MonoBehaviour
{
    public int maxChunkSize = 65535;
    // public int maxNumPoints = 50000;
    public float pointSize = 0.005f;
    public GameObject pointCloudElem;
    public Material pointCloudMaterial;

    readonly List<List<GameObject>> elemsList = new List<List<GameObject>>();

    int iterCount = 0;

    //public static ConcurrentQueue<int> q = new ConcurrentQueue<int>();

    void Start()
    {
        //elems = new List<GameObject>();
        for (int i = 0; i < 4; i++)
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

        //float fps = 1f / Time.smoothDeltaTime;
        //if (fps < 60)
        //    return;
        if (Time.smoothDeltaTime >= 0.025)  // 0.016 for 62.5 fps; 0.02 for 50; 0.25 for 40
            return;

        System.Threading.Thread.MemoryBarrier();
        switch (iterCount)
        {
            case 0:
                if (Constants.vert1 != null)
                    Render(Constants.vert1, Constants.col1, iterCount);
                break;
            case 1:
                if (Constants.vert2 != null)
                    Render(Constants.vert2, Constants.col2, iterCount);
                break;
            case 2:
                if (Constants.vert3 != null)
                    Render(Constants.vert3, Constants.col3, iterCount);
                break;
            case 3:
                if (Constants.vert4 != null)
                    Render(Constants.vert4, Constants.col4, iterCount);
                break;
        }
        System.Threading.Thread.MemoryBarrier();
        iterCount += 1;
        if (iterCount >= 4)
            iterCount = 0;
        //int idToRender;
        //if (q.TryDequeue(out idToRender))
        //{
        //    Debug.Log(string.Format("RenderQueue DQ: {0}; Q size: {1}", idToRender, q.Count));
        //    switch (idToRender)
        //    {
        //        case 1:
        //            if (Constants.vert1 != null)
        //                Render(Constants.vert1, Constants.col1, idToRender - 1);
        //            break;
        //        case 2:
        //            if (Constants.vert2 != null)
        //                Render(Constants.vert2, Constants.col2, idToRender - 1);
        //            break;
        //        case 3:
        //            if (Constants.vert3 != null)
        //                Render(Constants.vert3, Constants.col3, idToRender - 1);
        //            break;
        //        case 4:
        //            if (Constants.vert4 != null)
        //                Render(Constants.vert4, Constants.col4, idToRender - 1);
        //            break;
        //    }
        //} else
        //{
        //    Debug.Log("RenderQueue Empty!");
        //}
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
        //Debug.Assert(elems.Count == nChunks);
        //Debug.Log(nChunks);

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
