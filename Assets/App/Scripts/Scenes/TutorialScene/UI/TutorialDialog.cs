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
    /// チュートリアル用のダイアログを表示させる、表示できた時はTrueを返す
    /// </summary>
    /// <param name="dialogType"></param>
    public bool ShowDialog(DialogType dialogType)
    {
        int dialogTypeValue = (int)dialogType;
        // 表示済みのダイアログは表示しない
        if (dialogDisplayedFlag[dialogTypeValue]) return false;

        dialogCanvas.enabled = true;
        displayingDialogObj = dialogBodies[dialogTypeValue];
        displayingDialogObj.SetActive(true);

        // 表示済みとしてフラグを立てる
        dialogDisplayedFlag[dialogTypeValue] = true;

        return true;
    }

    /// <summary>
    /// ダイアログを閉じる、閉じた時はTrueを返す
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
    /// ダイアログ表示フラグを初期化する
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
