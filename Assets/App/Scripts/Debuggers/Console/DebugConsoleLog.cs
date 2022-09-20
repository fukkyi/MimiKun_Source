using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugConsoleLog : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timeText = null;
    [SerializeField]
    private TextMeshProUGUI statusText = null;
    [SerializeField]
    private TextMeshProUGUI contentText = null;

    [SerializeField]
    private Color defaultLogColor = new Color();
    [SerializeField]
    private Color okLogColor = new Color();
    [SerializeField]
    private Color cautionLogColor = new Color();
    [SerializeField]
    private Color errorLogColor = new Color();
    
    /// <summary>
    /// テキストの文字をセットする
    /// </summary>
    /// <param name="time"></param>
    /// <param name="status"></param>
    /// <param name="content"></param>
    public void SetLog(TimeSpan time, DebugLogStatus status, string content)
    {
        timeText.SetText($"[{time}]");
        statusText.SetText($"[{status}]");
        contentText.SetText(content);

        switch (status)
        {
            case DebugLogStatus.OK:
                statusText.color = okLogColor;
                break;
            case DebugLogStatus.Caution:
                statusText.color = cautionLogColor;
                break;
            case DebugLogStatus.Error:
                statusText.color = errorLogColor;
                break;
            default:
                statusText.color = defaultLogColor;
                break;
        }

        gameObject.SetActive(true);
    }

    public void Init()
    {
        gameObject.SetActive(false);
    }
}
