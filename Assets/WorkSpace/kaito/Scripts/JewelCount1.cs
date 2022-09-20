using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class JewelCount1 : MonoBehaviour

{
    public GameObject score_object = null; // Textオブジェクト
    //public int score_num = 0; // スコア変数

    [SerializeField]
    private ScoreModel scoreModel = null;
    [SerializeField]
    private MasterDataJewelry masterDataJewelry = null;

    // Start is called before the first frame update
    void Start()
    {
        int score = scoreModel.jewelryStatus.JewelryEmeraldHaveCount * masterDataJewelry.emeraldJewelryScore;
        int score2 = scoreModel.jewelryStatus.JewelryRubyHaveCount * masterDataJewelry.rubyJewelryScore;
        int scere3 = scoreModel.jewelryStatus.JewelryAmethystHaveCount * masterDataJewelry.amethystJewelryScore;
        int score4 = scoreModel.jewelryStatus.JewelryDiamondHaveCount * masterDataJewelry.diamondJewelryScore;

        score = score + score2 + scere3 + score4;
        //// オブジェクトからTextコンポーネントを取得
        Text score_text = score_object.GetComponent<Text>();
        // テキストの表示を入れ替える
        score_text.text = "宝石:" + score;//ここにプレイヤーが所持していた残りの宝石数を出す？

    }

    // Update is called once per frame
    void Update()
    {
        


    }
}
