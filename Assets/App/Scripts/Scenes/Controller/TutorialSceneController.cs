using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSceneController : GameSceneController
{
    public static new TutorialSceneController Instance { get { return SceneControllerManager.Instance.GetSceneController<TutorialSceneController>(); } }

    [SerializeField]
    private TutorialDialog tutorialDialog = null;
    [SerializeField]
    private slimeMove jewelryTutorialSlime = null;
    [SerializeField]
    private Jewelry[] tutorialJewelries = null;

    private BasicInputAction tutorialInputAction = null;

    private bool displayedDialog = false;

    private new void Awake()
    {
        base.Awake();

        tutorialInputAction = new BasicInputAction();
        tutorialInputAction.General.Next.performed += (context) => { HideTutorialDialog(); };
        tutorialInputAction.Enable();

        Hero.gameObject.SetActive(false);
    }

    private void Start()
    {
        ShowTutorialDialog(TutorialDialog.DialogType.Move);
    }

    private void Update()
    {
        UpdateJewelryTutorialSlime();
    }

    public override bool IsCanPlayerInput()
    {
        return base.IsCanPlayerInput() && !displayedDialog;
    }

    /// <summary>
    /// �`���[�g���A���_�C�A���O��\��������
    /// </summary>
    /// <param name="dialogType"></param>
    public void ShowTutorialDialog(TutorialDialog.DialogType dialogType)
    {
        if (displayedDialog) return;

        bool displaySuccessed = tutorialDialog.ShowDialog(dialogType);

        if (!displaySuccessed) return;

        if (dialogType == TutorialDialog.DialogType.Hero)
        {
            Hero.gameObject.SetActive(true);
        }

        Time.timeScale = 0f;
        displayedDialog = true;
    }

    /// <summary>
    /// �`���[�g���A���_�C�A���O�����
    /// </summary>
    public void HideTutorialDialog()
    {
        if (!displayedDialog) return;
        if (pauseCanvas.EnabledPause) return;

        bool hideSuccessed = tutorialDialog.HideDialog();

        if (!hideSuccessed) return;

        Time.timeScale = 1.0f;
        // TODO: �_�C�A���O�������Ƀv���C���[�̏����������Ă��܂����߁A�b��I�ȑΉ�
        StartCoroutine(WaitTurnOffDisplayedDialogFlag());
    }

    /// <summary>
    /// 1�t���[���҂���displayedDialog�t���O���I�t�ɂ���
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitTurnOffDisplayedDialogFlag()
    {
        yield return null;
        displayedDialog = false;
    }

    /// <summary>
    /// ��΃`���[�g���A���p�X���C�����X�V����
    /// TODO: ��Ύ擾���C�x���g���𔭉΂���d�g�݂�����ČĂяo������
    /// </summary>
    private void UpdateJewelryTutorialSlime()
    {
        if (jewelryTutorialSlime == null) return;

        bool allCollected = false;
        foreach(Jewelry jewelry in tutorialJewelries)
        {
            if (jewelry != null)
            {
                allCollected = true;
                break;
            }
        }

        if (allCollected)
        {
            Destroy(jewelryTutorialSlime.gameObject);
        }
    }
}
