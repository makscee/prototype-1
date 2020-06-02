using System;
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
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            ZoomOrthoCamera(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1);
        }
 
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ZoomOrthoCamera(Camera.main.ScreenToWorldPoint(Input.mousePosition), -1);
        }

        UpdateDrag();
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
        _camera.orthographicSize = orthographicSize - amount;

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
}