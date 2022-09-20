using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public abstract class AutoGenerateManagerBase<T> : MonoBehaviour where T : AutoGenerateManagerBase<T>
{
    public static T Instance { get; private set; } = null;
    public static readonly string AddressableManagerDirectoryPath = "Prefabs/Manager/";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = GetComponent<T>();
        DontDestroyOnLoad(Instance);

        OnGenerated();
    }

    /// <summary>
    /// マネージャー生成時に呼ばれる処理
    /// </summary>
    protected virtual void OnGenerated() {}

    /// <summary>
    /// Addressable用のアドレスを取得する
    /// </summary>
    /// <returns></returns>
    protected static string GetAddressableAddress()
    {
        Type managerType = typeof(T);
        // [ManagerのPrefabがあるフォルダ]+[クラス名]+[.prefab]
        return AddressableUtil.AddressableRootPath + AddressableManagerDirectoryPath + managerType.Name + AddressableUtil.MineTypePrefab;
    }
}
