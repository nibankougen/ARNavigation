using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* キャラクターのアニメーションおよび表情モーフを制御するためのクラス */
public abstract class CharacterMotion : MonoBehaviour{
    public Animator anim;
    public SkinnedMeshRenderer faceMesh;
    public Coroutine nowMotion = null;

    public void Smile() {
        if(nowMotion != null) {
            StopCoroutine(nowMotion);
        }
        nowMotion = StartCoroutine(SmileCoroutine());
    }

    public void Blink() {
        if (nowMotion != null) {
            StopCoroutine(nowMotion);
        }
        nowMotion = StartCoroutine(BlinkCoroutine());
    }

    public void Agree() {
        if (nowMotion != null) {
            StopCoroutine(nowMotion);
        }
        nowMotion = StartCoroutine(AgreeCoroutine());
    }

    public void Disagree() {
        if (nowMotion != null) {
            StopCoroutine(nowMotion);
        }
        nowMotion = StartCoroutine(DisagreeCoroutine());
    }

    public abstract void MoveStart();
    public abstract void MoveEnd();
    public abstract void CallStart();
    public abstract void CallEnd();
    public abstract IEnumerator SmileCoroutine();
    public abstract IEnumerator BlinkCoroutine();
    public abstract IEnumerator AgreeCoroutine();
    public abstract IEnumerator DisagreeCoroutine();
}
