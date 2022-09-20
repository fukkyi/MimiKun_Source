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
        // Editor��ł͎��s�I�����ɒl���߂�Ȃ����߁A�蓮�ŏ���������
        jewelryStatus = new JewelryPossessionStatus();
        playerHp = 0;
        nextSceneName = "";
        stageData = null;
    }

    public void OnBeforeSerialize() { }
}
