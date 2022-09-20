using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public static bool Initialized = false;

    /// <summary>
    /// ゲーム実行時の初期化処理
    /// (ゲーム開始時に自動で呼ばれる)
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        GenerateManagers();
    }

    /// <summary>
    /// Addressableに登録されている全てのマネージャーを生成する
    /// </summary>
    private static void GenerateManagers()
    {
        AddressableUtil.LoadPrefabsByLable(AddressableLable.Manager, (handle) =>
        {
            foreach (GameObject managerObj in handle.Result)
            {
                Instantiate(managerObj);
            }

            Initialized = true;
        });
    }
}
