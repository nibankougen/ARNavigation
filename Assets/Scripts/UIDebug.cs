using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebug : MonoBehaviour
{
    private static Text debugText = null;
    public Text sceneDebugText;

    void Start(){
        debugText = sceneDebugText;
    }

    public static void Log(string message) {
        if(debugText != null) {
            debugText.text = message;
        }
    }
}
