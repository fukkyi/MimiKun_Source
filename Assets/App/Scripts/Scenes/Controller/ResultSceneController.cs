using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultSceneController : BaseSceneController
{
    public static ResultSceneController Instance { get { return SceneControllerManager.Instance.GetSceneController<ResultSceneController>(); } }

    [SerializeField]
    private MasterDataJewelry masterDataJewelry;
    [SerializeField]
    private ScoreModel scoreModel;

    [SerializeField]
    private TextMeshProUGUI jewelryScoreText = null;
    [SerializeField]
    private TextMeshProUGUI lifeScoreText = null;
    [SerializeField]
    private TextMeshProUGUI totalScoreText = null;
    [SerializeField]
    private TextMeshProUGUI rankText = null;
    [SerializeField]
    private TextMeshProUGUI rankTitleText = null;

    [SerializeField]
    private int onceLifeScore = 1000;

    private BasicInputAction resultInputAction = null;

    private int jewelryScore = 0;
    private int lifeScore = 0;
    private int totalScore = 0;
    private StageRank stageRank = StageRank.None;

    private void Start()
    {
        InitResultInputAction();
        CalcScore();
        StartCoroutine(WaitEnableNextButtonInput());
    }

    /// <summary>
    /// リザルトシーン用InputActionを初期化する
    /// </summary>
    private void InitResultInputAction()
    {
        resultInputAction = new BasicInputAction();
        resultInputAction.General.Next.performed += (context) => { TransitionNextScene(); };
    }

    /// <summary>
    /// スコアを計算する
    /// </summary>
    private void CalcScore()
    {
        jewelryScore += scoreModel.jewelryStatus.JewelryEmeraldHaveCount * masterDataJewelry.emeraldJewelryScore;
        jewelryScore += scoreModel.jewelryStatus.JewelryRubyHaveCount * masterDataJewelry.rubyJewelryScore;
        jewelryScore += scoreModel.jewelryStatus.JewelryAmethystHaveCount * masterDataJewelry.amethystJewelryScore;
        jewelryScore += scoreModel.jewelryStatus.JewelryDiamondHaveCount * masterDataJewelry.diamondJewelryScore;

        lifeScore = scoreModel.playerHp * onceLifeScore;

        totalScore = jewelryScore + lifeScore;

        stageRank = scoreModel.stageData.CalcStageRank(totalScore);

        jewelryScoreText.SetText("{0} pts", jewelryScore);
        lifeScoreText.SetText("{0} pts", lifeScore);
        totalScoreText.SetText("{0} pts", totalScore);
        rankText.SetText(System.Enum.GetName(typeof(StageRank), stageRank));

        // TODO: マスターデータに持たせる
        string rankTitle = string.Empty;
        switch(stageRank)
        {
            case StageRank.S:
                rankTitle = "ミミッさん";
                break;
            case StageRank.A:
                rankTitle = "ミミッ先輩級";
                break;
            case StageRank.B:
                rankTitle = "ミミッくん級";
                break;
            case StageRank.C:
                rankTitle = "ミミッちゃん級";
                break;
        }
        rankTitleText.SetText(rankTitle);
    }
    
    /// <summary>
    /// 次へ進むボタンを少し待って有効化する
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitEnableNextButtonInput()
    {
        yield return new WaitForSeconds(2.0f);

        resultInputAction.Enable();
    }

    /// <summary>
    /// 次のシーンへ遷移させる
    /// </summary>
    public void TransitionNextScene()
    {
        SceneTransitionManager.Instance.TransitionByName(scoreModel.nextSceneName);

        resultInputAction.Dispose();
    }
}

public enum StageRank
{
    None,
    C,
    B,
    A,
    S,
}
