using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


/* キャラクターおよびリスポーン地点の位置や動きの管理をする */
public class CharacterManager : MonoBehaviour{
    public GameObject respawnObj;
    public GameObject[] characterObjects; //キャラクターのprefab
    private GameObject controllingCharacterObj = null;

    public ARRaycastManager arRaycast;    // レイキャスト用

    private bool respawnActive = false;

    private Pose characterPose; // キャラクターの現在位置ただしy座標は補正する場合あり
    private Vector3 goalPosition;
    private CharacterMotion motion; //キャラクターの動きの管理
    private float pastTime; //前回呼び出された時の時間
    private CharacterMotion nowCharacter;   // 現在のキャラクターのモーションやパラメーターを管理
    private int characterNum;
    private bool walk = false;

    private const float gravityAc = 1f;   // 重力加速度、この加速度に従って上方向に移動する
    private float moveSpeed_Y = 0f;   // Y方向、上下方向の移動速度

    private void Start() {
        SetCharacter(0);    // デフォルトでテックちゃんをセット
    }
    

    /* 位置は何もしていない時でも地面を認識する可能性があるので常に更新 */
    private void FixedUpdate() {
        float nowTime = Time.timeSinceLevelLoad;
        float timeDiv = nowTime - pastTime;
        pastTime = nowTime;     // もうpastTimeは使わないので時間の更新

        if (walk && nowCharacter.canWalk) {
            Vector3 direction = goalPosition - characterPose.position;
            float maxLen = nowCharacter.walkSpeed * timeDiv;    // 移動の最大距離

            if (direction.magnitude > maxLen) {
                characterPose.position = characterPose.position + (direction.normalized * maxLen);   // 仮の新しい位置
            } else {
                //速度よりも小さい時その場所にセットして終了
                characterPose.position = goalPosition;   // 仮の新しい位置

                //動作終了の処理
                WalkEnd();
            }

            /* 以下回転 */
            direction.y = 0;    // 上下方向には回転しない
            characterPose.rotation = Quaternion.LookRotation(direction);
        }
        

        /* 以下上下移動 */
        Vector3 rayOrigin = new Vector3(characterPose.position.x, characterPose.position.y + 4.0f, characterPose.position.z);
        Ray detectPlaneRay = new Ray(rayOrigin, new Vector3(0, -1, 0));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (arRaycast.Raycast(detectPlaneRay, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds)) {
            float div_y = hits[0].pose.position.y - characterPose.position.y;

            moveSpeed_Y += gravityAc * timeDiv; // 最高上下移動速度

            if (moveSpeed_Y < div_y) {
                div_y = moveSpeed_Y;
            }else if (div_y < -moveSpeed_Y) {
                div_y = -moveSpeed_Y;
            } else {
                //現在の速度で目的の高さまで到達できるので速度初期化
                moveSpeed_Y = 0f;
            }

            characterPose.position.y += div_y;
        } else {
            moveSpeed_Y = 0f;   // 上下方向には動かない
        }

        if(controllingCharacterObj != null) {
            controllingCharacterObj.transform.position = characterPose.position;
            controllingCharacterObj.transform.rotation = characterPose.rotation;
        }
    }

    public Pose GetCharacterPose() {
        return characterPose;
    }

    public bool GetWalk() {
        return walk;
    }

    /* 番号に応じてキャラクターをセットする */
    private void SetCharacter(int newCharacterNum) {
        characterNum = newCharacterNum;
    }


    /* CreateCharacterTaskで使用---------------------------------------------- */

    /* リスポーン地点の有効化および位置の制御を行う
     * cameraDirection：カメラの向き。normarizeは不要 */
    public void ActiveRespawn(Vector3 newRespawnPosition, Vector3 cameraDirection) {
        characterPose.position = newRespawnPosition;
        Vector3 cameraBearing = new Vector3(-cameraDirection.x, 0, -cameraDirection.z);
        characterPose.rotation = Quaternion.LookRotation(cameraBearing);

        respawnObj.SetActive(true);
        respawnObj.transform.SetPositionAndRotation(characterPose.position, Quaternion.Euler(90f, 0f, 0f));


        respawnActive = true;

        if(controllingCharacterObj != null) {
            controllingCharacterObj.transform.rotation = characterPose.rotation;
        }

    }

    /* リスポーン地点の無効化を行う */
    public void InactiveRespawn() {
        respawnObj.SetActive(false);
        respawnActive = false;
    }

    /* キャラクターの生成ができる時trueを返す */
    public bool CreateCharacter() {
        if (respawnActive) {
            if(controllingCharacterObj != null) {
                Destroy(controllingCharacterObj);    // 既にでている場合は削除
            }

            controllingCharacterObj = Instantiate(characterObjects[characterNum], characterPose.position, characterPose.rotation);
            switch (characterNum) {
                case 0: // テックちゃん
                    nowCharacter = controllingCharacterObj.GetComponent<TechChanMotion>();
                    break;
            }
            respawnObj.SetActive(false);
        }

        return respawnActive;
    }



    /* NavigationTaskで使用---------------------------------------------- */
    /* 動作開始時の処理 */
    public void WalkStart(Vector3 newGoalPosition) {
        goalPosition = newGoalPosition;
        nowCharacter.WalkStart();
        walk = true;
    }

    /* 目的地に着いた時に呼び出される。
     * 目的地についていなくても強制的に動作終了 */
    public void WalkEnd() {
        nowCharacter.WalkEnd();
        walk = false;
    }


    /* 自分の方を見る */
    public void LookAtPlayer(Vector3 cameraDirection) {
        Vector3 cameraBearing = new Vector3(-cameraDirection.x, 0, -cameraDirection.z);
        characterPose.rotation = Quaternion.LookRotation(cameraBearing);
        controllingCharacterObj.transform.rotation = characterPose.rotation;
    }

}
