using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.Rendering;

/* 動的光の変更 */
public class LightEstimation : MonoBehaviour{
    public ARCameraManager arCamManager;
    public Light controlLight;

    void Start() {
        GraphicsSettings.lightsUseLinearIntensity = true;
        GraphicsSettings.lightsUseColorTemperature = true;
        arCamManager.frameReceived += FrameChanged;
    }

    void OnDisable() {
        arCamManager.frameReceived -= FrameChanged;
    }

    void FrameChanged(ARCameraFrameEventArgs args) {
        if (args.lightEstimation.averageBrightness.HasValue) {
            controlLight.intensity = args.lightEstimation.averageBrightness.Value;
        }

        if (args.lightEstimation.averageColorTemperature.HasValue) {
            controlLight.colorTemperature = args.lightEstimation.averageColorTemperature.Value;
        }
    }
}
