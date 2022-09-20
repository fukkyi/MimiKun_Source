using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JewelryCount : MonoBehaviour
{
    //宝石の個数表示のテキスト取得
    [SerializeField]
    private TextMeshProUGUI centerCount;

    //それぞれの宝石の個数の格納用の配列
    // public int[] jewelryCount = new int[4] {0,0,0,0};

    //宝石を1個加算
    public int pickUp(int n)
    {
        Debug.Log("宝石拾った");
        // jewelryCount[n]++;

        //宝石の所持個数表示の更新
        //indication(n);
        return 0;
    }
    //宝石を1個減算※0個以下の場合減少なし
    public int discard(int n)
    {
        Debug.Log("宝石捨てた");
        /*
        if (jewelryCount[n] > 0)
        {
            // jewelryCount[n]--;

            //宝石の所持個数表示の更新
            indication(n);
        }
        */
        return 0;
    }

    //選択中の宝石の現在所持個数の表示
    public int indication(JewelryType jewelryType)
    {
        // centerCount.text = jewelryCount[n].ToString();
        centerCount.text = GameSceneController.Instance.Player.JewelryPossessionStatus.GetJewelryCountByType(jewelryType).ToString();
        return 0;
    }
}
