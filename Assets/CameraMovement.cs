using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    
    public float minCamSize = 3f, maxCamSize = 35f; // Min and max orthographic size

    public SpriteRenderer mapRenderer;

    [HideInInspector]public float mapMinX, mapMaxX, mapMinY, mapMaxY;

    // Variables for smooth zooming
    [HideInInspector]public float targetCamSize; // The target size for smooth zoom

    [SerializeField, Range(0.01f, 1f)]
    private float zoomSpeed = 0.2f; // How fast the camera zooms, editable in the editor with a slider

    private float smoothZoomVelocity; // SmoothDamp velocity reference for smooth zoom

    private Vector3 dragOrigin;
    public bool canMove;
    public bool canZoom;
    private void Awake()
    {
        // Calculate map bounds based on the map renderer
        CalculateBounds();
        // Initialize the target size to the current camera size
        targetCamSize = cam.orthographicSize;

    }

    public void CalculateBounds()
    {
        mapMinX = mapRenderer.transform.position.x - mapRenderer.bounds.size.x / 2f;
        mapMaxX = mapRenderer.transform.position.x + mapRenderer.bounds.size.x / 2f;

        mapMinY = mapRenderer.transform.position.y - mapRenderer.bounds.size.y / 2f;
        mapMaxY = mapRenderer.transform.position.y + mapRenderer.bounds.size.y / 2f;
    }

    void Update()
    {
        if (canMove)
        {
            PanCamera();   
        }
        if (canZoom)
        {
            HandleZoom();   
        }
    }

    private void HandleZoom()
    {
        // Zoom in or out based on scroll input
        if (Input.mouseScrollDelta.y > 0)
        {
            targetCamSize -= 1; // Zoom in
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            targetCamSize += 1; // Zoom out
        }

        // Clamp the target camera size to be within allowed range
        targetCamSize = Mathf.Clamp(targetCamSize, minCamSize, maxCamSize);

        // Get the world position of the mouse before the zoom
        Vector3 mouseWorldPositionBeforeZoom = cam.ScreenToWorldPoint(Input.mousePosition);

        // Smoothly transition to the target size
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetCamSize, ref smoothZoomVelocity, zoomSpeed);

        // Get the world position of the mouse after the zoom
        Vector3 mouseWorldPositionAfterZoom = cam.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the difference between the mouse world positions before and after zoom
        Vector3 difference = mouseWorldPositionBeforeZoom - mouseWorldPositionAfterZoom;

        // Adjust the camera's position to maintain the world position under the cursor
        cam.transform.position = ClampCamera(cam.transform.position + difference);
    }

    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            cam.transform.position = ClampCamera(cam.transform.position + difference);
        }
    }

    public Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }
}
