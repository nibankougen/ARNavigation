using System;
using UnityEngine;

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




}
