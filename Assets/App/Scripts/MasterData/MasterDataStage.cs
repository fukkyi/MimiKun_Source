using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
  fileName = "StageMasterData",
  menuName = "ScriptableObject/MasterDataStage")
]
public class MasterDataStage : ScriptableObject
{
    public int RankScoreBorderS;
    public int RankScoreBorderA;
    public int RankScoreBorderB;

    public string nextStageSceneName;

    /// <summary>
    /// スコアからランクを計算する
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public StageRank CalcStageRank(int score)
    {
        StageRank rank = StageRank.None;
        if (score >= RankScoreBorderS)
        {
            rank = StageRank.S;
        }
        else if (score >= RankScoreBorderA)
        {
            rank = StageRank.A;
        }
        else if (score >= RankScoreBorderB)
        {
            rank = StageRank.B;
        }
        else
        {
            rank = StageRank.C;
        }

        return rank;
    }
}
