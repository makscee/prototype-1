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
            ZoomOrthoCamera(_camera.ScreenToWorldPoint(Input.mousePosition), 0.2f);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ZoomOrthoCamera(_camera.ScreenToWorldPoint(Input.mousePosition), -0.2f);
        }
    }

    const float MinZoom = 1f;
    const float MaxZoom = 30f;

    void ZoomOrthoCamera(Vector3 zoomTowards, float amount)
    {
        var orthographicSize = _camera.orthographicSize;
        var multiplier = (1.0f / orthographicSize * amount);
        transform.position += (zoomTowards - transform.position) * multiplier;
        _camera.orthographicSize = Mathf.Clamp(orthographicSize - amount, MinZoom, MaxZoom);
    }

    static readonly float PanSpeed = 20f;
    static readonly float ZoomSpeedTouch = 0.1f;
    static readonly float ZoomSpeedMouse = 0.5f;
    static readonly float[] ZoomBounds = new float[] {0.1f, 85f};
    Vector2 lastPanPosition;
    Vector3 startCameraPos;
    int panFingerId; // Touch mode only
    public static bool WasZoomingLastFrame; // Touch mode only
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
                if (!WasZoomingLastFrame)
                {
                    lastZoomPositions = newPositions;
                    WasZoomingLastFrame = true;
                }
                else
                {
                    // Zoom based on the distance between the new positions compared to the 
                    // distance between the previous positions.
                    var newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                    var oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    var towardsVec =
                        _camera.ScreenToWorldPoint((newPositions[1] - newPositions[0]) / 2 + newPositions[0]);
                    Debug.DrawLine(_camera.ScreenToWorldPoint(newPositions[0]), towardsVec);
                    var offset = newDistance - oldDistance;

                    // ZoomCamera(offset, ZoomSpeedTouch);
                    ZoomOrthoCamera(towardsVec, offset / 120);

                    lastZoomPositions = newPositions;
                }

                break;

            default:
                WasZoomingLastFrame = false;
                break;
        }
    }
}