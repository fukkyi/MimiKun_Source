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
    /// 特定の名前のシーンに遷移させる
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="loadWaitingTime"></param>
    public void TransitionByName(string sceneName, TransitionType transitionType = TransitionType.FadeInOut, float fadeOutTime = 1.0f, float fadeInTime = 1.0f)
    {
        if (IsTransiting) return;

        StartCoroutine(TransitionScene(sceneName, transitionType, fadeOutTime, fadeInTime));
    }

    /// <summary>
    /// 現在のアクティブなシーンを取得する
    /// </summary>
    /// <returns></returns>
    public Scene GetCurrentScene()
    {
        return SceneManager.GetActiveScene();
    }

    /// <summary>
    /// シーン遷移をさせるコルーチン
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="transitionType"></param>
    /// <returns></returns>
    private IEnumerator TransitionScene(string sceneName, TransitionType transitionType, float fadeOutTime, float fadeInTime)
    {
        IsTransiting = true;
        // フェードアウトさせる
        yield return StartCoroutine(FadeOut(fadeOutTime));
        // シーンをロードする
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(sceneName);
        do
        {
            yield return null;
        }
        while (!loadAsync.isDone);
        // 遷移先のシーンのコントローラーをキャッシュする
        SceneControllerManager.Instance.CacheSceneController();

        // フェードインさせる
        yield return StartCoroutine(FadeIn(fadeInTime));

        IsTransiting = false;
    }

    /// <summary>
    /// フェードアウトさせるコルーチン
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
    /// フェードインさせるコルーチン
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
    /// フェード用の画像のアルファ値をセットする
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