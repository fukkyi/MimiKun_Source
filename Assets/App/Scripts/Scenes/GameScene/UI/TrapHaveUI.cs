using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrapHaveUI : MonoBehaviour
{
    [SerializeField]
    private Image trapIconUI = null;

    /// <summary>
    /// トラップ所持アイコンを更新する
    /// </summary>
    /// <param name="haveCount"></param>
    public void UpdateIcon(int haveCount)
    {
        bool iconEnable = haveCount > 0;
        trapIconUI.enabled = iconEnable;
    }
}
