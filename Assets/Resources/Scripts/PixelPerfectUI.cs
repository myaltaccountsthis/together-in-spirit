using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class PixelPerfectUI : MonoBehaviour
{
    private Camera MainCamera;
    private CanvasScaler canvasScaler;
    private int defaultHeight;

    void Start() {
        MainCamera = Camera.main;
        canvasScaler = GetComponent<CanvasScaler>();
        defaultHeight = MainCamera.GetComponent<PixelPerfectCamera>().refResolutionY;
        AdjustScalingFactor();
    }

    void LateUpdate() {
        AdjustScalingFactor();
    }

    private int GetPixelRatio() {
        return Mathf.Max(Screen.height / defaultHeight, 1);
    }

    private void AdjustScalingFactor() {
        canvasScaler.scaleFactor = GetPixelRatio();
    }
}