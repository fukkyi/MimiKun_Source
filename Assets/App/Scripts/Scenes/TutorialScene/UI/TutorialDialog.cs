using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDialog : MonoBehaviour
{
    public bool DisplayingDialog { get; private set; } = false;

    [SerializeField]
    private GameObject[] dialogBodies = null;
    [SerializeField]
    private Canvas dialogCanvas = null;

    private GameObject displayingDialogObj = null;
    private Dictionary<int, bool> dialogDisplayedFlag = new Dictionary<int, bool>();

    private void Awake()
    {
        InitDialogDisplayedFlag();
    }

    /// <summary>
    /// �`���[�g���A���p�̃_�C�A���O��\��������A�\���ł�������True��Ԃ�
    /// </summary>
    /// <param name="dialogType"></param>
    public bool ShowDialog(DialogType dialogType)
    {
        int dialogTypeValue = (int)dialogType;
        // �\���ς݂̃_�C�A���O�͕\�����Ȃ�
        if (dialogDisplayedFlag[dialogTypeValue]) return false;

        dialogCanvas.enabled = true;
        displayingDialogObj = dialogBodies[dialogTypeValue];
        displayingDialogObj.SetActive(true);

        // �\���ς݂Ƃ��ăt���O�𗧂Ă�
        dialogDisplayedFlag[dialogTypeValue] = true;

        return true;
    }

    /// <summary>
    /// �_�C�A���O�����A��������True��Ԃ�
    /// </summary>
    public bool HideDialog()
    {
        if (displayingDialogObj == null) return false;

        dialogCanvas.enabled = false;
        displayingDialogObj.SetActive(false);
        displayingDialogObj = null;

        return true;
    }

    /// <summary>
    /// �_�C�A���O�\���t���O������������
    /// </summary>
    private void InitDialogDisplayedFlag()
    {
        foreach(DialogType dialogType in Enum.GetValues(typeof(DialogType)))
        {
            dialogDisplayedFlag.Add((int)dialogType, false);
        }
    }

    public enum DialogType
    {
        Move,
        Jump,
        Jewelry,
        JewelryPick,
        Hero,
        HeroJewelry,
        HeroTrap,
    }
}
