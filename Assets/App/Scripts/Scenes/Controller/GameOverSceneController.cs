using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSceneController : BaseSceneController
{
    [SerializeField]
    private ScoreModel scoreModel = null;
    [SerializeField]
    private GameObject firstSelectedObj = null;

    private void Start()
    {
        AppManager.Instance.EventSystem.firstSelectedGameObject = firstSelectedObj;
        AppManager.Instance.EventSystem.SetSelectedGameObject(firstSelectedObj);
        AudioManager.Instance.PlayBGMWithFade("Gameover");
    }

    /// <summary>
    /// 前のステージのシーンに遷移する
    /// </summary>
    public void TransitionToBeforeStage()
    {
        SceneTransitionManager.Instance.TransitionByName(scoreModel.stageSceneName);
        AudioManager.Instance.StopCurrentBGMWithFade();
    }

    /// <summary>
    /// タイトルシーンに遷移する
    /// </summary>
    public void TransitionToTitle()
    {
        SceneTransitionManager.Instance.TransitionByName("TitleScene");
        AudioManager.Instance.StopCurrentBGMWithFade();
    }
}
