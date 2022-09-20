using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneUICanvas : MonoBehaviour
{
    public HPControl PlayerHpUI { get { return playerHpUI; } }
    [SerializeField]
    private HPControl playerHpUI = null;

    public improveJewelryUi JewelrySelectUI { get { return jewelrySelectUI; } }
    [SerializeField]
    private improveJewelryUi jewelrySelectUI = null;

    public TrapHaveUI TrapHaveUI { get { return trapHaveUI; } }
    [SerializeField]
    private TrapHaveUI trapHaveUI = null;

    public ScoreUI ScoreUI { get { return scoreUI; } }
    [SerializeField]
    private ScoreUI scoreUI = null;
}
