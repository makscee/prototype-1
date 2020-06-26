using System;
using UnityEditor;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    Camera _camera;
    
    [SerializeField]Painter painter;

    void OnEnable()
    {
        _camera = GetComponent<Camera>();
        painter.palette = new Palette();
        for (var i = 0; i < 4; i++)
        {
            painter.palette.Colors[i] = Color.white;
        }
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

    const float MinZoom = 1f;
    const float MaxZoom = 30f;

    void ZoomOrthoCamera(Vector3 zoomTowards, float amount)
    {
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
    static readonly float[] ZoomBounds = new float[] {0.1f, 85f};
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
}