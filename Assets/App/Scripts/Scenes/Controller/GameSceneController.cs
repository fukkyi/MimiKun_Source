using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : BaseSceneController
{
    public static GameSceneController Instance { get { return SceneControllerManager.Instance.GetSceneController<GameSceneController>(); } }
    public Player Player { get; private set; } = null;
    public Hero Hero { get; private set; } = null;
    public MasterDataJewelry JewelryData { get { return jewelryData; } }
    public GridPlatformerNavigator Navigator { get { return navigator; } }
    public GameSceneUICanvas GameSceneUICanvas { get { return gameSceneUICanvas; } }
    public Trap TrapObj { get { return trapObj; } }
    public bool StageCleared { get; private set; } = false;
    public bool GameOvered { get; private set; } = false;
    public bool Paused { get; private set; } = false;

    [SerializeField]
    protected GameSceneUICanvas gameSceneUICanvas = null;
    [SerializeField]
    protected PauseCanvas pauseCanvas = null;
    [SerializeField]
    protected MasterDataJewelry jewelryData = null;
    [SerializeField]
    protected MasterDataStage stageData = null;
    [SerializeField]
    protected ScoreModel scorePresenter = null;
    [SerializeField]
    protected Trap trapObj = null;
    [SerializeField]
    protected GridPlatformerNavigator navigator = null;

    protected BasicInputAction gameInputAction = null;

    protected void Awake()
    {
        FindPlayer();
        FindHero();
        SetCurrentSceneNameToModel();
        InitGameInputAction();
    }

    protected void InitGameInputAction()
    {
        gameInputAction = new BasicInputAction();
        gameInputAction.Game.Pause.performed += (context) => { TogglePause(); };

        gameInputAction.Enable();
    }

    protected void OnDestroy()
    {
        gameInputAction.Dispose();
    }

    /// <summary>
    /// �V�[������v���C���[��������
    /// </summary>
    /// <returns></returns>
    public Player FindPlayer()
    {
        Player = GameObject.FindWithTag(LayerTagUtil.TagNamePlayer)?.GetComponent<Player>();

        return Player;
    }

    /// <summary>
    /// �V�[������E�҂�������
    /// </summary>
    /// <returns></returns>
    public Hero FindHero()
    {
        Hero = GameObject.FindWithTag(LayerTagUtil.TagNameHero)?.GetComponent<Hero>();

        return Hero;
    }

    /// <summary>
    /// �v���C���[�̓��͂��󂯕t���Ă����Ԃ�
    /// </summary>
    /// <returns></returns>
    public virtual bool IsCanPlayerInput()
    {
        return !pauseCanvas.EnabledPause;
    }

    /// <summary>
    /// ����̍��W�Ɉ�ԋ߂��h���b�v���ꂽ��΂��擾����
    /// </summary>
    /// <returns></returns>
    public Jewelry FetchNearestDropedJewelryByPos(Vector3 position)
    {
        GameObject[] jewelryObjects = GameObject.FindGameObjectsWithTag(LayerTagUtil.TagNameJewelry);

        if (jewelryObjects.Length == 0) return null;

        GameObject nearestJewelryObj = null;
        float minDistance = float.MaxValue;
        foreach (GameObject jewelryObj in jewelryObjects)
        {
            Jewelry jewelry = jewelryObj.GetComponent<Jewelry>();
            // �h���b�v����ĂȂ���΂͑Ώۂɂ��Ȃ�
            if (!jewelry.IsDroped) continue;

            float jewelrySqrDistance = (jewelryObj.transform.position - position).sqrMagnitude;
            if (minDistance <= jewelrySqrDistance) continue;

            minDistance = jewelrySqrDistance;
            nearestJewelryObj = jewelryObj;
        }

        if (nearestJewelryObj == null) return null;

        return nearestJewelryObj.GetComponent<Jewelry>();
    }

    /// <summary>
    /// �X�e�[�W�N���A�ɂ���
    /// </summary>
    /// <param name="overrideNextStageName"></param>
    public void ClearCurrentStage(string overrideNextStageName = "")
    {
        if (StageCleared) return;

        string nextSceneName = overrideNextStageName == string.Empty ? stageData.nextStageSceneName : overrideNextStageName;

        StageCleared = true;
        SetScoreToModel();
        SetNextSceneToModel(nextSceneName);

        SceneTransitionManager.Instance.TransitionByName("ResultScene");
    }
    
    /// <summary>
    /// �Q�[���I�[�o�[�ɂ���
    /// </summary>
    public void GameOverCurrentStage()
    {
        if (GameOvered) return;

        GameOvered = true;
        AudioManager.Instance.StopCurrentBGMWithFade(2.0f);
        SceneTransitionManager.Instance.TransitionByName("GameOverScene", fadeOutTime: 2.0f);
    }

    /// <summary>
    /// ����̃X�R�A�����f���ɃZ�b�g����
    /// </summary>
    public void SetScoreToModel()
    {
        scorePresenter.jewelryStatus = Player.JewelryPossessionStatus;
        scorePresenter.playerHp = Player.CurrentHp;
        scorePresenter.stageData = stageData;
    }

    /// <summary>
    /// ���ɑJ�ڂ���V�[���̖��O�����f���ɃZ�b�g����
    /// </summary>
    /// <param name="nextSceneName"></param>
    public void SetNextSceneToModel(string nextSceneName)
    {
        scorePresenter.nextSceneName = nextSceneName;
    }

    /// <summary>
    /// ���݂̃X�e�[�W�V�[���̖��O�����f���ɃZ�b�g����
    /// </summary>
    public void SetCurrentSceneNameToModel()
    {
        scorePresenter.stageSceneName = SceneTransitionManager.Instance.GetCurrentScene().name;
    }

    /// <summary>
    /// �|�[�Y��Ԃ�؂�ւ���
    /// </summary>
    public void TogglePause()
    {
        if (GameOvered) return;
        if (StageCleared) return;

        if (pauseCanvas.EnabledPause)
        {
            pauseCanvas.DisablePause();
        }
        else
        {
            pauseCanvas.EnablePause();
        }
    }
}
