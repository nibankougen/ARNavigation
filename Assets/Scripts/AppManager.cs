using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/* アプリ全体の初期化やフェーズ管理を行う */
public class AppManager : MonoBehaviour{

#if UNITY_ANDROID && !UNITY_EDITOR

    private const int FLAG_NOT_FULL_SCREEN = 2048;

    void Awake(){
        // androidのみステータスバーの表示のためのスレッドを立てる
        Screen.fullScreen = false;
        RunOnAndroidUiThread(SetFlagsInThread);
    }

    private static void RunOnAndroidUiThread(Action thread) {
        // スレッドをセット
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                activity.Call("runOnUiThread", new AndroidJavaRunnable(thread));
            }
        }
    }

    private static void SetFlagsInThread() {
        // Windowにフラグをセット
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                using (var window = activity.Call<AndroidJavaObject>("getWindow")) {
                    window.Call("setFlags", FLAG_NOT_FULL_SCREEN, -1);
                }
            }
        }
    }
#endif


    public ARRaycastManager arRaycast;
    public Animator overlayCanvasAnim;
    public CharacterManager charaManager;
    public Text debugText;
    public Camera cam;
    public Transform camPosition;
    private Task nowTask;
    private bool touchActive = false;


    public void SetNowTask(Task newTask) {
        nowTask = newTask;
    }

    void Start() {
        nowTask = new CreateCharacterTask(this, arRaycast, charaManager, overlayCanvasAnim);
    }

    void Update() {
        try {
            nowTask.DoUpdateFunc();
        }catch(Exception e) {
            debugText.text = "Error Update: " + e.Message + "\n" + e.StackTrace;
        }

        if (touchActive && !EventSystem.current.IsPointerOverGameObject(0) && Input.touchCount > 0) {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began) {
                // UI以外をタッチされたとき
                try {
                    nowTask.Touched(t.position);
                } catch (Exception e) {
                    debugText.text = "Error Touch: " + e.Message + "\n" + e.StackTrace;
                }
            }
        }
    }

    /* 最初のウィンドウを消してアクティベート */
    public void TouchActivate() {
        overlayCanvasAnim.SetTrigger("CautionOK");
        touchActive = true;
    }

}
