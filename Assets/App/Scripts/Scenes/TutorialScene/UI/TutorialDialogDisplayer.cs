using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDialogDisplayer : MonoBehaviour
{
    [SerializeField]
    private TutorialDialog.DialogType dialogType = new TutorialDialog.DialogType();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LayerTagUtil.CompareLayerForPlayer(collision.gameObject.layer))
        {
            if (GameSceneController.Instance.Player.IsDead) return;

            TutorialSceneController.Instance.ShowTutorialDialog(dialogType);
        }
    }
}
