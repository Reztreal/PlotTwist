using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CreateMesh : MonoBehaviour
{
    public TMPro.TMP_Dropdown surfaceDropdown;

    public GameObject vertexPrefab;
    public List<GameObject> vertexRepresentations;

    public Material lineMaterial; // Material for the line renderers
    private List<LineRenderer> lineRenderers = new List<LineRenderer>(); // Store line renderers


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

    private enum SurfaceType
    { Sphere, Torus, MobiusStrip}
    private SurfaceType selectedSurfaceType = SurfaceType.Sphere;

    private void Start()
    {
        surfaceDropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(surfaceDropdown); });
        
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        CreateVertexRepresentations();


    }

    private void Update()
    {
        UpdateMesh();
    }

    private void DropdownValueChanged(TMP_Dropdown dropdown)
    {
        selectedSurfaceType = (SurfaceType)dropdown.value;
        ClearVertexRepresentations();

        Debug.Log(selectedSurfaceType.ToString());
        CreateShape();
        CreateVertexRepresentations();

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

        switch (selectedSurfaceType)
        {
            case SurfaceType.Sphere:
                u = new Vector2(0, 6.283f);
                v = new Vector2(0, 6.283f);
                CreateVertices(uStep, vStep, vertexCount, ParametricFunctions.ParametricSphere);
                break;
            case SurfaceType.Torus:
                u = new Vector2(0, 6.283f);
                v = new Vector2(0, 6.283f);
                CreateVertices(uStep, vStep, vertexCount, ParametricFunctions.Torus);
                break;
        }
        CreateTriangles();
    }

    /// <summary>
    /// Creates the vertex points of the surface and treats the vertices array like a 2d array.
    /// </summary>
    /// <param name="u_step"></param>
    /// <param name="v_step"></param>
    /// <param name="vCount"></param>
    private void CreateVertices(float u_step, float v_step, int vCount, System.Func<float, float, Vector3> surfaceFunction)
    {
        vertices = new Vector3[vCount];

        for (int i = 0; i < VResolution; i++)
        {
            for (int j = 0; j < UResolution; j++)
            {
                vertices[i * UResolution + j] = surfaceFunction(u.x + u_step * j, v.x + v_step * i);
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
    /// 
    /// This function creates the triangles from the vertex points on the surface.
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
    /// Unity uses clockwise winding so using the above quad formed from the vertices of the surface
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

    public void CreateVertexRepresentations()
    {
        if (vertexPrefab == null) return;

        foreach (Vector3 vertex in mesh.vertices)
        {
            GameObject vertexRep = Instantiate(vertexPrefab, transform.TransformPoint(vertex), Quaternion.identity, transform);
            vertexRepresentations.Add(vertexRep);
        }
    }

    public void ClearVertexRepresentations()
    {
        foreach (GameObject vertexRep in vertexRepresentations)
        {
            Destroy(vertexRep);
        }
        vertexRepresentations.Clear();
    }
}