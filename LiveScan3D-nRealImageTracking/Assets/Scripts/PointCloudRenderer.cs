using Draco;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PointCloudRenderer : MonoBehaviour
{
    public int maxChunkSize = 65535;
    // public int maxNumPoints = 50000;
    public float pointSize = 0.005f;
    public GameObject pointCloudElem;
    public Material pointCloudMaterial;

    List<GameObject> elems;

    void Start()
    {
        elems = new List<GameObject>();
        UpdatePointSize();
    }

    void Update()
    {
        if (transform.hasChanged)
        {
            UpdatePointSize();
            transform.hasChanged = false;
        }
    }

    void UpdatePointSize()
    {
        pointCloudMaterial.SetFloat("_PointSize", pointSize * transform.localScale.x);
    }

    async public Task Render(byte[] dracoBytes)
    {
        var draco = new DracoMeshLoader();  // TODO optimize?
        var mesh = await draco.ConvertDracoMeshToUnity(dracoBytes);
        if (mesh != null)
        {
            // this line ensures mesh visible during zooming in
            mesh.bounds = new UnityEngine.Bounds(transform.position, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));

            GetComponent<MeshFilter>().sharedMesh = mesh;
        }
        Resources.UnloadUnusedAssets();  // free unused meshes; TODO occassionally call
    }

    void AddElems(int nElems)
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

            elems.Add(newElem);
        }            
    }

    void RemoveElems(int nElems)
    {
        for (int i = 0; i < nElems; i++)
        {
            Destroy(elems[0]);
            elems.Remove(elems[0]);
        }
    }
}
