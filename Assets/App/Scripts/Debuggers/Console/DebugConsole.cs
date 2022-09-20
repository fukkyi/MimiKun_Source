using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugConsole : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField = null;
    [SerializeField]
    private DebugConsoleLogger consoleLogger = null;

    private DebugCommandExecuter commandExecuter = new DebugCommandExecuter();
    private List<string> commandLogList = new List<string>();
    private BasicInputAction inputAction = null;

    private int displayCommandLogCount = 0;

    private void Awake()
    {
        Init();
        commandExecuter.Init();
    }

    /// <summary>
    /// 初期化処理を行う
    /// </summary>
    private void Init()
    {
        SetInputAction();
    }

    private void SetInputAction()
    {
        inputAction = new BasicInputAction();
        inputAction.Debug.ConsoleLogNext.performed += (context) => { SetCommandConsoleLogNext(); };
        inputAction.Debug.ConsoleLogPrev.performed += (context) => { SetCommandConsoleLogPrev(); };
    }

    private void SetCommandConsoleLogPrev()
    {
        int maxLogIndex = commandLogList.Count - 1;
        if (displayCommandLogCount >= maxLogIndex) return;

        displayCommandLogCount++;

        StartCoroutine(SetInputTextByCommandLog());
    }

    private void SetCommandConsoleLogNext()
    {
        int minLogIndex = 0;
        if (displayCommandLogCount <= minLogIndex) return;

        displayCommandLogCount--;

        StartCoroutine(SetInputTextByCommandLog());
    }

    private IEnumerator SetInputTextByCommandLog()
    {
        string commandLogText = commandLogList[displayCommandLogCount];

        inputField.text = commandLogText;
        yield return null;
        // 次のフレームで処理しないとキャレットが移動しない
        inputField.caretPosition = commandLogText.Length;
    }

    private void PutLog(DebugLogStatus status, string content)
    {
        consoleLogger.SetLogScrollPositionToLastLog();
        consoleLogger.PutLog(status, content);
    }

    private void PutCommandLog(string commandText)
    {
        commandLogList.Add(commandText);
        displayCommandLogCount = commandLogList.Count;
    }

    public void OnEnableConsole()
    {
        consoleLogger.SetLogScrollPositionToLastLog();
        inputField.ActivateInputField();

        displayCommandLogCount = commandLogList.Count;
        inputAction.Enable();
    }

    public void OnDisableConsole()
    {
        inputField.text = string.Empty;
        inputAction.Disable();
    }

    public void OnEnteredCommand(string inputText)
    {
        if (!DebugManager.Instance.IsEnabledConsole) return;

        if (inputText != string.Empty)
        {
            consoleLogger.PutLog(DebugLogStatus.Command, inputText);
            commandExecuter.ExecuteCommand(inputText);
            inputField.text = string.Empty;

            PutCommandLog(inputText);
        }

        inputField.ActivateInputField();
    }

    public void PutNoticeLog(string content)
    {
        PutLog(DebugLogStatus.Notice, content);
    }

    public void PutOKLog(string content)
    {
        PutLog(DebugLogStatus.OK, content);
    }

    public void PutCautionLog(string content)
    {
        PutLog(DebugLogStatus.Caution, content);
    }

    public void PutErrorLog(string content)
    {
        PutLog(DebugLogStatus.Error, content);
    }
}
