using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThanksSceneController : BaseSceneController
{
    private BasicInputAction thanksInputAction = null;

    // Start is called before the first frame update
    void Start()
    {
        InitThanksInputAction();
        StartCoroutine(WaitEnableNextButtonInput());
    }

    /// <summary>
    /// �T���N�X�V�[���pInputAction������������
    /// </summary>
    private void InitThanksInputAction()
    {
        thanksInputAction = new BasicInputAction();
        thanksInputAction.General.Next.performed += (context) => { TransitionTitleScene(); };
    }

    private IEnumerator WaitEnableNextButtonInput()
    {
        yield return new WaitForSeconds(3.0f);

        thanksInputAction.Enable();
    }

    /// <summary>
    /// ���̃V�[���֑J�ڂ�����
    /// </summary>
    public void TransitionTitleScene()
    {
        SceneTransitionManager.Instance.TransitionByName("TitleScene");

        thanksInputAction.Dispose();
    }
}
