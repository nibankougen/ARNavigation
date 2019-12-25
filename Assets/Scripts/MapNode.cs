using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode {
    public float latitude, longitude, elevation;
    public string id;
    public List<NextNode> nextNodes;
    public float sumOfLen;  // ノードの距離計算に使用


    public MapNode(float newLatitude, float newLongitude, float newElevation) {
        latitude = newLatitude;
        longitude = newLongitude;
        elevation = newElevation;
        nextNodes = new List<NextNode>();
    }
}
