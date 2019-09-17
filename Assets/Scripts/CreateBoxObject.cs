using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class CreateBoxObject : MonoBehaviour {
    [SerializeField]
    GameObject objectPrefab;

    ARRaycastManager raycastManager;
    List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

    void Awake() {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (raycastManager.Raycast(Input.GetTouch(0).position, hitResults)) {
                Instantiate(objectPrefab, hitResults[0].pose.position, Quaternion.identity);
            }
        }
    }
}