using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMonoBehavior : MonoBehaviour
{
    protected bool isStartExecuted = false;

    // Start is called before the first frame update
    void Start()
    {
        if (GameInitializer.Initialized)
        {
            OnStart();
        }
        else
        {
            // 初期化が完了していない場合は初期化を待つ
            StartCoroutine(WaitInitialized(OnStart));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!CanExecuteUpdate()) return;

        OnUpdate();
    }

    /// <summary>
    /// Start() のラッパー
    /// </summary>
    public virtual void OnStart()
    {
        isStartExecuted = true;
    }

    /// <summary>
    /// Update() のラッパー
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// Update処理を実行しても良い状態か
    /// </summary>
    /// <returns></returns>
    protected bool CanExecuteUpdate()
    {
        return GameInitializer.Initialized && isStartExecuted;
    }

    /// <summary>
    /// ゲーム初期化が完了するまで待ち、処理を実行する
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    protected IEnumerator WaitInitialized(Action action)
    {
        while(!GameInitializer.Initialized)
        {
            yield return null;
        }

        action.Invoke();
    }
}
