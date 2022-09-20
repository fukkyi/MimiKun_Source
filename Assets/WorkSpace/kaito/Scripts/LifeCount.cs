using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LifeCount : MonoBehaviour

{
    public GameObject score_object = null; // Textオブジェクト
    public int score_num = 0; // スコア変数

    [SerializeField]
    private ScoreModel scoreModel = null;


    // Start is called before the first frame update
    void Start()
    {
        int Life = scoreModel.playerHp;
        // オブジェクトからTextコンポーネントを取得
        Text score_text = score_object.GetComponent<Text>();
        // テキストの表示を入れ替える
        score_text.text = "ライフ:" + Life * 300; //ここにプレイヤーが所持していた残りのライフを出す？
    }

    // Update is called once per frame
    void Update()
    {
        


    }
}
