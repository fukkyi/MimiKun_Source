using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
  fileName = "JewelryMasterData",
  menuName = "ScriptableObject/MasterDataJewelry")
]
public class MasterDataJewelry : ScriptableObject
{
    public Jewelry emeraldJewelryObj = null;
    public Jewelry rubyJewelryObj = null;
    public Jewelry amethystJewelryObj = null;
    public Jewelry diamondJewelryObj = null;

    public int emeraldJewelryScore = 100;
    public int rubyJewelryScore = 200;
    public int amethystJewelryScore = 500;
    public int diamondJewelryScore = 1000;

    public float emeraldJewelryBindTime = 0.5f;
    public float rubyJewelryBindTime = 1.0f;
    public float amethystJewelryBindTime = 1.5f;
    public float diamondJewelryBindTime = 2.0f;

    /// <summary>
    /// 指定した種類の宝石オブジェクトを取得する
    /// </summary>
    /// <param name="jewelryType"></param>
    /// <returns></returns>
    public Jewelry GetJewelryObjByType(JewelryType jewelryType)
    {
        Jewelry jewelry = null;
        switch(jewelryType)
        {
            case JewelryType.Emerald:
                jewelry = emeraldJewelryObj;
                break;

            case JewelryType.Ruby:
                jewelry = rubyJewelryObj;
                break;

            case JewelryType.Amethyst:
                jewelry = amethystJewelryObj;
                break;

            case JewelryType.Diamond:
                jewelry = diamondJewelryObj;
                break;
        }

        return jewelry;
    }

    /// <summary>
    /// 指定した種類の宝石スコアを取得する
    /// </summary>
    /// <param name="jewelryType"></param>
    /// <returns></returns>
    public int GetScoreByType(JewelryType jewelryType)
    {
        int score = 0;
        switch (jewelryType)
        {
            case JewelryType.Emerald:
                score = emeraldJewelryScore;
                break;

            case JewelryType.Ruby:
                score = rubyJewelryScore;
                break;

            case JewelryType.Amethyst:
                score = amethystJewelryScore;
                break;

            case JewelryType.Diamond:
                score = diamondJewelryScore;
                break;
        }

        return score;
    }

    /// <summary>
    /// 指定した種類の拘束時間を取得する
    /// </summary>
    /// <param name="jewelryType"></param>
    /// <returns></returns>
    public float GetBindTimeByType(JewelryType jewelryType)
    {
        float bindTime = 0;
        switch (jewelryType)
        {
            case JewelryType.Emerald:
                bindTime = emeraldJewelryBindTime;
                break;

            case JewelryType.Ruby:
                bindTime = rubyJewelryBindTime;
                break;

            case JewelryType.Amethyst:
                bindTime = amethystJewelryBindTime;
                break;

            case JewelryType.Diamond:
                bindTime = diamondJewelryBindTime;
                break;
        }

        return bindTime;
    }
}

public enum JewelryType
{
    Emerald,
    Ruby,
    Amethyst,
    Diamond
}