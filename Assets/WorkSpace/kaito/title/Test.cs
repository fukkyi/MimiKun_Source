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

    //�Q�[���I��:�{�^������Ăяo��
    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
        Application.Quit();//�Q�[���v���C�I��
#endif
    }

    public void LoadGameScene()
    {
        AudioManager.Instance.PlaySE("Submit");
        SceneTransitionManager.Instance.TransitionByName("TutorialScene");
        AudioManager.Instance.StopCurrentBGMWithFade();
    }
}
