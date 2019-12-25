using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


public class CreateCharacterTask : Task{
    float startTime;

    /* コンストラクタ */
    public CreateCharacterTask(ARContentManager newArContentManager, ARRaycastManager newArRaycast, CharacterManager newCharaManager) {
        arContentManager = newArContentManager;
        arRaycast = newArRaycast;
        charaManager = newCharaManager;
        startTime = Time.timeSinceLevelLoad;
    }

    /* Updata時に呼ばれる */
    public override void DoUpdateFunc() {
        Vector3 screenSpawnPoint = arContentManager.cam.ViewportToScreenPoint(new Vector3(0.5f, 0.4f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        
        if(Time.timeSinceLevelLoad - startTime > 4.0f && arRaycast.Raycast(screenSpawnPoint, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds)) {
            charaManager.ActiveRespawn(hits[0].pose.position, Camera.current.transform.forward);
            arContentManager.StartCreateCharacter();
        } else {
            charaManager.InactiveRespawn();
        }
    }

    public override void Touched(Vector2 position) {
        charaManager.CreateCharacter();

        arContentManager.SetNowTask(new CharacterWaitTask(arContentManager, arRaycast, charaManager));
    }
}
