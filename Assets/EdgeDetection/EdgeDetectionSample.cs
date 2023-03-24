/* 
*   VideoKit OpenCV Sample
*   Copyright (c) 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using UnityEngine;
    using UnityEngine.UI;
    using NatML.VideoKit;
    using OpenCVForUnity.CoreModule;
    using OpenCVForUnity.ImgprocModule;
    using OpenCVForUnity.UtilsModule;
    using OpenCVForUnity.UnityUtils;

    public class EdgeDetectionSample : MonoBehaviour {

        [Header(@"Camera")]
        public VideoKitCameraManager cameraManager;

        [Header(@"UI")]
        public RawImage rawImage;
        public AspectRatioFitter aspectFitter;

        Mat imageMatrix;
        Mat edgeMatrix;
        Texture2D edgeTexture;

        private void Start () {
            // Check that camera manager has ML capability
            if (!cameraManager.capabilities.HasFlag(VideoKitCameraManager.Capabilities.MachineLearning)) {
                Debug.LogError(@"Camera manager must have machine learning capability enabled");
                return;
            }
            // Listen for camera frames
            cameraManager.OnCameraFrame.AddListener(OnCameraFrame);
        }

        private void OnCameraFrame (CameraFrame frame) {
            // Update frame matrix
            Texture2D frameTexture = frame.texture as Texture2D;
            imageMatrix ??= new Mat(frameTexture.height, frameTexture.width, CvType.CV_8UC4);
            MatUtils.copyToMat(frameTexture.GetRawTextureData<byte>(), imageMatrix);
            // Perform Canny edge detection
            edgeMatrix ??= new Mat();
            Imgproc.Canny(imageMatrix, edgeMatrix, 100, 200);
            // Convert to texture
            edgeTexture = edgeTexture ? edgeTexture : new Texture2D(edgeMatrix.width(), edgeMatrix.height(), TextureFormat.RGBA32, false);
            Utils.matToTexture2D(edgeMatrix, edgeTexture, false);
            // Display texture
            rawImage.texture = edgeTexture;
            aspectFitter.aspectRatio = (float)edgeTexture.width / edgeTexture.height;
        }

        private void OnDisable () {
            // Stop listening for frames
            cameraManager.OnCameraFrame.RemoveListener(OnCameraFrame);
            // Release matrix and texture
            Texture2D.Destroy(edgeTexture);
            edgeMatrix?.Dispose();
            imageMatrix?.Dispose();
        }
    }
}