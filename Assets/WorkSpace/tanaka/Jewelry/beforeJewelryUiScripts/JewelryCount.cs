using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JewelryCount : MonoBehaviour
{
    //��΂̌��\���̃e�L�X�g�擾
    [SerializeField]
    private TextMeshProUGUI centerCount;

    //���ꂼ��̕�΂̌��̊i�[�p�̔z��
    // public int[] jewelryCount = new int[4] {0,0,0,0};

    //��΂�1���Z
    public int pickUp(int n)
    {
        Debug.Log("��ΏE����");
        // jewelryCount[n]++;

        //��΂̏������\���̍X�V
        //indication(n);
        return 0;
    }
    //��΂�1���Z��0�ȉ��̏ꍇ�����Ȃ�
    public int discard(int n)
    {
        Debug.Log("��Ύ̂Ă�");
        /*
        if (jewelryCount[n] > 0)
        {
            // jewelryCount[n]--;

            //��΂̏������\���̍X�V
            indication(n);
        }
        */
        return 0;
    }

    //�I�𒆂̕�΂̌��ݏ������̕\��
    public int indication(JewelryType jewelryType)
    {
        // centerCount.text = jewelryCount[n].ToString();
        centerCount.text = GameSceneController.Instance.Player.JewelryPossessionStatus.GetJewelryCountByType(jewelryType).ToString();
        return 0;
    }
}
