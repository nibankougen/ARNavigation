using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


/* キャラクターおよびリスポーン地点の位置や動きの管理をする */
public class CharacterManager : MonoBehaviour{
    public GameObject respawnObj;
    public GameObject characterObj; //キャラクターのprefab
    private GameObject controllingCharacterObj = null;

    public ARRaycastManager arRaycast;    // レイキャスト用

    private bool respawnActive = false;

    private Pose characterPose; // キャラクターの現在位置ただしy座標は補正する場合あり
    private CharacterMotion motion; //キャラクターの動きの管理
    private float pastTime; //前回呼び出された時の時間
    private const float speed = 0.5f;  //キャラクターの時間当たりの速さ

    private int debugCount = 0;

    /* 位置は何もしていない時でも地面を認識する可能性があるので常に更新 */
    private void Update() {
        Vector3 rayOrigin = new Vector3(characterPose.position.x, characterPose.position.y + 4.0f, characterPose.position.z);
        Ray detectPlaneRay = new Ray(rayOrigin, new Vector3(0, -1, 0));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (arRaycast.Raycast(detectPlaneRay, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds)) {
            // hitがあった場合はそれに従って補正
            UIDebug.Log("ray cast hit : "+debugCount.ToString());
            debugCount++;
            float div_y = hits[0].pose.position.y - characterPose.position.y;

            characterPose.position.y += div_y;
        }

        if(controllingCharacterObj != null) {
            controllingCharacterObj.transform.position = characterPose.position;
            controllingCharacterObj.transform.rotation = characterPose.rotation;
        }
    }

    public Pose GetCharacterPose() {
        return characterPose;
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

            controllingCharacterObj = Instantiate(characterObj, characterPose.position, characterPose.rotation);
            respawnObj.SetActive(false);
        }

        return respawnActive;
    }



    /* NavigationTaskで使用---------------------------------------------- */
    /* 動作開始時の処理 */
    public void MoveStart() {
        pastTime = Time.timeSinceLevelLoad;
    }

    /* 位置の更新。基本的には現在位置からtargetPositionの方向に向かってspeedの分移動するが
     * 地面が認識されている時はその位置を補正する
     * 返り値は、まだ動いている時にtrueを返す*/
    public bool Move(Vector3 goalPosition) {
        Vector3 direction = goalPosition - characterPose.position;
        bool ret = true;
        float nowTime = Time.timeSinceLevelLoad;
        float maxLen = speed * (nowTime - pastTime);    // 移動の最大距離
        pastTime = nowTime;     // もうpastTimeは使わないので時間の更新

        if (direction.magnitude > maxLen) {
            characterPose.position = characterPose.position + (direction.normalized * maxLen);   // 仮の新しい位置
        } else {
            //速度よりも小さい時その場所にセットして終了
            characterPose.position = goalPosition;   // 仮の新しい位置
            ret = false;

            //動作終了の処理

        }

        /* 以下回転 */
        direction.y = 0;    // 上下方向には回転しない
        characterPose.rotation = Quaternion.LookRotation(direction);

        return ret;
    }


    /* 自分の方を見る */
    public void LookAtPlayer(Vector3 cameraDirection) {
        Vector3 cameraBearing = new Vector3(-cameraDirection.x, 0, -cameraDirection.z);
        characterPose.rotation = Quaternion.LookRotation(cameraBearing);
        controllingCharacterObj.transform.rotation = characterPose.rotation;
    }

}
