using UnityEngine;
using UnityEngine.XR.ARFoundation;

public abstract class Task{
    protected AppManager appManager;
    protected ARRaycastManager arRaycast;
    protected Animator anim;
    protected CharacterManager charaManager;

    public abstract void DoUpdateFunc();
    public abstract void Touched(Vector2 position);

}
