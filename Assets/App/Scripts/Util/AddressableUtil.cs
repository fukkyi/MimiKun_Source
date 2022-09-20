using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressableUtil
{
    public static readonly string AddressableRootPath = "Assets/App/Addressables/";
    public static readonly string MineTypePrefab = ".prefab";

    public static void LoadPrefabsByLable(AddressableLable lable, Action<AsyncOperationHandle<IList<GameObject>>> OnAssetLoaded)
    {
        Addressables.LoadAssetsAsync<GameObject>(lable.ToString(), null).Completed += OnAssetLoaded;
    }

    /// <summary>
    /// Addresable�V�X�e���Ńv���n�u��ǂݍ���
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="OnAssetLoaded"></param>
    public static void LoadPrefab(string assetPath, Action<AsyncOperationHandle<GameObject>> OnAssetLoaded)
    {
        string addressableAssetPath = GenerateAssetPath(assetPath, MineTypePrefab);
        Addressables.LoadAssetAsync<GameObject>(addressableAssetPath).Completed += OnAssetLoaded;
    }

    /// <summary>
    /// Addressable�A�Z�b�g��ǂݍ��ނ��߂̃p�X���쐬����
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="mineType"></param>
    /// <returns></returns>
    private static string GenerateAssetPath(string assetPath, string mineType)
    {
        return AddressableRootPath + assetPath + mineType;
    }
}

public enum AddressableLable
{
    None,
    Default,
    Manager,
}
