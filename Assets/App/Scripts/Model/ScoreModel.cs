using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
  fileName = "GameScoreData",
  menuName = "ScriptableObject/ScoreModel")
]
public class ScoreModel : ScriptableObject, ISerializationCallbackReceiver
{
    [NonSerialized]
    public JewelryPossessionStatus jewelryStatus = new JewelryPossessionStatus();
    [NonSerialized]
    public int playerHp = 0;
    [NonSerialized]
    public string stageSceneName = "";
    [NonSerialized]
    public string nextSceneName = "";
    [NonSerialized]
    public MasterDataStage stageData = null;

    public void OnAfterDeserialize()
    {
        // Editor上では実行終了時に値が戻らないため、手動で初期化する
        jewelryStatus = new JewelryPossessionStatus();
        playerHp = 0;
        nextSceneName = "";
        stageData = null;
    }

    public void OnBeforeSerialize() { }
}
