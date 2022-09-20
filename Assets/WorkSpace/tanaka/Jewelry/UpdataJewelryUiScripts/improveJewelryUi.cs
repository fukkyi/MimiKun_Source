using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class improveJewelryUi : MonoBehaviour
{
    //宝石画像の配列
    [Header("Emerald,Ruby,Amethyst,Diamondの順番で適用")]
    [SerializeField]
    private Sprite[] sprite = default;

    //宝石を載せるImage
    [SerializeField]
    private Image centerImage = default;

    //宝石の個数表示のテキスト取得
    [SerializeField]
    private TextMeshProUGUI centerCount;

    //現在選択している宝石の種類
    public JewelryType nowJewelry = JewelryType.Emerald;

    //それぞれの宝石の個数の格納用の配列
    public int[] jewelryCount = new int[4] { 0, 0, 0, 0 };
    //どの宝石の個数に変化があったか確認用
    private int[] oldJewelryCount = new int[4] { 0, 0, 0, 0 };

    //各種宝石を所有しているかどうか
    public bool existEmerald = false;
    public bool existRuby = false;
    public bool existAmethyst = false;
    public bool existDiamond = false;

    void Start()
    {
        //初期化

        centerImage.sprite = sprite[0];

        //初期化したとき宝石を所有してないので透明化
        centerImage.color = new Color(255, 255, 255, 0);
        centerCount.color = new Color(255, 255, 255, 0);
    }


    void Update()
    {

        #region DebugKeyConfig
        /*
        //デバッグ用(「←」or「Q」で宝石が左回転)
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Q))
        {
            leftChangeJewelry();
        }

        //デバッグ用(「→」or「E」で宝石が右回転)
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.E))
        {
            rightChangeJewelry();
        }

        //デバッグ用(宝石の種類(1,2,3,4)と「↑」で宝石を1個加算)
        //エメラルドを一個追加
        if (Input.GetKey(KeyCode.Alpha1) && Input.GetKeyDown(KeyCode.UpArrow))
        {
            jewelryCount[0]++;
            pickUp();
        }
        if (Input.GetKey(KeyCode.Alpha2) && Input.GetKeyDown(KeyCode.UpArrow))
        {
            jewelryCount[1]++;
            pickUp();
        }
        if (Input.GetKey(KeyCode.Alpha3) && Input.GetKeyDown(KeyCode.UpArrow))
        {
            jewelryCount[2]++;
            pickUp();
        }
        if (Input.GetKey(KeyCode.Alpha4) && Input.GetKeyDown(KeyCode.UpArrow))
        {
            jewelryCount[3]++;
            pickUp();
        }

        //デバッグ用(宝石の種類(1,2,3,4)と「↓」で現在選択中の宝石を1個減算※0個以下の場合は減少なし)
        if (Input.GetKey(KeyCode.Alpha1) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            jewelryCount[0]--;
            discard();
        }
        if (Input.GetKey(KeyCode.Alpha2) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            jewelryCount[1]--;
            discard();
        }
        if (Input.GetKey(KeyCode.Alpha3) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            jewelryCount[2]--;
            discard();
        }
        if (Input.GetKey(KeyCode.Alpha4) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            jewelryCount[3]--;
            discard();
        }
        */
        #endregion

        //indication(nowJewelry);

    }

    //どの宝石を拾ったか判別
    public void pickUp()
    {
        jewelryCount = ConvertJewelryCountByPossessionStatus(jewelryCount);
        Debug.Log("宝石拾った");
        //どの宝石を拾ったか
        for (int i = 0; i < 4; i++)
        {
            //変化があった宝石を探す
            if (oldJewelryCount[i] != jewelryCount[i])
            {
                //拾った宝石を表示する
                switch (i)
                {
                    case 0:
                        {
                            existEmerald = true;
                            indication(JewelryType.Emerald);
                            break;
                        }
                    case 1:
                        {
                            existRuby = true;
                            indication(JewelryType.Ruby);
                            break;
                        }
                    case 2:
                        {
                            existAmethyst = true;
                            indication(JewelryType.Amethyst);
                            break;
                        }
                    case 3:
                        {
                            existDiamond = true;
                            indication(JewelryType.Diamond);
                            break;
                        }
                }
            }
        }
        
        jewelryCount.CopyTo(oldJewelryCount, 0);
    }

    //どの宝石を捨てたか判別
    public void discard()
    {
        jewelryCount = ConvertJewelryCountByPossessionStatus(jewelryCount);
        Debug.Log("宝石捨てた");
        //どの宝石を捨てたか
        for (int i = 0; i < 4; i++)
        {
            //変化があった宝石を探す
            if (oldJewelryCount[i] != jewelryCount[i])
            {
                //0以下になった場合、0にする
                if (jewelryCount[i] <= 0) jewelryCount[i] = 0;

                oldJewelryCount[i] = jewelryCount[i];

                //0になっていた場合、各種宝石のフラグを変更する
                //選択中の宝石が0になった場合、別の所有している宝石に変更
                if (jewelryCount[i] <= 0)
                {
                    switch (i)
                    {
                        case 0:
                            {
                                existEmerald = false;
                                if (nowJewelry == JewelryType.Emerald)
                                    rightChangeJewelry();
                                break;
                            }
                        case 1:
                            {
                                existRuby = false;
                                if (nowJewelry == JewelryType.Ruby)
                                    rightChangeJewelry();
                                break;
                            }
                        case 2:
                            {
                                existAmethyst = false;
                                if (nowJewelry == JewelryType.Amethyst)
                                    rightChangeJewelry();
                                break;
                            }
                        case 3:
                            {
                                existDiamond = false;
                                if (nowJewelry == JewelryType.Diamond)
                                    rightChangeJewelry();
                                break;
                            }
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            {
                                indication(JewelryType.Emerald);
                                break;
                            }
                        case 1:
                            {
                                indication(JewelryType.Ruby);
                                break;
                            }
                        case 2:
                            {
                                indication(JewelryType.Amethyst);
                                break;
                            }
                        case 3:
                            {
                                indication(JewelryType.Diamond);
                                break;
                            }
                    }
                }
            }
        }

        jewelryCount.CopyTo(oldJewelryCount, 0);
    }

    //すべての宝石を所有していない場合「true」を返す
    public bool notHaveJewelry()
    {
        if (!existEmerald && !existRuby && !existAmethyst && !existDiamond)
        {
            return true;
        }
        return false;
    }

    //宝石を所有ているとき空白状態を解除する
    public void activeJewelryImage()
    {
        if (!notHaveJewelry())
        {
            //どれか宝石を所有しているとき表示する
            centerImage.color = new Color(255, 255, 255, 255);
            centerCount.color = new Color(255, 255, 255, 255);
        }
        else
        {
            //すべて宝石を所有していない場合空白にする(透明化)
            centerImage.color = new Color(255, 255, 255, 0);
            centerCount.color = new Color(255, 255, 255, 0);
        }
    }

    //渡された宝石の種類を表示する
    public void indication(JewelryType jewelrySelect)
    {
        if (notHaveJewelry())
        {
            activeJewelryImage();
        }
        else
        {
            activeJewelryImage();
            //取捨選択した宝石を表示する
            switch (jewelrySelect)
            {
                case JewelryType.Emerald:
                    {
                        nowJewelry = JewelryType.Emerald;
                        centerImage.sprite = sprite[0];
                        break;
                    }
                case JewelryType.Ruby:
                    {
                        nowJewelry = JewelryType.Ruby;
                        centerImage.sprite = sprite[1];
                        break;
                    }
                case JewelryType.Amethyst:
                    {
                        nowJewelry = JewelryType.Amethyst;
                        centerImage.sprite = sprite[2];
                        break;
                    }
                case JewelryType.Diamond:
                    {
                        nowJewelry = JewelryType.Diamond;
                        centerImage.sprite = sprite[3];
                        break;
                    }
            }

            centerCount.text = GameSceneController.Instance.Player.JewelryPossessionStatus.GetJewelryCountByType(jewelrySelect).ToString();
            Debug.Log(jewelrySelect);
            Debug.Log(GameSceneController.Instance.Player.JewelryPossessionStatus.GetJewelryCountByType(jewelrySelect));
        }
    }

    //右回り(宝石を所有していない場合スキップ)
    public void rightChangeJewelry()
    {
        switch (nowJewelry)
        {
            case JewelryType.Emerald:
                {
                    if (existRuby) nowJewelry += 1;
                    else if (existAmethyst) nowJewelry += 2;
                    else if (existDiamond) nowJewelry += 3;
                    indication(nowJewelry);
                    break;
                }
            case JewelryType.Ruby:
                {
                    if (existAmethyst) nowJewelry += 1;
                    else if (existDiamond) nowJewelry += 2;
                    else if (existEmerald) nowJewelry -= 1;
                    indication(nowJewelry);
                    break;
                }
            case JewelryType.Amethyst:
                {
                    if (existDiamond) nowJewelry += 1;
                    else if (existEmerald) nowJewelry -= 2;
                    else if (existRuby) nowJewelry -= 1;
                    indication(nowJewelry);
                    break;
                }
            case JewelryType.Diamond:
                {
                    if (existEmerald) nowJewelry -= 3;
                    else if (existRuby) nowJewelry -= 2;
                    else if (existAmethyst) nowJewelry -= 1;
                    indication(nowJewelry);
                    break;
                }
            default:
                {
                    indication(nowJewelry);
                    break;
                }
        }
    }

    //左回り(宝石を所有していない場合スキップ)
    public void leftChangeJewelry()
    {
        switch (nowJewelry)
        {
            case JewelryType.Emerald:
                {
                    if (existDiamond) nowJewelry += 3;
                    else if (existAmethyst) nowJewelry += 2;
                    else if (existRuby) nowJewelry += 1;
                    indication(nowJewelry);
                    break;
                }
            case JewelryType.Ruby:
                {
                    if (existEmerald) nowJewelry -= 1;
                    else if (existDiamond) nowJewelry += 2;
                    else if (existAmethyst) nowJewelry += 1;
                    indication(nowJewelry);
                    break;
                }
            case JewelryType.Amethyst:
                {
                    if (existRuby) nowJewelry -= 1;
                    else if (existEmerald) nowJewelry -= 2;
                    else if (existDiamond) nowJewelry += 1;
                    indication(nowJewelry);
                    break;
                }
            case JewelryType.Diamond:
                {
                    if (existAmethyst) nowJewelry -= 1;
                    else if (existRuby) nowJewelry -= 2;
                    else if (existEmerald) nowJewelry -= 3;
                    indication(nowJewelry);
                    break;
                }
            default:
                {
                    indication(nowJewelry);
                    break;
                }
        }
    }

    private int[] ConvertJewelryCountByPossessionStatus(int[] jewelryCount)
    {
        JewelryPossessionStatus playerJewelry = GameSceneController.Instance.Player.JewelryPossessionStatus;
        jewelryCount[0] = playerJewelry.JewelryEmeraldHaveCount;
        jewelryCount[1] = playerJewelry.JewelryRubyHaveCount;
        jewelryCount[2] = playerJewelry.JewelryAmethystHaveCount;
        jewelryCount[3] = playerJewelry.JewelryDiamondHaveCount;

        return jewelryCount;
    }
}
