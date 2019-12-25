using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/* アプリ全体の初期化やフェーズ管理を行う */
public class ARContentManager : MonoBehaviour{
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
    public DataManager dataManager;
    private Task nowTask;

    /* windowState
     * 0 : 通常のAR
     * 1 : caution表示
     * 2 : 検索表示
     * 3 : メニュー表示 */
    public int windowState = 1;    // windowの状態。


    public void SetNowTask(Task newTask) {
        nowTask = newTask;
    }

    void Start() {
        nowTask = new CreateCharacterTask(this, arRaycast, charaManager);
    }

    void Update() {
        try {
            nowTask.DoUpdateFunc();
        }catch(Exception e) {
            debugText.text = "Error Update: " + e.Message + "\n" + e.StackTrace;
        }

        if (windowState == 0 && !EventSystem.current.IsPointerOverGameObject(0) && Input.touchCount > 0) {
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
        windowState = 0;
    }

    /* spawn可能状態の画面に変更*/
    public void StartCreateCharacter() {
        overlayCanvasAnim.SetBool("Info", false);
        windowState = 0;   // 初期状態にセット
    }

    /* 検索状態に変更 */
    public void OpenSearchWindow() {
        if(windowState == 0) {
            windowState = 2;
            overlayCanvasAnim.SetBool("Search", true);
        }
    }

    /* 検索状態を終了 */
    public void CloseSearchWindow() {
        if(windowState == 2) {
            windowState = 0;
            dataManager.SearchWord("");
            overlayCanvasAnim.SetBool("Point", false);
            overlayCanvasAnim.SetBool("Search", false);
        }
    }

    /* メニュー状態に変更 */
    public void OpenMenuWindow() {
        if (windowState == 0) {
            windowState = 3;
            overlayCanvasAnim.SetBool("Menu", true);
        }
    }

    /* メニュー状態を終了 */
    public void CloseMenuWindow() {
        if (windowState == 3) {
            windowState = 0;
            overlayCanvasAnim.SetBool("Menu", false);
            dataManager.SearchWord("");
        }
    }

    /* 地点のwindowを開く */
    public void OpenPointWindow() {
        overlayCanvasAnim.SetBool("Point", true);
    }

    /* 地点のwindowを閉じる */
    public void ClosePointWindow() {
        overlayCanvasAnim.SetBool("Point", false);
    }
}
