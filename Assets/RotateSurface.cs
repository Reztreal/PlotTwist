using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSurface : MonoBehaviour
{
    public float rotateSpeed = 200f; // Adjust the rotation speed as needed

    private bool isRotating = false;

    private void Update()
    {
        // Check if the left mouse button is just pressed
        if (Input.GetMouseButtonDown(0))
        {
            StartRotation();
        }

        // Rotate while holding down the left mouse button
        if (isRotating)
        {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, -mouseX, Space.World);
            transform.Rotate(Vector3.right, mouseY, Space.World);
        }

        // Check if the left mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            EndRotation();
        }
    }

    private void StartRotation()
    {
        isRotating = true;
        GetComponent<CreateMesh>().ClearVertexRepresentations();

    }

    private void EndRotation()
    {
        isRotating = false;
        GetComponent<CreateMesh>().CreateVertexRepresentations();

    }
}

