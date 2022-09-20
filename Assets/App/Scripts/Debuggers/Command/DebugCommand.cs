using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DebugCommand
{
    /// <summary>
    /// �R�}���h�����s���邽�߂̃R�}���h��
    /// </summary>
    public abstract string CommandName { get; }

    /// <summary>
    /// �R�}���h�̈���
    /// </summary>
    public abstract DebugCommandArgument[] Arguments { get; }

    /// <summary>
    /// �R�}���h���s���̏���
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
/// �f�o�b�O�R�}���h�p�̈����̍\����
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
    /// ���̈������K�{�ł��邩
    /// </summary>
    /// <returns></returns>
    public bool IsRequired()
    {
        return defaultValue == string.Empty;
    }

    /// <summary>
    /// �l���f�t�H���g�l�ɐݒ肷��
    /// </summary>
    public void SetValueToDefault()
    {
        value = defaultValue;
    }
}
