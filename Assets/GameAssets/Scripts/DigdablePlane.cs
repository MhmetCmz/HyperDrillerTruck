using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DigdablePlane : MonoBehaviour
{
    [SerializeField]
    float power;  
    MeshFilter meshFilter;
    Mesh clayMesh;

    Vector3[] verts,originalVertices; 

    public bool isDigging;


    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        clayMesh = meshFilter.mesh;
        originalVertices = clayMesh.vertices;
        verts = clayMesh.vertices; 
    } 

    public void AddDepression(Vector3 depressionPoint, Vector3 radius)
    {
        depressionPoint = transform.InverseTransformPoint(depressionPoint);
        for (int i = 0; i < verts.Length; ++i)
        { 
            var distance = (verts[i] - depressionPoint).magnitude;
            if (distance < radius.magnitude)
            {
                var newVert = originalVertices[i] + Vector3.back * power;
                verts[i] = newVert;
            }
        }

        clayMesh.SetVertices(verts);
    } 
}
