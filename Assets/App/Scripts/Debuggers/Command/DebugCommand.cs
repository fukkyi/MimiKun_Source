using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DebugCommand
{
    /// <summary>
    /// コマンドを実行するためのコマンド名
    /// </summary>
    public abstract string CommandName { get; }

    /// <summary>
    /// コマンドの引数
    /// </summary>
    public abstract DebugCommandArgument[] Arguments { get; }

    /// <summary>
    /// コマンド実行時の処理
    /// </summary>
    public abstract bool Execute(DebugCommandArgument[] arguments);

    protected string GetArgumentValueByName(DebugCommandArgument[] arguments, string name)
    {
        foreach(DebugCommandArgument argument in arguments)
        {
            if (argument.name != name) continue;

            return argument.value;
        }

        return string.Empty;
    }
}

/// <summary>
/// デバッグコマンド用の引数の構造体
/// </summary>
public struct DebugCommandArgument
{
    public string name;
    public string value;
    public string defaultValue;

    public DebugCommandArgument(string name, string defaultValue = "")
    {
        this.name = name;
        this.value = "";
        this.defaultValue = defaultValue;
    }

    /// <summary>
    /// この引数が必須であるか
    /// </summary>
    /// <returns></returns>
    public bool IsRequired()
    {
        return defaultValue == string.Empty;
    }

    /// <summary>
    /// 値をデフォルト値に設定する
    /// </summary>
    public void SetValueToDefault()
    {
        value = defaultValue;
    }
}
