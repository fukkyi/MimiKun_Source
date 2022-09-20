using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : AutoGenerateManagerBase<SceneTransitionManager>
{
    public readonly static string TitleSceneName = "Title";
    public readonly static string TutorialSceneNama = "Tutorial";

    public bool IsTransiting { get; private set; } = false;

    [SerializeField]
    private Image fadeImage = null;

    private float currentFadeTime = 0;

    /// <summary>
    /// ����̖��O�̃V�[���ɑJ�ڂ�����
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="loadWaitingTime"></param>
    public void TransitionByName(string sceneName, TransitionType transitionType = TransitionType.FadeInOut, float fadeOutTime = 1.0f, float fadeInTime = 1.0f)
    {
        if (IsTransiting) return;

        StartCoroutine(TransitionScene(sceneName, transitionType, fadeOutTime, fadeInTime));
    }

    /// <summary>
    /// ���݂̃A�N�e�B�u�ȃV�[�����擾����
    /// </summary>
    /// <returns></returns>
    public Scene GetCurrentScene()
    {
        return SceneManager.GetActiveScene();
    }

    /// <summary>
    /// �V�[���J�ڂ�������R���[�`��
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="transitionType"></param>
    /// <returns></returns>
    private IEnumerator TransitionScene(string sceneName, TransitionType transitionType, float fadeOutTime, float fadeInTime)
    {
        IsTransiting = true;
        // �t�F�[�h�A�E�g������
        yield return StartCoroutine(FadeOut(fadeOutTime));
        // �V�[�������[�h����
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(sceneName);
        do
        {
            yield return null;
        }
        while (!loadAsync.isDone);
        // �J�ڐ�̃V�[���̃R���g���[���[���L���b�V������
        SceneControllerManager.Instance.CacheSceneController();

        // �t�F�[�h�C��������
        yield return StartCoroutine(FadeIn(fadeInTime));

        IsTransiting = false;
    }

    /// <summary>
    /// �t�F�[�h�A�E�g������R���[�`��
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeOut(float fadeTime)
    {
        currentFadeTime = 0;
        fadeImage.raycastTarget = true;
        while(currentFadeTime < fadeTime)
        {
            currentFadeTime = Mathf.Clamp(currentFadeTime + Time.unscaledDeltaTime, 0, fadeTime);
            SetAlphaImage(currentFadeTime / fadeTime);

            yield return null;
        }
    }

    /// <summary>
    /// �t�F�[�h�C��������R���[�`��
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeIn(float fadeTime)
    {
        currentFadeTime = 0;
        fadeImage.raycastTarget = false;
        while (currentFadeTime < fadeTime)
        {
            currentFadeTime = Mathf.Clamp(currentFadeTime + Time.unscaledDeltaTime, 0, fadeTime);
            SetAlphaImage(1 - (currentFadeTime / fadeTime));

            yield return null;
        }
    }

    /// <summary>
    /// �t�F�[�h�p�̉摜�̃A���t�@�l���Z�b�g����
    /// </summary>
    /// <param name="alpha"></param>
    private void SetAlphaImage(float alpha)
    {
        Color fadeColor = Color.black;
        fadeColor.a = alpha;

        fadeImage.color = fadeColor;
    }
}

public enum TransitionType
{
    FadeInOut,
}