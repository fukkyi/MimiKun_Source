using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    /// <summary>
    /// ƒvƒŒƒCƒ„[‚Éæ“¾‚³‚ê‚½Û‚Ìˆ—
    /// </summary>
    public abstract void ReceivedByPlayer(Player player);

    /// <summary>
    /// —EÒ‚Éæ“¾‚³‚ê‚½Û‚Ìˆ—
    /// </summary>
    public abstract void ReceivedByHero(Hero hero);
}
