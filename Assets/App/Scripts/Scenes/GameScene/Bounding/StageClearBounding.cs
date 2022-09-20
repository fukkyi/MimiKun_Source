using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearBounding : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName = string.Empty;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LayerTagUtil.CompareLayerForPlayer(collision.gameObject.layer))
        {
            if (GameSceneController.Instance.Player.IsDead) return;

            GameSceneController.Instance.ClearCurrentStage(nextSceneName);
        }
    }
}
