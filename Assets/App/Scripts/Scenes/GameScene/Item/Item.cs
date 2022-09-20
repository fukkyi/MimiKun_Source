using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    /// <summary>
    /// �v���C���[�Ɏ擾���ꂽ�ۂ̏���
    /// </summary>
    public abstract void ReceivedByPlayer(Player player);

    /// <summary>
    /// �E�҂Ɏ擾���ꂽ�ۂ̏���
    /// </summary>
    public abstract void ReceivedByHero(Hero hero);
}
