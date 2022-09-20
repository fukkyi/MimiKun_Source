using UnityEngine;

public class Test : GameMonoBehavior
{
    [SerializeField]
    private GameObject firstSelectObject = null;

    public override void OnStart()
    {
        AudioManager.Instance.PlayBGMWithCrossFade("Title", volume: 0.5f);
        AppManager.Instance.EventSystem.firstSelectedGameObject = firstSelectObject;
        AppManager.Instance.EventSystem.SetSelectedGameObject(firstSelectObject);
        base.OnStart();
    }

    //ゲーム終了:ボタンから呼び出す
    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
        Application.Quit();//ゲームプレイ終了
#endif
    }

    public void LoadGameScene()
    {
        AudioManager.Instance.PlaySE("Submit");
        SceneTransitionManager.Instance.TransitionByName("TutorialScene");
        AudioManager.Instance.StopCurrentBGMWithFade();
    }
}
