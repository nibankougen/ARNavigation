using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordsContent : MonoBehaviour {
    public Text nameText, placeText;
    private DataManager dm;
    private string placeID;

    public void SetNameText(string name, string place, DataManager newDm, string newPlaceID) {
        nameText.text = name;
        placeText.text = place;
        dm = newDm;
        placeID = newPlaceID;
    }

    public void PushedFunc() {
        dm.PointSelect(placeID);
    }
}
