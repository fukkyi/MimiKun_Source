using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugManager : AutoGenerateManagerBase<DebugManager>
{
    public bool IsEnabledDebugMode { get; private set; } = false;
    public bool IsEnabledConsole { get; private set; } = false;
    public DebugConsole Console { get { return debugConsole; } }

    [SerializeField]
    private Canvas debugCanvas = null;
    [SerializeField]
    private List<DebuggerObject> debuggerObjects = null;
    [SerializeField]
    private DebugConsole debugConsole = null;

    private BasicInputAction inputAction = null;


    protected override void OnGenerated()
    {
        SetUpInputAction();
    }

    private void LateUpdate()
    {
        if (!IsEnabledDebugMode) return;

        debuggerObjects.ForEach((item) => { item.UpdateManage(); });
    }

    /// <summary>
    /// 入力処理の準備を行う
    /// </summary>
    private void SetUpInputAction()
    {
        inputAction = new BasicInputAction();
        inputAction.Debug.ToggleDebugMode.performed += (context) => { ToggleDebugMode(); };
        inputAction.Debug.EnableConsole.performed += (context) => { EnableConsole(); };
        inputAction.Debug.DisableConsole.performed += (context) => { DisableConsole(); };

        inputAction.Enable();
    }

    /// <summary>
    /// デバッグモードを切り替える
    /// </summary>
    private void ToggleDebugMode()
    {
        if (IsEnabledConsole) return;

        IsEnabledDebugMode = !IsEnabledDebugMode;
        debugCanvas.enabled = IsEnabledDebugMode;

        if (IsEnabledDebugMode)
        {
            debuggerObjects.ForEach((item) => { item.OnDebugModeEnabled(); });
        }
        else
        {
            debuggerObjects.ForEach((item) => { item.OnDebugModeDisable(); });
        }
    }

    private void EnableConsole()
    {
        if (!IsEnabledDebugMode) return;
        if (IsEnabledConsole) return;

        IsEnabledConsole = true;
        debugConsole.gameObject.SetActive(IsEnabledConsole);

        debugConsole.OnEnableConsole();
    }

    private void DisableConsole()
    {
        if (!IsEnabledConsole) return;

        IsEnabledConsole = false;
        debugConsole.gameObject.SetActive(IsEnabledConsole);

        debugConsole.OnDisableConsole();
    }
}
