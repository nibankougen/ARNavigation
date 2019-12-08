using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


public class CreateCharacterTask : Task{
    float startTime;

    /* コンストラクタ */
    public CreateCharacterTask(AppManager newAppManager, ARRaycastManager newArRaycast, CharacterManager newCharaManager, Animator newAnim) {
        appManager = newAppManager;
        arRaycast = newArRaycast;
        charaManager = newCharaManager;
        anim = newAnim;
        startTime = Time.timeSinceLevelLoad;
    }

    /* Updata時に呼ばれる */
    public override void DoUpdateFunc() {
        Vector3 screenSpawnPoint = appManager.cam.ViewportToScreenPoint(new Vector3(0.5f, 0.4f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        
        if(Time.timeSinceLevelLoad - startTime > 4.0f && arRaycast.Raycast(screenSpawnPoint, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes)) {
            charaManager.ActiveRespawn(hits[0].pose.position, Camera.current.transform.forward);
            anim.SetBool("Info", false);
        } else {
            charaManager.InactiveRespawn();
        }
    }

    public override void Touched(Vector2 position) {
        charaManager.CreateCharacter();

        appManager.SetNowTask(new CharacterWaitTask(appManager, arRaycast, charaManager, anim));
    }
}
