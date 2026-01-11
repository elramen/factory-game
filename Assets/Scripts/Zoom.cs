using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zoom : MonoBehaviour
{
    public CameraManager cameraManager;

    public float zoomBy = 1;
    public float minSize = 1;
    public float maxSize = 10;

    public Button zoomInButton;
    public Button zoomOutButton;
    public float zoomTime;
    public AnimationCurve movement;

    void Start()
    {
        SetButtons();
    }

    public void ZoomIn()
    {
        StartCoroutine(AnimateZoom(cameraManager.screenUnitWidth, cameraManager.screenUnitWidth - zoomBy));
    }

    public void ZoomOut()
    {
        StartCoroutine(AnimateZoom(cameraManager.screenUnitWidth, cameraManager.screenUnitWidth + zoomBy));
    }

    IEnumerator AnimateZoom(float start, float stop)
    {
        float change = stop - start;

        float timeElapsed = 0f;

        while (timeElapsed < zoomTime)
        {
            cameraManager.screenUnitWidth =
                start + change * movement.Evaluate(timeElapsed / zoomTime);
            cameraManager.SetCameraSize();

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        cameraManager.screenUnitWidth = stop;
        cameraManager.SetCameraSize();

        SetButtons();
        yield return null;
    }

    private void SetButtons()
    {
        zoomInButton.interactable = canZoomIn();
        zoomOutButton.interactable = canZoomOut();
    }

    private bool canZoomIn()
    {
        return (cameraManager.screenUnitWidth - zoomBy >= minSize);
    }

    private bool canZoomOut()
    {
        return (cameraManager.screenUnitWidth + zoomBy <= maxSize);
    }
}
