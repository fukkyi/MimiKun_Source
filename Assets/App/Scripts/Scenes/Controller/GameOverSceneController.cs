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
    /// �O�̃X�e�[�W�̃V�[���ɑJ�ڂ���
    /// </summary>
    public void TransitionToBeforeStage()
    {
        SceneTransitionManager.Instance.TransitionByName(scoreModel.stageSceneName);
        AudioManager.Instance.StopCurrentBGMWithFade();
    }

    /// <summary>
    /// �^�C�g���V�[���ɑJ�ڂ���
    /// </summary>
    public void TransitionToTitle()
    {
        SceneTransitionManager.Instance.TransitionByName("TitleScene");
        AudioManager.Instance.StopCurrentBGMWithFade();
    }
}
