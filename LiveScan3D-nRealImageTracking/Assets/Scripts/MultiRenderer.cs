using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MultiRenderer : MonoBehaviour
{
    public int maxChunkSize = 65535;
    // public int maxNumPoints = 50000;
    public float pointSize = 0.005f;
    public GameObject pointCloudElem;
    public Material pointCloudMaterial;
    private readonly List<List<GameObject>> elemsList = new List<List<GameObject>>();
    [HideInInspector]
    public static List<Pose> playerPoseList = new List<Pose>();

    public static MultiRenderer instance = null;

    int iterCount = 0;

    public static readonly bool[] flip = new bool[Constants.NumClients];
    private static readonly bool[] flop = new bool[Constants.NumClients];
    //private static readonly float[] lastUpdateTime = new float[NumClients];

    void Start()
    {
        instance = this;

        for (int i = 0; i < Constants.NumClients; i++)
        {
            elemsList.Add(new List<GameObject>());
            playerPoseList.Add(new Pose());
        }

        UpdatePointSize();
    }

    void Update()
    {
        if (transform.hasChanged)
        {
            UpdatePointSize();
            transform.hasChanged = false;
        }

        if (Time.smoothDeltaTime >= 0.033)  // 0.016 for 62.5 fps; 0.02 for 50; 0.25 for 40
        {
            //Debug.Log("Skip frame");
            return;
        }

        try
        {
            //if (lastUpdateTime[iterCount] + 8000 < Time.time)
            //{
            //    // destroy gameobj
            //    GameObject obj = GameObject.Find("/GameObject(Clone)/PointCloudRenderer1(Clone)");
            //    if (obj != null)
            //    {
            //        DestroyImmediate(obj);
            //        Debug.LogWarning("Destroy upon MultiRender timeout");
            //    }

            //    // update time
            //    lastUpdateTime[iterCount] = Time.time;

            //    // turn into finally
            //    return;
            //}

            // render ONLY ONE pointcloud each time
            if (Constants.Vertices.Count > iterCount && flip[iterCount] != flop[iterCount])
            {
                Render(Constants.Vertices[iterCount], Constants.Colors[iterCount], iterCount);

                // update flip-flop flag
                flop[iterCount] = !flop[iterCount];

                //lastUpdateTime[iterCount] = Time.time;
            }  // else save frame; do not update time
        }
        finally
        {
            iterCount += 1;
            if (iterCount >= Constants.NumClients)  // range within 0 1 2
                iterCount = 0;
        }
    }

    void UpdatePointSize()
    {
        pointCloudMaterial.SetFloat("_PointSize", pointSize * transform.localScale.x);
    }

    public void Render(float[] arrVertices, byte[] arrColors, int elemsIdx)
    {
        // NOTE: this call is SEVERELY COSTLY; do not use it
        //arrVertices = TransPoseVector(arrVertices, elemsIdx);  // transpose

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
        Pose targetPose = playerPoseList[Array.IndexOf(PositionManager.PositionData, elemsIdx)];
        for (int i = 0; i < nChunks; i++)
        {
            int nPointsToRender = System.Math.Min(maxChunkSize, nPoints - offset);

            ElemRenderer renderer = elemsList[elemsIdx][i].GetComponent<ElemRenderer>();  // TODO no indexof
            // update transform; replace TransPoseVector()
            if (renderer.transform.position != targetPose.position)
            {
                renderer.transform.position = targetPose.position;
                renderer.transform.rotation = targetPose.rotation * Quaternion.Euler(-180, 0, 0);  // TODO patch
            }
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

    public float[] TransPoseVector(float[] v, int multiID)
    {
        if (v == null)
            return null;

        float[] outVector = v;
        // TODO no indexof
        Pose targetPose = playerPoseList[Array.IndexOf(PositionManager.PositionData, multiID)];
        for (int i = 0; i < v.Length / 3; i++)
            for (int j = 0; j < 3; j++)
                outVector[i * 3 + j] += targetPose.position[j];
        return outVector;
    }

    //    public static void SwapTwoItems(int a, int b)
    //    {
    //        Transform transformA = playerTransformList[a];
    //        Transform transformB = playerTransformList[b];
    //        if (transformA != null && transformB != null)
    //            (playerTransformList[a], playerTransformList[b]) = (playerTransformList[b], playerTransformList[a]);
    //        else if (transformA == null && transformB == null)
    //            return;
    //        // fill data to prevent null
    //        else if (transformA == null)
    //            playerTransformList[a] = transformB;
    //        else
    //            playerTransformList[b] = transformA;
    //    }

    //    public static void SetTransformByIndexList(int[] indexList)
    //    {
    //        List<Transform> newVal = new List<Transform>();
    //        foreach (int i in indexList)
    //            newVal.Add(playerTransformList[i]);
    //        playerTransformList = newVal;
    //    }
}
