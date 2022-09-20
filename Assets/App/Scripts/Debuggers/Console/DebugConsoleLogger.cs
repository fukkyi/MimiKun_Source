using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsoleLogger : MonoBehaviour
{
    [SerializeField]
    private DebugConsoleLog logTemplate = null;
    [SerializeField]
    private RectTransform logParentTrans = null;
    [SerializeField]
    private ScrollRect logScrollRect = null;
    [SerializeField]
    private int maxLogCount = 50;

    private List<DebugConsoleLog> logs = new List<DebugConsoleLog>();

    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// ���������s��
    /// </summary>
    private void Init()
    {
        for (int i = 0; i < maxLogCount; i++)
        {
            DebugConsoleLog generatedLog = Instantiate(logTemplate, logParentTrans);
            generatedLog.Init();
            logs.Add(generatedLog);
        }
    }

    /// <summary>
    /// ���O���o�͂���
    /// </summary>
    /// <param name="status"></param>
    /// <param name="content"></param>
    public void PutLog(DebugLogStatus status, string content)
    {
        DebugConsoleLog disableConsoleLog = logs.FirstOrDefault(log => log.gameObject.activeSelf == false);

        if (disableConsoleLog == null)
        {
            int lastChildIndex = logParentTrans.childCount - 1;
            disableConsoleLog = logs.FirstOrDefault(log => log.transform.GetSiblingIndex() == lastChildIndex);
        }

        disableConsoleLog.transform.SetAsFirstSibling();

        TimeSpan nowTime = DateTime.Now.TimeOfDay;
        nowTime = new TimeSpan(nowTime.Hours, nowTime.Minutes, nowTime.Seconds);

        disableConsoleLog.SetLog(nowTime, status, content);
    }

    /// <summary>
    /// ���O����������
    /// </summary>
    public void ClearLog()
    {
        logs.ForEach(log => log.Init());
    }

    /// <summary>
    /// �ŐV�̃��O�̈ʒu�Ƀ��O�̃X�N���[���ʒu�����킹��
    /// </summary>
    public void SetLogScrollPositionToLastLog()
    {
        logScrollRect.verticalNormalizedPosition = 0f;
    }
}

/// <summary>
/// ���O�X�e�[�^�X�̎��
/// </summary>
public enum DebugLogStatus
{
    None,
    Command,
    OK,
    Notice,
    Caution,
    Error,
}
