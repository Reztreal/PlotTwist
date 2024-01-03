using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParametricFunctions : MonoBehaviour
{
    public static Vector3 ParametricSphere(float u, float v)
    {
        float x = Mathf.Cos(u) * Mathf.Sin(v);
        float y = Mathf.Sin(u) * Mathf.Sin(v);
        float z = Mathf.Cos(v);

        return new Vector3(x, y, z);
    }

    public static Vector3 Torus(float u, float v)
    {
        float x = (0.6f + 0.2f * Mathf.Cos(v)) * Mathf.Cos(u);
        float y = (0.6f + 0.2f * Mathf.Cos(v)) * Mathf.Sin(u);
        float z = 0.2f * Mathf.Sin(v);

        return new Vector3(x, y, z);
    }
}
