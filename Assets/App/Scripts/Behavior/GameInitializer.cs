using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public static bool Initialized = false;

    /// <summary>
    /// �Q�[�����s���̏���������
    /// (�Q�[���J�n���Ɏ����ŌĂ΂��)
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        GenerateManagers();
    }

    /// <summary>
    /// Addressable�ɓo�^����Ă���S�Ẵ}�l�[�W���[�𐶐�����
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
