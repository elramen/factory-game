using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class CameraManager : MonoBehaviour
{
    public Camera cam;
    
    public float screenUnitWidth = 0;
    public float screenUnitHeight = 0;
    public int screenPixelWidth = 0;
    public int screenPixelHeight = 0;
    public float aspectRatio;

    void Start()
    {
        cam = GetComponent<Camera>();
        SetCameraSize();
    }

    void OnValidate()
    {
        SetCameraSize();
    }

    public void SetCameraSize()
    {
        screenPixelWidth = cam.pixelWidth;
        screenPixelHeight = cam.pixelHeight;
        aspectRatio = (float)screenPixelWidth / (float)screenPixelHeight;
        float newHeight = screenUnitWidth / (2 * aspectRatio);
		screenUnitHeight = newHeight * 2;
		cam.orthographicSize = newHeight;
    }

    public Vector2 PixelVectorToUnits(Vector2 pixelVector)
    {
        return new Vector2(
            pixelVector.x * screenUnitWidth / screenPixelWidth,
            pixelVector.y * screenUnitHeight / screenPixelHeight
        );
    }
}
