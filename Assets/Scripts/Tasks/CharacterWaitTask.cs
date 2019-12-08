using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CharacterWaitTask : Task {
    private const float farDistance = 1.5f;  // これより離れている場合はこの距離まで近付く

    private bool move = false;
    private Vector3 goalPosition;


    /* コンストラクタ */
    public CharacterWaitTask(AppManager newAppManager, ARRaycastManager newArRaycast, CharacterManager newCharaManager, Animator newAnim) {
        appManager = newAppManager;
        arRaycast = newArRaycast;
        charaManager = newCharaManager;
        anim = newAnim;


        // 離れた位置に生成した場合は近寄る
        Vector3 distance = charaManager.GetCharacterPose().position - Camera.current.transform.position;
        distance.y = 0;
        if(distance.magnitude > farDistance) {
            goalPosition = appManager.camPosition.position;
            goalPosition.y = charaManager.GetCharacterPose().position.y;   // 高さはスタート地点と同じとする
            charaManager.MoveStart();
            move = true;
        }
    }

    /* Updata時に呼ばれる */
    public override void DoUpdateFunc() {
        if (move) {
            goalPosition = appManager.camPosition.position;     // 目的地は最新のカメラ位置に更新
            goalPosition.y = charaManager.GetCharacterPose().position.y;    // 高さは最新の場所に更新
            move = charaManager.Move(goalPosition);
        } else {
            // 離れた位置に生成した場合は近寄る
            Vector3 distance = charaManager.GetCharacterPose().position - Camera.current.transform.position;
            distance.y = 0;
            if (distance.magnitude > farDistance) {
                goalPosition = appManager.camPosition.position;
                goalPosition.y = charaManager.GetCharacterPose().position.y;   // 高さはスタート地点と同じとする
                charaManager.MoveStart();
                move = true;
            }
        }
    }

    public override void Touched(Vector2 position) {

    }
}
