using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneControllerManager : AutoGenerateManagerBase<SceneControllerManager>
{
    protected static readonly string SceneControllerName = "Controller";

    protected BaseSceneController sceneController = null;

    /// <summary>
    /// ���݂̃V�[���̃R���g���[���[���擾����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetSceneController<T>() where T : BaseSceneController
    {
        if (sceneController == null)
        {
            CacheSceneController();
        }
        if (sceneController is T castedSceneController)
        {
            return castedSceneController;
        }
        // �w��̃V�[���R���g���[���[�ɃL���X�g�ł��Ȃ��ꍇ��null��Ԃ�
        return null;
    }

    /// <summary>
    /// ���݂̃V�[���̃R���g���[���[���L���X�g���Ȃ��Ŏ擾����
    /// </summary>
    /// <returns></returns>
    public BaseSceneController GetBaseSceneController()
    {
        return sceneController;
    }

    /// <summary>
    /// ���݂̃V�[���R���g���[���[���L���b�V������
    /// </summary>
    public void CacheSceneController()
    {
        sceneController = FindSceneController();
    }

    /// <summary>
    /// �V�[���R���g���[���[���V�[���q�G�����L�[����擾����
    /// </summary>
    /// <returns></returns>
    protected BaseSceneController FindSceneController()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string controllerName = GenerateSceneControllerName(currentScene);

        BaseSceneController sceneController = null;
        foreach (GameObject rootGameObject in currentScene.GetRootGameObjects())
        {
            if (rootGameObject.name != controllerName) continue;

            sceneController = rootGameObject.GetComponent<BaseSceneController>();
            break;
        }

        return sceneController;
    }

    /// <summary>
    /// �V�[���R���g���[���[�p�̃I�u�W�F�N�g�̖��O�𐶐�����
    /// [Scene��] + ["Controller"]
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    public string GenerateSceneControllerName(Scene scene)
    {
        return scene.name + SceneControllerName;
    }
}
