using UnityEngine;
using UnityEngine.XR.ARFoundation;

public abstract class Task{
    protected ARContentManager arContentManager;
    protected ARRaycastManager arRaycast;
    protected CharacterManager charaManager;

    public abstract void DoUpdateFunc();
    public abstract void Touched(Vector2 position);

}
