using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private MasterDataJewelry masterDataJewelry = null;

    public void WriteScore(JewelryPossessionStatus jewelryPossessionStatus)
    {
        int emeraldScore = jewelryPossessionStatus.JewelryEmeraldHaveCount * masterDataJewelry.emeraldJewelryScore;
        int amethstScore = jewelryPossessionStatus.JewelryAmethystHaveCount * masterDataJewelry.amethystJewelryScore;
        int RubyScore = jewelryPossessionStatus.JewelryRubyHaveCount * masterDataJewelry.rubyJewelryScore;
        int diamondScore = jewelryPossessionStatus.JewelryDiamondHaveCount * masterDataJewelry.diamondJewelryScore;

        int totalScore = emeraldScore + amethstScore + RubyScore + diamondScore;

        scoreText.text = ("ÉXÉRÉA\n") + totalScore.ToString();
    } 
}
