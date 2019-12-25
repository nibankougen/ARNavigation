using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CharacterWaitTask : Task {
    private const float farDistance = 1.5f;  // これより離れている場合はこの距離まで近付く
    private const float nearDistance = 0.5f;    // ここまで近付く

    private Vector3 goalPosition;


    /* コンストラクタ */
    public CharacterWaitTask(ARContentManager newARContentManager, ARRaycastManager newArRaycast, CharacterManager newCharaManager) {
        arContentManager = newARContentManager;
        arRaycast = newArRaycast;
        charaManager = newCharaManager;

        // 離れた位置に生成した場合は近寄る
        Vector3 distance = charaManager.GetCharacterPose().position - arContentManager.camPosition.position;
        distance.y = 0;
        if(distance.magnitude > farDistance) {
            Vector3 camToCharacter = charaManager.GetCharacterPose().position - arContentManager.camPosition.position;
            goalPosition = camToCharacter.normalized * nearDistance + arContentManager.camPosition.position;
            goalPosition.y = charaManager.GetCharacterPose().position.y;   // 高さはスタート地点と同じとする
            charaManager.WalkStart(goalPosition);
        }
    }

    /* Updata時に呼ばれる */
    public override void DoUpdateFunc() {
        if (charaManager.GetWalk()) {
            Vector3 distance = charaManager.GetCharacterPose().position - arContentManager.camPosition.position;
            distance.y = 0;

            if(distance.magnitude <= nearDistance) {
                // 十分近いので移動終了
                charaManager.WalkEnd();
            } else {
                Vector3 camToCharacter = charaManager.GetCharacterPose().position - arContentManager.camPosition.position;
                goalPosition = camToCharacter.normalized * nearDistance + arContentManager.camPosition.position;
                goalPosition.y = charaManager.GetCharacterPose().position.y;    // 高さは最新の場所に更新
                charaManager.WalkStart(goalPosition);
            }
        } else {
            // 離れた位置に生成した場合は近寄る
            Vector3 distance = charaManager.GetCharacterPose().position - Camera.current.transform.position;
            distance.y = 0;
            if (distance.magnitude > farDistance) {
                Vector3 camToCharacter = charaManager.GetCharacterPose().position - arContentManager.camPosition.position;
                goalPosition = camToCharacter.normalized * nearDistance + arContentManager.camPosition.position;
                goalPosition.y = charaManager.GetCharacterPose().position.y;   // 高さはスタート地点と同じとする
                charaManager.WalkStart(goalPosition);
            }
        }
    }

    public override void Touched(Vector2 position) {

    }
}
