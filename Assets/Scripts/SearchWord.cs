using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchWord {
    public string word { get; private set; }
    public string placeID { get; private set; }
    public string floor { get; private set; }   // nullのとき建物情報

    public SearchWord(string newWord, string newPlaceID) {
        word = newWord;
        placeID = newPlaceID;
    }

    public SearchWord(string newWord, string newPlaceID, string newFloor) {
        word = newWord;
        placeID = newPlaceID;
        floor = newFloor;
    }

    public bool Check(string search) {
        return word.Contains(search);
    }

    
}
