using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCanvas : MonoBehaviour
{
    public bool EnabledPause { get; private set; } = false;

    [SerializeField]
    private GameObject firstSelectObj = null;
    [SerializeField]
    private Canvas pauseUICanvas = null;

    private float beforeTimeScale = 0;
    private bool backedTitle = false;

    /// <summary>
    /// ポーズを有効にする
    /// </summary>
    public void EnablePause()
    {
        if (backedTitle) return;

        AppManager.Instance.EventSystem.firstSelectedGameObject = firstSelectObj;
        AppManager.Instance.EventSystem.SetSelectedGameObject(firstSelectObj);
        beforeTimeScale = Time.timeScale;

        Time.timeScale = 0;

        pauseUICanvas.enabled = true;
        EnabledPause = true;
    }

    /// <summary>
    /// ポーズを無効にする
    /// </summary>
    public void DisablePause()
    {
        if (backedTitle) return;

        AppManager.Instance.EventSystem.SetSelectedGameObject(null);

        Time.timeScale = beforeTimeScale;

        pauseUICanvas.enabled = false;
        EnabledPause = false;
    }

    /// <summary>
    /// タイトルに戻る
    /// </summary>
    public void BackToTitle()
    {
        if (backedTitle) return;

        SceneTransitionManager.Instance.TransitionByName("TitleScene");

        pauseUICanvas.enabled = false;
    }
}
