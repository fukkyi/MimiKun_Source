using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    /// <summary>
    /// プレイヤーに取得された際の処理
    /// </summary>
    public abstract void ReceivedByPlayer(Player player);

    /// <summary>
    /// 勇者に取得された際の処理
    /// </summary>
    public abstract void ReceivedByHero(Hero hero);
}
