using Core.EventSystem;
using Scripts.Core.EventSystem;
using System;
using UnityEngine;

namespace Scripts.Core.Utiles
{
    [RequireComponent(typeof(Camera))]

    public class ForceAspectRatio : MonoBehaviour
    {
        public float targetAspect = 16f / 9f;

        private int lastScreenWidth;
        private int lastScreenHeight;

        void Start()
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            UpdateCameraViewport();
        }

        void Update()
        {
            if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
            {
                lastScreenWidth = Screen.width;
                lastScreenHeight = Screen.height;
                UpdateCameraViewport();
            }
        }

        void UpdateCameraViewport()
        {
            float windowAspect = (float)Screen.width / (float)Screen.height;
            float scaleHeight = windowAspect / targetAspect;

            Camera camera = Camera.main;

            if (scaleHeight < 1.0f)
            {
                Rect rect = new Rect(0, (1.0f - scaleHeight) / 2.0f, 1.0f, scaleHeight);
                camera.rect = rect;
            }
            else
            {
                float scaleWidth = 1.0f / scaleHeight;
                Rect rect = new Rect((1.0f - scaleWidth) / 2.0f, 0, scaleWidth, 1.0f);
                camera.rect = rect;
            }

            camera.backgroundColor = Color.black;
            camera.clearFlags = CameraClearFlags.SolidColor;
        }

    }
}
