using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CreateMesh : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;


    // U axis
    [Min(16)]
    public int UResolution = 16;

    // V axis
    [Min(16)]
    public int VResolution = 16;

    // x is min, y is max
    // u is for width
    // v is for height
    public Vector2 u;
    public Vector2 v;

    public bool doubleSided = true;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
    }

    private void Update()
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    private void CreateShape()
    {
        float uStep = (u.x - u.y) / (UResolution - 1);
        float vStep = (v.x - v.y) / (VResolution - 1);

        int vertexCount = doubleSided ? UResolution * VResolution * 2 : UResolution * VResolution;
        CreateVertices(uStep, vStep, vertexCount);
        CreateTriangles();
    }

    /// <summary>
    /// Create the vertex points of the surface and treat the vertices array like a 2d array.
    /// </summary>
    /// <param name="u_step"></param>
    /// <param name="v_step"></param>
    /// <param name="vCount"></param>
    private void CreateVertices(float u_step, float v_step, int vCount)
    {
        vertices = new Vector3[vCount];

        for (int i = 0; i < VResolution; i++)
        {
            for (int j = 0; j < UResolution; j++)
            {
                vertices[i * UResolution + j] = Torus(u.x + u_step * j, v.x + v_step * i);
            }
        }

        if (doubleSided)
        {
            CreateBackVertices();
        }

        mesh.vertices = vertices;
    }

    /// <summary>
    /// Creates the back vertices of the surface. Necessary if the surface is rendered double sided to create back triangles
    /// </summary>
    private void CreateBackVertices()
    {
        int length = vertices.Length / 2;
        for (int i = 0; i < length; i++)
        {
            vertices[length + i] = vertices[i];
        }
    }


    /// <summary>
    /// This function create the triangles from the vertex points of the surface.
    /// 
    ///    1          3
    ///     *--------*
    ///     | \      |
    ///     |   \    |
    ///     |     \  |
    ///     |       \|
    ///     *--------*
    ///    0         2
    ///    
    /// Unity uses clock wise winding so using the above quad formed from the vertices of the surface
    /// (0, 1, 2),
    /// (2, 1, 3)
    /// are the triangles that belong to this quad. If the surface is rendered double sided back triangles
    /// should be created as well
    /// 
    /// </summary>
    private void CreateTriangles()
    {
        int vertexCount = UResolution * VResolution;
        int triangleCount = (UResolution - 1) * (VResolution - 1) * 6;
        triangleCount = doubleSided ? triangleCount * 2: triangleCount;
        
        triangles = new int[triangleCount];

        for (int i = 0, vertIndex = 0, triIndex = 0; i < VResolution - 1; i++, vertIndex++)
        {
            for (int j = 0; j < UResolution - 1; j++, vertIndex++, triIndex += 6)
            {
                triangles[triIndex] = vertIndex;
                triangles[triIndex + 1] = vertIndex + UResolution;
                triangles[triIndex + 2] = vertIndex + 1;
                triangles[triIndex + 3] = vertIndex + 1;
                triangles[triIndex + 4] = vertIndex + UResolution;
                triangles[triIndex + 5] = vertIndex + UResolution + 1;
            }
        }

        if (doubleSided)
        {
            CreateBackTriangles();
        }

        mesh.triangles = triangles;
    }

    private void CreateBackTriangles()
    {
        int length = triangles.Length / 2;
        int vertLength = UResolution * VResolution;

        for (int i = 0; i < length; i += 6)
        {
            triangles[length + i] = triangles[i] + vertLength;
            triangles[length + i + 1] = triangles[length + i + 4] = triangles[i + 2] + vertLength;
            triangles[length + i + 2] = triangles[length + i + 3] = triangles[i + 1] + vertLength;
            triangles[length + i + 5] = triangles[i + 5] + vertLength;
        }
    }

    private Vector3 ParametricSphere(float u, float v)
    {
        float x = Mathf.Cos(u) * Mathf.Sin(v);
        float y = Mathf.Sin(u) * Mathf.Sin(v);
        float z = Mathf.Cos(v);

        return new Vector3(x, y, z);
    }

    private Vector3 Torus(float u, float v)
    {
        float x = (0.6f + 0.2f * Mathf.Cos(v)) * Mathf.Cos(u);
        float y = (0.6f + 0.2f * Mathf.Cos(v)) * Mathf.Sin(u);
        float z = 0.2f * Mathf.Sin(v);

        return new Vector3(x, y, z);
    }    
}