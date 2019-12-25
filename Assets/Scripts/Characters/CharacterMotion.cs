using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* キャラクターのアニメーションおよび表情モーフを制御するためのクラス */
public abstract class CharacterMotion : MonoBehaviour{
    protected const string ANIM_BOOL_WALK = "Walk";
    protected const string ANIM_BOOL_CALL = "Call";
    protected const string ANIM_TRIGGER_AGREE = "Agree";
    protected const string ANIM_TRIGGER_DISAGREE = "Disagree";
    protected const string ANIM_TRIGGER_SMILE = "Smile";

    public Animator anim, faceAnim;
    public SkinnedMeshRenderer faceMesh;
    public bool canWalk = false;    // モーション的に歩くことができる状態かを判定

    public float walkSpeed;    //キャラクターごとの歩く速さ

    public abstract void Smile();
    public abstract void Agree();
    public abstract void Disagree();
    public abstract void WalkStart();
    public abstract void WalkEnd();
    public abstract void CallStart();
    public abstract void CallEnd();


    /* 以下はアニメーションと歩行の整合性を取るためにアニメーションから呼び出す関数 */
    public void CanWalkToTrue() {
        canWalk = true;
    }

    public void CanWalkToFalse() {
        canWalk = false;
    }
}
