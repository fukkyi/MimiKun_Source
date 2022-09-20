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
    /// ���U���g�V�[���pInputAction������������
    /// </summary>
    private void InitResultInputAction()
    {
        resultInputAction = new BasicInputAction();
        resultInputAction.General.Next.performed += (context) => { TransitionNextScene(); };
    }

    /// <summary>
    /// �X�R�A���v�Z����
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

        // TODO: �}�X�^�[�f�[�^�Ɏ�������
        string rankTitle = string.Empty;
        switch(stageRank)
        {
            case StageRank.S:
                rankTitle = "�~�~�b����";
                break;
            case StageRank.A:
                rankTitle = "�~�~�b��y��";
                break;
            case StageRank.B:
                rankTitle = "�~�~�b����";
                break;
            case StageRank.C:
                rankTitle = "�~�~�b�����";
                break;
        }
        rankTitleText.SetText(rankTitle);
    }
    
    /// <summary>
    /// ���֐i�ރ{�^���������҂��ėL��������
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitEnableNextButtonInput()
    {
        yield return new WaitForSeconds(2.0f);

        resultInputAction.Enable();
    }

    /// <summary>
    /// ���̃V�[���֑J�ڂ�����
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
