using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechChanMotion : CharacterMotion {
    public override void Smile() {

    }

    public override void Agree() {
        anim.SetTrigger(ANIM_TRIGGER_AGREE);
        faceAnim.SetTrigger(ANIM_TRIGGER_AGREE);
    }

    public override void Disagree() {
        anim.SetTrigger(ANIM_TRIGGER_DISAGREE);
        faceAnim.SetTrigger(ANIM_TRIGGER_DISAGREE);
    }

    public override void WalkStart() {
        anim.SetBool(ANIM_BOOL_WALK, true);
        faceAnim.SetBool(ANIM_BOOL_WALK, true);
    }

    public override void WalkEnd() {
        anim.SetBool(ANIM_BOOL_WALK, false);
        faceAnim.SetBool(ANIM_BOOL_WALK, false);
    }

    public override void CallStart() {
        anim.SetBool(ANIM_BOOL_CALL, true);
        faceAnim.SetBool(ANIM_BOOL_CALL, true);
    }

    public override void CallEnd() {
        anim.SetBool(ANIM_BOOL_CALL, false);
        faceAnim.SetBool(ANIM_BOOL_CALL, false);
    }
}
