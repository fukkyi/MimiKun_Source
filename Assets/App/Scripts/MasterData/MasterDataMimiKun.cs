using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
  fileName = "MimiKunMasterData",
  menuName = "ScriptableObject/MasterDataMimiKun")
]
public class MasterDataMimiKun : ScriptableObject
{
    public MimiKunMobilityTable mobilityTable = new MimiKunMobilityTable();
}

[Serializable]
public class MimiKunMobility
{
    public int jewelryCount = 0;
    public float scale = 1.0f;
    public float moveSpeed = 8.0f;
    public float jumpHeight = 5.5f;
}

[Serializable]
public class MimiKunMobilityTable
{
    public MimiKunMobility[] Mobilities;

    /// <summary>
    /// 宝石の所持数から体の大きさをテーブルから選択する
    /// </summary>
    /// <param name="jewelryCount"></param>
    /// <returns></returns>
    public MimiKunMobility SelectMobilityByJewelryCount(int jewelryCount)
    {
        MimiKunMobility selectMobility = new MimiKunMobility();
        int currentJewelryCount = 0;

        foreach(MimiKunMobility mobilities in Mobilities)
        {
            if (mobilities.jewelryCount > jewelryCount) continue;
            if (mobilities.jewelryCount < currentJewelryCount) continue;

            selectMobility = mobilities;
            currentJewelryCount = Mathf.Max(currentJewelryCount, mobilities.jewelryCount);
        }

        return selectMobility;
    }
}