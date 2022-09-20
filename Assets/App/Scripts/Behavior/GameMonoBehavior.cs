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
            // ���������������Ă��Ȃ��ꍇ�͏�������҂�
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
    /// Start() �̃��b�p�[
    /// </summary>
    public virtual void OnStart()
    {
        isStartExecuted = true;
    }

    /// <summary>
    /// Update() �̃��b�p�[
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// Update���������s���Ă��ǂ���Ԃ�
    /// </summary>
    /// <returns></returns>
    protected bool CanExecuteUpdate()
    {
        return GameInitializer.Initialized && isStartExecuted;
    }

    /// <summary>
    /// �Q�[������������������܂ő҂��A���������s����
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
