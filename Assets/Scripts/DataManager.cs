using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/* データの管理および検索とそれに関係するデータの管理 */
public class DataManager : MonoBehaviour {
    public ARContentManager arContentManager;

    public RectTransform wordContentPrefab;
    public RectTransform searchedWordsField;
    public RectTransform searchWindowTrans;
    public InputField searchInputField;
    public Animator overlayCanvasAnim;
    public Text selectingPointNameText;

    private const int diffOfFullToHalf = '０' - '0';
    private static int windowMaxHeight;

    private int campusSelect = 0;
    private string selectedPointID = null;

    private List <MapNode> nodes;
    private Dictionary<string, SearchWord> places;
    private List<SearchWord> words;

    /* placesとwordsのロード */
    private void Start() {
        windowMaxHeight = Screen.height * 600 / Screen.width - 100;
        LoadData();
    }

    private void LoadData() {
        string graphFileName, placesFileName, searchWordsFileName;

        if (campusSelect == 0) {
            // 大岡山
            graphFileName = "graph_O";
            placesFileName = "Places_O";
            searchWordsFileName = "SearchWords_O";
        } else {
            graphFileName = "graph_S";
            placesFileName = "Places_S";
            searchWordsFileName = "SearchWords_S";
        }

        // 最初にグラフを作成
        nodes = new List<MapNode>();

        TextAsset nodesText = Resources.Load(graphFileName) as TextAsset;
        using (StringReader reader = new StringReader(nodesText.text)) {
            // ノード作成
            while(reader.Peek() > -1) {
                string nodesLine = reader.ReadLine();
                if(nodesLine == "") {
                    // ノード作成終了
                    break;
                }

                string[] nodesContents = nodesLine.Split(',');
                MapNode newNode = new MapNode(
                        float.Parse(nodesContents[0]),
                        float.Parse(nodesContents[1]),
                        float.Parse(nodesContents[2]));

                if(nodesContents[3] != "") {
                    newNode.id = nodesContents[3];
                }

                nodes.Add(newNode);
            }

            // エッジ作成
            while (reader.Peek() > -1) {
                string nodesLine = reader.ReadLine();
                string[] nodesContents = nodesLine.Split(',');

                int id_1 = int.Parse(nodesContents[0]);
                int id_2 = int.Parse(nodesContents[1]);
                float len = float.Parse(nodesContents[2]);
                nodes[id_2].nextNodes.Add(new NextNode(nodes[id_1], len));
                nodes[id_1].nextNodes.Add(new NextNode(nodes[id_2], len));
            }
        }


        places = new Dictionary<string, SearchWord>();
        words = new List<SearchWord>();

        TextAsset placesText = Resources.Load(placesFileName) as TextAsset;
        using (StringReader reader = new StringReader(placesText.text)) {
            while (reader.Peek() > -1) {
                string placesLine = reader.ReadLine();

                string[] placesContents = placesLine.Split(',');
                SearchWord newPlace = new SearchWord(placesContents[1], placesContents[0]);
                places[placesContents[0]] = newPlace;
                words.Add(newPlace);
            }
        }
        
        TextAsset wordsText = Resources.Load(searchWordsFileName) as TextAsset;
        using(StringReader reader = new StringReader(wordsText.text)) {
            while(reader.Peek() > -1) {
                string wordsLine = reader.ReadLine();
                string[] wordsContents = wordsLine.Split(',');
                SearchWord newWord = new SearchWord(wordsContents[0], wordsContents[1], wordsContents[2]);
                words.Add(newWord);
            }
        }
    }

    /* 単語の検索を行う */
    public void SearchWord(string searchWord) {
        // まずすでに表示されているものを削除
        foreach (Transform old in searchedWordsField) {
            Destroy(old.gameObject);
        }

        if (arContentManager.windowState != 2) { // 検索状態かチェック
            searchInputField.text = "";
            searchWindowTrans.sizeDelta = new Vector2(-40, 76);     // サイズを変更
            return;
        }

        if(searchWord.IndexOf("\n", System.StringComparison.Ordinal) != -1) {
            // 改行が入力された時入力終了
            searchInputField.DeactivateInputField();
            searchWord = searchWord.Replace("\n", "");  // 改行を取り除く
            searchInputField.text = searchWord;
        }


        int wordsCount = 0;

        // アルファベットを大文字にして数字は半角にする
        searchWord = Regex.Replace(searchWord.ToUpper(), "[０-９]", p => ((char)(p.Value[0] - diffOfFullToHalf)).ToString());

        // 検索に引っかかった場合は表示追加
        string newPlace;
        if (campusSelect == 0) {
            newPlace = "大岡山キャンパス";
        } else {
            newPlace = "すずかけ台キャンパス";
        }

        if (searchWord != null && searchWord != "") {
            // 検索実行
            foreach (SearchWord targetWord in words) {
                if (targetWord.Check(searchWord)) {
                    wordsCount++;

                    if (targetWord.floor != null) {
                        newPlace += " " + places[targetWord.placeID].word
                            + " " + targetWord.floor + "階";
                    }

                    // 表示
                    RectTransform newWordContentObj = Instantiate(wordContentPrefab) as RectTransform;
                    newWordContentObj.SetParent(searchedWordsField, false);

                    WordsContent newWordsContent = newWordContentObj.GetComponent<WordsContent>();
                    newWordsContent.SetNameText(targetWord.word, newPlace, this, targetWord.placeID);
                }
            }
        }

        // windowのサイズを設定
        if(wordsCount == 0) {
            searchWindowTrans.sizeDelta = new Vector2(-40, 76);
        } else {
            int windowHeight = 100 * wordsCount + 80;
            
            if(windowHeight > windowMaxHeight) {
                windowHeight = windowMaxHeight;
            }
            searchWindowTrans.sizeDelta = new Vector2(-40, windowHeight);
        }
    }


    public void PointSelect(string placeID) {
        searchInputField.DeactivateInputField();
        if (places.ContainsKey(placeID)){
            selectedPointID = placeID;
            selectingPointNameText.text = places[placeID].word;
            arContentManager.OpenPointWindow();
        }
    }

}
