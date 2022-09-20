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
    /// すべてのコマンドを取得する
    /// </summary>
    /// <returns></returns>
    protected DebugCommand[] GetAllCommands()
    {
        // DebugCommandの派生クラスをすべて取得する
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
    /// コマンドを実行する
    /// [コマンド名] [引数名=引数の値] という書式で渡す
    /// 例: "test_command hoge=fuga"
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
    /// コンソールの入力からコマンド内容をビルドする
    /// </summary>
    /// <param name="commandText"></param>
    /// <returns></returns>
    private CommandContent? BuildCommand(string commandText)
    {
        CommandContent commandContent = new CommandContent();

        string[] splitedTexts = commandText.Split(commandSeparators, StringSplitOptions.RemoveEmptyEntries);

        // 空白のみの場合などで配列の要素がない場合は処理を行わない
        if (splitedTexts.Length <= 0) return null;

        string commandName = splitedTexts.First();
        DebugCommand command = FindCommandByName(commandName);

        // コマンドが見つからない場合は処理を行わない
        if (command == null)
        {
            DebugManager.Instance.Console.PutErrorLog($"[{commandName}] command is not found");
            return null;
        }

        DebugCommandArgument[] commandArguments = command.Arguments;
        List<DebugCommandArgument> inputArgumentList = new List<DebugCommandArgument>();
        // コマンド名以外の要素は引数として取得する
        string[] commandArgumentTexts = splitedTexts.Where((text) => { return text != commandName; }).ToArray();

        foreach (string argumentText in commandArgumentTexts)
        {
            DebugCommandArgument? buildedArgument = BuildCommandArgument(argumentText);
            // 入力した引数の書式が違う場合は処理を行わない
            if (buildedArgument == null)
            {
                DebugManager.Instance.Console.PutErrorLog($"[{argumentText}] invalid argument format");
                return null;
            }

            DebugCommandArgument inputArgument = (DebugCommandArgument)buildedArgument;
            // 同じ名前の引数はスキップする
            if (inputArgumentList.Contains(inputArgument)) continue;

            bool existsCommandArgument = commandArguments.Where((commandArgument) => { return commandArgument.name == inputArgument.name; }).Count() > 0;
            // 入力した引数がコマンドの引数に存在しないなら処理を行わない
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
                // 必須な引数の値が見つからなかった場合は処理を行わない
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
    /// 名前からコマンドを取得する
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private DebugCommand FindCommandByName(string name)
    {
        return commandList.FirstOrDefault((command) => { return command.CommandName == name; });
    }

    /// <summary>
    /// 文字列からコマンド用の引数をビルドする
    /// </summary>
    /// <param name="argumentText"></param>
    /// <returns></returns>
    private DebugCommandArgument? BuildCommandArgument(string argumentText)
    {
        // 引数用の文字列を [名前, 値] で分ける
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
