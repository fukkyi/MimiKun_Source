using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TotalCount : MonoBehaviour

{
    public GameObject score_object = null; // Text�I�u�W�F�N�g
    public int score_num = 0; // �X�R�A�ϐ�

    [SerializeField]
    private ScoreModel scoreModel = null;
    [SerializeField]
    private MasterDataJewelry masterDataJewelry = null;



    // Start is called before the first frame update
    void Start()
    {
        int score = scoreModel.jewelryStatus.JewelryEmeraldHaveCount * masterDataJewelry.emeraldJewelryScore;
        int score2 = scoreModel.jewelryStatus.JewelryRubyHaveCount * masterDataJewelry.rubyJewelryScore;
        int scere3 = scoreModel.jewelryStatus.JewelryAmethystHaveCount * masterDataJewelry.amethystJewelryScore;
        int score4 = scoreModel.jewelryStatus.JewelryDiamondHaveCount * masterDataJewelry.diamondJewelryScore;

        score = score + score2 + scere3 + score4;

        int Life = scoreModel.playerHp;

        score_num = score + Life * 300;


        // �I�u�W�F�N�g����Text�R���|�[�l���g���擾
        Text score_text = score_object.GetComponent<Text>();
        // �e�L�X�g�̕\�������ւ���
        score_text.text = "�v:" + score_num;  //��ΐ�+���C�t�̍��v�̐����o���H
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
