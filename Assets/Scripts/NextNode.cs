using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextNode {
    MapNode next;
    float distance;

    public NextNode(MapNode newNext, float newDistance) {
        next = newNext;
        distance = newDistance;
    }
}