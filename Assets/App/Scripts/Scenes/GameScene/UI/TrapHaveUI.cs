using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrapHaveUI : MonoBehaviour
{
    [SerializeField]
    private Image trapIconUI = null;

    /// <summary>
    /// �g���b�v�����A�C�R�����X�V����
    /// </summary>
    /// <param name="haveCount"></param>
    public void UpdateIcon(int haveCount)
    {
        bool iconEnable = haveCount > 0;
        trapIconUI.enabled = iconEnable;
    }
}
