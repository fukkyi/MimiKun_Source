using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

public class DebugCommandExecuter
{
    private List<DebugCommand> commandList = new List<DebugCommand>();
    private string[] commandSeparators = new string[] { " " };
    private string[] argumentValueSeparators = new string[] { "=" };

    public void Init()
    {
        commandList.AddRange(GetAllCommands());
    }

    /// <summary>
    /// ���ׂẴR�}���h���擾����
    /// </summary>
    /// <returns></returns>
    protected DebugCommand[] GetAllCommands()
    {
        // DebugCommand�̔h���N���X�����ׂĎ擾����
        Type[] commandTypes = Assembly.GetAssembly(typeof(DebugCommand)).GetTypes().Where(type => {
            return type.IsSubclassOf(typeof(DebugCommand)) && !type.IsAbstract;
        }).ToArray();

        DebugCommand[] commands = new DebugCommand[commandTypes.Length];
        for(int i = 0; i < commands.Length; i++)
        {
            commands[i] = (DebugCommand)Activator.CreateInstance(commandTypes[i]);
        }

        return commands;
    }

    /// <summary>
    /// �R�}���h�����s����
    /// [�R�}���h��] [������=�����̒l] �Ƃ��������œn��
    /// ��: "test_command hoge=fuga"
    /// </summary>
    /// <param name="commandText"></param>
    public void ExecuteCommand(string commandText)
    {
        CommandContent? buildedCommandContent = BuildCommand(commandText);

        if (buildedCommandContent == null) return;

        CommandContent commandContent = (CommandContent)buildedCommandContent;

        commandContent.command.Execute(commandContent.arguments);
    }

    /// <summary>
    /// �R���\�[���̓��͂���R�}���h���e���r���h����
    /// </summary>
    /// <param name="commandText"></param>
    /// <returns></returns>
    private CommandContent? BuildCommand(string commandText)
    {
        CommandContent commandContent = new CommandContent();

        string[] splitedTexts = commandText.Split(commandSeparators, StringSplitOptions.RemoveEmptyEntries);

        // �󔒂݂̂̏ꍇ�ȂǂŔz��̗v�f���Ȃ��ꍇ�͏������s��Ȃ�
        if (splitedTexts.Length <= 0) return null;

        string commandName = splitedTexts.First();
        DebugCommand command = FindCommandByName(commandName);

        // �R�}���h��������Ȃ��ꍇ�͏������s��Ȃ�
        if (command == null)
        {
            DebugManager.Instance.Console.PutErrorLog($"[{commandName}] command is not found");
            return null;
        }

        DebugCommandArgument[] commandArguments = command.Arguments;
        List<DebugCommandArgument> inputArgumentList = new List<DebugCommandArgument>();
        // �R�}���h���ȊO�̗v�f�͈����Ƃ��Ď擾����
        string[] commandArgumentTexts = splitedTexts.Where((text) => { return text != commandName; }).ToArray();

        foreach (string argumentText in commandArgumentTexts)
        {
            DebugCommandArgument? buildedArgument = BuildCommandArgument(argumentText);
            // ���͂��������̏������Ⴄ�ꍇ�͏������s��Ȃ�
            if (buildedArgument == null)
            {
                DebugManager.Instance.Console.PutErrorLog($"[{argumentText}] invalid argument format");
                return null;
            }

            DebugCommandArgument inputArgument = (DebugCommandArgument)buildedArgument;
            // �������O�̈����̓X�L�b�v����
            if (inputArgumentList.Contains(inputArgument)) continue;

            bool existsCommandArgument = commandArguments.Where((commandArgument) => { return commandArgument.name == inputArgument.name; }).Count() > 0;
            // ���͂����������R�}���h�̈����ɑ��݂��Ȃ��Ȃ珈�����s��Ȃ�
            if (!existsCommandArgument)
            {
                DebugManager.Instance.Console.PutErrorLog($"[{inputArgument.name}] argument is not found");
                return null;
            }

            inputArgumentList.Add(inputArgument);
        }

        for(int i = 0; i < commandArguments.Length; i++)
        {
            DebugCommandArgument commandArgument = commandArguments[i];
            bool existsInputArgument = inputArgumentList.Where((inputArgument) => { return commandArgument.name == inputArgument.name; }).Count() > 0;

            if (existsInputArgument)
            {
                DebugCommandArgument matchArgument = inputArgumentList.FirstOrDefault((inputArgument) => { return inputArgument.name == commandArgument.name; });
                commandArgument.value = matchArgument.value;
            }
            else
            {
                // �K�{�Ȉ����̒l��������Ȃ������ꍇ�͏������s��Ȃ�
                if (commandArgument.IsRequired())
                {
                    DebugManager.Instance.Console.PutErrorLog($"[{commandArgument.name}] is requires argument");
                    return null;
                }

                commandArgument.SetValueToDefault();
            }

            commandArguments[i] = commandArgument;
        }

        commandContent.command = command;
        commandContent.arguments = commandArguments;

        return commandContent;
    }

    /// <summary>
    /// ���O����R�}���h���擾����
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private DebugCommand FindCommandByName(string name)
    {
        return commandList.FirstOrDefault((command) => { return command.CommandName == name; });
    }

    /// <summary>
    /// �����񂩂�R�}���h�p�̈������r���h����
    /// </summary>
    /// <param name="argumentText"></param>
    /// <returns></returns>
    private DebugCommandArgument? BuildCommandArgument(string argumentText)
    {
        // �����p�̕������ [���O, �l] �ŕ�����
        int splitCount = 2;
        string[] splitedArgumentText = argumentText.Split(argumentValueSeparators, splitCount, StringSplitOptions.RemoveEmptyEntries);
        DebugCommandArgument argument = new DebugCommandArgument();

        if (splitedArgumentText.Length != splitCount) return null;

        argument.name = splitedArgumentText.First();
        argument.value = splitedArgumentText.Last();

        return argument;
    }

    private struct CommandContent
    {
        public DebugCommand command;
        public DebugCommandArgument[] arguments;
    }
}
