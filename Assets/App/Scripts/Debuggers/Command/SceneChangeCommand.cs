using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeCommand : DebugCommand
{
    public override string CommandName => "scene_change";

    public override DebugCommandArgument[] Arguments => new DebugCommandArgument[] { 
        new DebugCommandArgument("scene"),
        new DebugCommandArgument("transType", "FadeInOut")
    };

    public override bool Execute(DebugCommandArgument[] arguments)
    {
        string sceneName = GetArgumentValueByName(arguments, "scene");
        string transType = GetArgumentValueByName(arguments, "transType");

        if (!ExistSceneByName(sceneName))
        {
            DebugManager.Instance.Console.PutErrorLog($"[{sceneName}] scene is not Found");
            return false;
        }

        TransitionType transitionType;
        if (!Enum.TryParse(transType, true, out transitionType))
        {
            DebugManager.Instance.Console.PutErrorLog($"[{transType}] transitionType is not found");
            return false;
        }
        // 遷移中の場合はコマンドを実行しない
        if (SceneTransitionManager.Instance.IsTransiting)
        {
            DebugManager.Instance.Console.PutErrorLog($"already scene transitioning");
            return false;
        }

        SceneTransitionManager.Instance.TransitionByName(sceneName, transitionType);

        DebugManager.Instance.Console.PutOKLog($"scene trasition to [{sceneName}]");

        return true;
    }

    /// <summary>
    /// 名前から存在するシーンか調べる
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private bool ExistSceneByName(string sceneName)
    {
        bool sceneExist = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            // BuildSettingsに登録されたシーンからシーンのパスを取得する
            string registeredScenePath = SceneUtility.GetScenePathByBuildIndex(i);
            // シーンのパスからシーン名を取得する
            string registeredSceneName = Path.GetFileNameWithoutExtension(registeredScenePath);

            if (registeredSceneName == sceneName)
            {
                sceneExist = true;
                break;
            }
        }

        return sceneExist;
    }
}
