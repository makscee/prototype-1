using System;
using UnityEditor;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    Camera _camera;

    void OnEnable()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        // if ((Input.touchSupported || EditorApplication.isPlaying) &&
        //     Application.platform != RuntimePlatform.WebGLPlayer)
        // {
        //     HandleTouch();
        // }
        // else
        // {
        //     HandleMouse();
        // }
        HandleTouch();
        HandleMouse();
    }

    void HandleMouse()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            ZoomOrthoCamera(_camera.ScreenToWorldPoint(Input.mousePosition), 1);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ZoomOrthoCamera(_camera.ScreenToWorldPoint(Input.mousePosition), -1);
        }
    }

    const float MinZoom = 4f;
    const float MaxZoom = 30f;

    void ZoomOrthoCamera(Vector3 zoomTowards, float amount)
    {
        Debug.Log("enter");
        if (_camera.orthographicSize < MinZoom && amount > 0 ||
            _camera.orthographicSize > MaxZoom && amount < 0) return;
        
        var orthographicSize = _camera.orthographicSize;
        var multiplier = (1.0f / orthographicSize * amount);
        transform.position += (zoomTowards - transform.position) * multiplier;
        _camera.orthographicSize = Mathf.Clamp(orthographicSize - amount, MinZoom, MaxZoom);
    }
    
    Vector3 lastDragPosition;

    void UpdateDrag()
    {
        if (!InputHandler.BlockClicked && (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
            lastDragPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        if (!InputHandler.BlockClicked && (Input.GetMouseButton(1) || Input.GetMouseButton(2)))
        {
            var delta = lastDragPosition - _camera.ScreenToWorldPoint(Input.mousePosition);
            transform.Translate(delta);
            lastDragPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    static readonly float PanSpeed = 20f;
    static readonly float ZoomSpeedTouch = 0.1f;
    static readonly float ZoomSpeedMouse = 0.5f;
    static readonly float[] ZoomBounds = new float[] {1f, 85f};
    Vector2 lastPanPosition;
    Vector3 startCameraPos;
    int panFingerId; // Touch mode only
    bool wasZoomingLastFrame; // Touch mode only
    Vector2[] lastZoomPositions; // Touch mode only

    void HandleTouch()
    {
        switch (Input.touchCount)
        {

            // case 1: // Panning
            //     wasZoomingLastFrame = false;
            //
            //     // If the touch began, capture its position and its finger ID.
            //     // Otherwise, if the finger ID of the touch doesn't match, skip it.
            //     var touch = Input.GetTouch(0);
            //     if (touch.phase == TouchPhase.Began)
            //     {
            //         lastPanPosition = touch.position;
            //         startCameraPos = transform.position;
            //         panFingerId = touch.fingerId;
            //     }
            //     else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved)
            //     {
            //         PanCamera(touch.position);
            //     }
            //
            //     break;

            case 2: // Zooming
                var newPositions = new Vector2[] {Input.GetTouch(0).position, Input.GetTouch(1).position};
                if (!wasZoomingLastFrame)
                {
                    lastZoomPositions = newPositions;
                    wasZoomingLastFrame = true;
                }
                else
                {
                    // Zoom based on the distance between the new positions compared to the 
                    // distance between the previous positions.
                    var newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                    var oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    var offset = newDistance - oldDistance;

                    // ZoomCamera(offset, ZoomSpeedTouch);
                    ZoomOrthoCamera(transform.position, offset / 80);

                    lastZoomPositions = newPositions;
                }

                break;

            default:
                wasZoomingLastFrame = false;
                break;
        }
    }

    void HandleMouse_new()
    {
        // On mouse down, capture it's position.
        // Otherwise, if the mouse is still down, pan the camera.
        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            PanCamera(Input.mousePosition);
        }

        // Check for scrolling to zoom the camera
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll, ZoomSpeedMouse);
    }

    void PanCamera(Vector2 newPanPosition)
    {
        Vector2 startPos = _camera.ScreenToWorldPoint(lastPanPosition);
        Vector2 endPos = _camera.ScreenToWorldPoint(newPanPosition);
        Vector3 offset = startPos - endPos;
        transform.position = startCameraPos + offset;
    }

    void ZoomCamera(float offset, float speed)
    {
        if (offset == 0)
        {
            return;
        }

        _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
    }
}