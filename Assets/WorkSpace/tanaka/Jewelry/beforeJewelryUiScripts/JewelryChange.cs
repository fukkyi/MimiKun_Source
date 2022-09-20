using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JewelryChange : MonoBehaviour
{
    //宝石画像の配列
    [SerializeField]
    public Sprite[] sprite = default;

    //宝石を載せるImage
    [SerializeField]
    private Image[] Image = default;

    //宝石カウント用スクリプト
    public JewelryCount jewelryCount;

    //現在選択している宝石の種類
    public JewelryType nowJewelry = JewelryType.Emerald;

    void Start()
    {
        //初期化
        JewelryCount jewelryCount = GetComponent<JewelryCount>();
        for (int i = 0; i < 4; i++)
        {
            Image[i].sprite = sprite[i];
        }
    }

    void Update()
    {
        #region DebugKeyConfig
        
        //デバッグ用(「←」で宝石が左回転)
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            for(int i = 0;i < 4; i++)
            {
                if(i == 0)
                {
                    Image[4].sprite = Image[0].sprite;
                }
                Image[i].sprite = Image[i + 1].sprite;
            }
            //現在選択中の宝石の情報を取得
            jewelrySerch();
        }

        //デバッグ用(「→」で宝石が右回転)※上手な回し方が思い浮かばなかったので力技
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Image[4].sprite = Image[0].sprite;
            Image[0].sprite = Image[3].sprite;
            Image[3].sprite = Image[2].sprite;
            Image[2].sprite = Image[1].sprite;
            Image[1].sprite = Image[4].sprite;
            //現在選択中の宝石の情報を取得
            jewelrySerch();
        }

        //デバッグ用(「↑」で現在選択中の宝石を1個加算)
        /*
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            jewelrySerch();
            jewelryCount.pickUp(centerJewelry);
        }

        //デバッグ用(「↓」で現在選択中の宝石を1個減算※0個以下の場合は減少なし)
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            jewelrySerch();
            jewelryCount.discard(centerJewelry);
        }
        */
        #endregion

        //現在選択中の宝石の個数表示
        jewelryCount.indication(nowJewelry);
            
    }
    //現在選択中の宝石を探す
    public void jewelrySerch()
    {
        for (int i = 0; i < 4; i++)
        {
            //中央の宝石の種類
            if (Image[0].sprite == sprite[i])
            {
                nowJewelry = (JewelryType)i;
            }
        }
    }

    public void SelectJewelryToRight()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                Image[4].sprite = Image[0].sprite;
            }
            Image[i].sprite = Image[i + 1].sprite;
        }
        //現在選択中の宝石の情報を取得
        jewelrySerch();
    }

    public void SelectJewelryToLeft()
    {
        Image[4].sprite = Image[0].sprite;
        Image[0].sprite = Image[3].sprite;
        Image[3].sprite = Image[2].sprite;
        Image[2].sprite = Image[1].sprite;
        Image[1].sprite = Image[4].sprite;
        //現在選択中の宝石の情報を取得
        jewelrySerch();
    }
}
