/* 
*   NatDevice OpenCV
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using UnityEngine;
    using UnityEngine.UI;
    using NatML.Devices;
    using NatML.Devices.Outputs;
    using OpenCVForUnity.CoreModule;
    using OpenCVForUnity.ImgprocModule;
    using OpenCVForUnity.UnityUtils;

    public class EdgeDetectionSample : MonoBehaviour {

        [Header(@"UI")]
        public RawImage rawImage;
        public AspectRatioFitter aspectFitter;

        CameraDevice cameraDevice;
        MatOutput matOutput;

        Mat edgeMatrix;
        Texture2D edgeTexture;

        async void Start () {
            // Request camera permissions
            var permissionStatus = await MediaDeviceQuery.RequestPermissions<CameraDevice>();
            if (permissionStatus != PermissionStatus.Authorized) {
                Debug.LogError(@"User did not grant camera permissions");
                return;
            }
            // Discover a camera device
            var query = new MediaDeviceQuery(MediaDeviceCriteria.CameraDevice);
            cameraDevice = query.current as CameraDevice;
            // Start the preview // Here we create a matrix output that produces gray matrices
            matOutput = new MatOutput() { format = Imgproc.COLOR_RGBA2GRAY };
            cameraDevice.StartRunning(matOutput);
            // Create edge matrix and texture
            edgeMatrix = new Mat();
        }

        void Update () {
            // Check that the preview matrix is available
            if (matOutput?.matrix == null)
                return;
            // Perform Canny edge detection
            Imgproc.Canny(matOutput.matrix, edgeMatrix, 100, 200);
            // Convert to texture
            edgeTexture = edgeTexture ? edgeTexture : new Texture2D(edgeMatrix.width(), edgeMatrix.height(), TextureFormat.RGBA32, false);
            Utils.matToTexture2D(edgeMatrix, edgeTexture, false);
            // Display texture
            rawImage.texture = edgeTexture;
            aspectFitter.aspectRatio = (float)edgeTexture.width / edgeTexture.height;
        }

        void OnDisable () => matOutput?.Dispose();
    }
}