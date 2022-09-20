using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JewelryChange : MonoBehaviour
{
    //��Ή摜�̔z��
    [SerializeField]
    public Sprite[] sprite = default;

    //��΂��ڂ���Image
    [SerializeField]
    private Image[] Image = default;

    //��΃J�E���g�p�X�N���v�g
    public JewelryCount jewelryCount;

    //���ݑI�����Ă����΂̎��
    public JewelryType nowJewelry = JewelryType.Emerald;

    void Start()
    {
        //������
        JewelryCount jewelryCount = GetComponent<JewelryCount>();
        for (int i = 0; i < 4; i++)
        {
            Image[i].sprite = sprite[i];
        }
    }

    void Update()
    {
        #region DebugKeyConfig
        
        //�f�o�b�O�p(�u���v�ŕ�΂�����])
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            for(int i = 0;i < 4; i++)
            {
                if(i == 0)
                {
                    Image[4].sprite = Image[0].sprite;
                }
                Image[i].sprite = Image[i + 1].sprite;
            }
            //���ݑI�𒆂̕�΂̏����擾
            jewelrySerch();
        }

        //�f�o�b�O�p(�u���v�ŕ�΂��E��])�����ȉ񂵕����v�������΂Ȃ������̂ŗ͋Z
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Image[4].sprite = Image[0].sprite;
            Image[0].sprite = Image[3].sprite;
            Image[3].sprite = Image[2].sprite;
            Image[2].sprite = Image[1].sprite;
            Image[1].sprite = Image[4].sprite;
            //���ݑI�𒆂̕�΂̏����擾
            jewelrySerch();
        }

        //�f�o�b�O�p(�u���v�Ō��ݑI�𒆂̕�΂�1���Z)
        /*
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            jewelrySerch();
            jewelryCount.pickUp(centerJewelry);
        }

        //�f�o�b�O�p(�u���v�Ō��ݑI�𒆂̕�΂�1���Z��0�ȉ��̏ꍇ�͌����Ȃ�)
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            jewelrySerch();
            jewelryCount.discard(centerJewelry);
        }
        */
        #endregion

        //���ݑI�𒆂̕�΂̌��\��
        jewelryCount.indication(nowJewelry);
            
    }
    //���ݑI�𒆂̕�΂�T��
    public void jewelrySerch()
    {
        for (int i = 0; i < 4; i++)
        {
            //�����̕�΂̎��
            if (Image[0].sprite == sprite[i])
            {
                nowJewelry = (JewelryType)i;
            }
        }
    }

    public void SelectJewelryToRight()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                Image[4].sprite = Image[0].sprite;
            }
            Image[i].sprite = Image[i + 1].sprite;
        }
        //���ݑI�𒆂̕�΂̏����擾
        jewelrySerch();
    }

    public void SelectJewelryToLeft()
    {
        Image[4].sprite = Image[0].sprite;
        Image[0].sprite = Image[3].sprite;
        Image[3].sprite = Image[2].sprite;
        Image[2].sprite = Image[1].sprite;
        Image[1].sprite = Image[4].sprite;
        //���ݑI�𒆂̕�΂̏����擾
        jewelrySerch();
    }
}
