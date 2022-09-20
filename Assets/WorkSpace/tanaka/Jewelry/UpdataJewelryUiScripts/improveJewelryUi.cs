using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class improveJewelryUi : MonoBehaviour
{
    //��Ή摜�̔z��
    [Header("Emerald,Ruby,Amethyst,Diamond�̏��ԂœK�p")]
    [SerializeField]
    private Sprite[] sprite = default;

    //��΂��ڂ���Image
    [SerializeField]
    private Image centerImage = default;

    //��΂̌��\���̃e�L�X�g�擾
    [SerializeField]
    private TextMeshProUGUI centerCount;

    //���ݑI�����Ă����΂̎��
    public JewelryType nowJewelry = JewelryType.Emerald;

    //���ꂼ��̕�΂̌��̊i�[�p�̔z��
    public int[] jewelryCount = new int[4] { 0, 0, 0, 0 };
    //�ǂ̕�΂̌��ɕω������������m�F�p
    private int[] oldJewelryCount = new int[4] { 0, 0, 0, 0 };

    //�e���΂����L���Ă��邩�ǂ���
    public bool existEmerald = false;
    public bool existRuby = false;
    public bool existAmethyst = false;
    public bool existDiamond = false;

    void Start()
    {
        //������

        centerImage.sprite = sprite[0];

        //�����������Ƃ���΂����L���ĂȂ��̂œ�����
        centerImage.color = new Color(255, 255, 255, 0);
        centerCount.color = new Color(255, 255, 255, 0);
    }


    void Update()
    {

        #region DebugKeyConfig
        /*
        //�f�o�b�O�p(�u���vor�uQ�v�ŕ�΂�����])
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Q))
        {
            leftChangeJewelry();
        }

        //�f�o�b�O�p(�u���vor�uE�v�ŕ�΂��E��])
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.E))
        {
            rightChangeJewelry();
        }

        //�f�o�b�O�p(��΂̎��(1,2,3,4)�Ɓu���v�ŕ�΂�1���Z)
        //�G�������h����ǉ�
        if (Input.GetKey(KeyCode.Alpha1) && Input.GetKeyDown(KeyCode.UpArrow))
        {
            jewelryCount[0]++;
            pickUp();
        }
        if (Input.GetKey(KeyCode.Alpha2) && Input.GetKeyDown(KeyCode.UpArrow))
        {
            jewelryCount[1]++;
            pickUp();
        }
        if (Input.GetKey(KeyCode.Alpha3) && Input.GetKeyDown(KeyCode.UpArrow))
        {
            jewelryCount[2]++;
            pickUp();
        }
        if (Input.GetKey(KeyCode.Alpha4) && Input.GetKeyDown(KeyCode.UpArrow))
        {
            jewelryCount[3]++;
            pickUp();
        }

        //�f�o�b�O�p(��΂̎��(1,2,3,4)�Ɓu���v�Ō��ݑI�𒆂̕�΂�1���Z��0�ȉ��̏ꍇ�͌����Ȃ�)
        if (Input.GetKey(KeyCode.Alpha1) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            jewelryCount[0]--;
            discard();
        }
        if (Input.GetKey(KeyCode.Alpha2) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            jewelryCount[1]--;
            discard();
        }
        if (Input.GetKey(KeyCode.Alpha3) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            jewelryCount[2]--;
            discard();
        }
        if (Input.GetKey(KeyCode.Alpha4) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            jewelryCount[3]--;
            discard();
        }
        */
        #endregion

        //indication(nowJewelry);

    }

    //�ǂ̕�΂��E����������
    public void pickUp()
    {
        jewelryCount = ConvertJewelryCountByPossessionStatus(jewelryCount);
        Debug.Log("��ΏE����");
        //�ǂ̕�΂��E������
        for (int i = 0; i < 4; i++)
        {
            //�ω�����������΂�T��
            if (oldJewelryCount[i] != jewelryCount[i])
            {
                //�E������΂�\������
                switch (i)
                {
                    case 0:
                        {
                            existEmerald = true;
                            indication(JewelryType.Emerald);
                            break;
                        }
                    case 1:
                        {
                            existRuby = true;
                            indication(JewelryType.Ruby);
                            break;
                        }
                    case 2:
                        {
                            existAmethyst = true;
                            indication(JewelryType.Amethyst);
                            break;
                        }
                    case 3:
                        {
                            existDiamond = true;
                            indication(JewelryType.Diamond);
                            break;
                        }
                }
            }
        }
        
        jewelryCount.CopyTo(oldJewelryCount, 0);
    }

    //�ǂ̕�΂��̂Ă�������
    public void discard()
    {
        jewelryCount = ConvertJewelryCountByPossessionStatus(jewelryCount);
        Debug.Log("��Ύ̂Ă�");
        //�ǂ̕�΂��̂Ă���
        for (int i = 0; i < 4; i++)
        {
            //�ω�����������΂�T��
            if (oldJewelryCount[i] != jewelryCount[i])
            {
                //0�ȉ��ɂȂ����ꍇ�A0�ɂ���
                if (jewelryCount[i] <= 0) jewelryCount[i] = 0;

                oldJewelryCount[i] = jewelryCount[i];

                //0�ɂȂ��Ă����ꍇ�A�e���΂̃t���O��ύX����
                //�I�𒆂̕�΂�0�ɂȂ����ꍇ�A�ʂ̏��L���Ă����΂ɕύX
                if (jewelryCount[i] <= 0)
                {
                    switch (i)
                    {
                        case 0:
                            {
                                existEmerald = false;
                                if (nowJewelry == JewelryType.Emerald)
                                    rightChangeJewelry();
                                break;
                            }
                        case 1:
                            {
                                existRuby = false;
                                if (nowJewelry == JewelryType.Ruby)
                                    rightChangeJewelry();
                                break;
                            }
                        case 2:
                            {
                                existAmethyst = false;
                                if (nowJewelry == JewelryType.Amethyst)
                                    rightChangeJewelry();
                                break;
                            }
                        case 3:
                            {
                                existDiamond = false;
                                if (nowJewelry == JewelryType.Diamond)
                                    rightChangeJewelry();
                                break;
                            }
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            {
                                indication(JewelryType.Emerald);
                                break;
                            }
                        case 1:
                            {
                                indication(JewelryType.Ruby);
                                break;
                            }
                        case 2:
                            {
                                indication(JewelryType.Amethyst);
                                break;
                            }
                        case 3:
                            {
                                indication(JewelryType.Diamond);
                                break;
                            }
                    }
                }
            }
        }

        jewelryCount.CopyTo(oldJewelryCount, 0);
    }

    //���ׂĂ̕�΂����L���Ă��Ȃ��ꍇ�utrue�v��Ԃ�
    public bool notHaveJewelry()
    {
        if (!existEmerald && !existRuby && !existAmethyst && !existDiamond)
        {
            return true;
        }
        return false;
    }

    //��΂����L�Ă���Ƃ��󔒏�Ԃ���������
    public void activeJewelryImage()
    {
        if (!notHaveJewelry())
        {
            //�ǂꂩ��΂����L���Ă���Ƃ��\������
            centerImage.color = new Color(255, 255, 255, 255);
            centerCount.color = new Color(255, 255, 255, 255);
        }
        else
        {
            //���ׂĕ�΂����L���Ă��Ȃ��ꍇ�󔒂ɂ���(������)
            centerImage.color = new Color(255, 255, 255, 0);
            centerCount.color = new Color(255, 255, 255, 0);
        }
    }

    //�n���ꂽ��΂̎�ނ�\������
    public void indication(JewelryType jewelrySelect)
    {
        if (notHaveJewelry())
        {
            activeJewelryImage();
        }
        else
        {
            activeJewelryImage();
            //��̑I��������΂�\������
            switch (jewelrySelect)
            {
                case JewelryType.Emerald:
                    {
                        nowJewelry = JewelryType.Emerald;
                        centerImage.sprite = sprite[0];
                        break;
                    }
                case JewelryType.Ruby:
                    {
                        nowJewelry = JewelryType.Ruby;
                        centerImage.sprite = sprite[1];
                        break;
                    }
                case JewelryType.Amethyst:
                    {
                        nowJewelry = JewelryType.Amethyst;
                        centerImage.sprite = sprite[2];
                        break;
                    }
                case JewelryType.Diamond:
                    {
                        nowJewelry = JewelryType.Diamond;
                        centerImage.sprite = sprite[3];
                        break;
                    }
            }

            centerCount.text = GameSceneController.Instance.Player.JewelryPossessionStatus.GetJewelryCountByType(jewelrySelect).ToString();
            Debug.Log(jewelrySelect);
            Debug.Log(GameSceneController.Instance.Player.JewelryPossessionStatus.GetJewelryCountByType(jewelrySelect));
        }
    }

    //�E���(��΂����L���Ă��Ȃ��ꍇ�X�L�b�v)
    public void rightChangeJewelry()
    {
        switch (nowJewelry)
        {
            case JewelryType.Emerald:
                {
                    if (existRuby) nowJewelry += 1;
                    else if (existAmethyst) nowJewelry += 2;
                    else if (existDiamond) nowJewelry += 3;
                    indication(nowJewelry);
                    break;
                }
            case JewelryType.Ruby:
                {
                    if (existAmethyst) nowJewelry += 1;
                    else if (existDiamond) nowJewelry += 2;
                    else if (existEmerald) nowJewelry -= 1;
                    indication(nowJewelry);
                    break;
                }
            case JewelryType.Amethyst:
                {
                    if (existDiamond) nowJewelry += 1;
                    else if (existEmerald) nowJewelry -= 2;
                    else if (existRuby) nowJewelry -= 1;
                    indication(nowJewelry);
                    break;
                }
            case JewelryType.Diamond:
                {
                    if (existEmerald) nowJewelry -= 3;
                    else if (existRuby) nowJewelry -= 2;
                    else if (existAmethyst) nowJewelry -= 1;
                    indication(nowJewelry);
                    break;
                }
            default:
                {
                    indication(nowJewelry);
                    break;
                }
        }
    }

    //�����(��΂����L���Ă��Ȃ��ꍇ�X�L�b�v)
    public void leftChangeJewelry()
    {
        switch (nowJewelry)
        {
            case JewelryType.Emerald:
                {
                    if (existDiamond) nowJewelry += 3;
                    else if (existAmethyst) nowJewelry += 2;
                    else if (existRuby) nowJewelry += 1;
                    indication(nowJewelry);
                    break;
                }
            case JewelryType.Ruby:
                {
                    if (existEmerald) nowJewelry -= 1;
                    else if (existDiamond) nowJewelry += 2;
                    else if (existAmethyst) nowJewelry += 1;
                    indication(nowJewelry);
                    break;
                }
            case JewelryType.Amethyst:
                {
                    if (existRuby) nowJewelry -= 1;
                    else if (existEmerald) nowJewelry -= 2;
                    else if (existDiamond) nowJewelry += 1;
                    indication(nowJewelry);
                    break;
                }
            case JewelryType.Diamond:
                {
                    if (existAmethyst) nowJewelry -= 1;
                    else if (existRuby) nowJewelry -= 2;
                    else if (existEmerald) nowJewelry -= 3;
                    indication(nowJewelry);
                    break;
                }
            default:
                {
                    indication(nowJewelry);
                    break;
                }
        }
    }

    private int[] ConvertJewelryCountByPossessionStatus(int[] jewelryCount)
    {
        JewelryPossessionStatus playerJewelry = GameSceneController.Instance.Player.JewelryPossessionStatus;
        jewelryCount[0] = playerJewelry.JewelryEmeraldHaveCount;
        jewelryCount[1] = playerJewelry.JewelryRubyHaveCount;
        jewelryCount[2] = playerJewelry.JewelryAmethystHaveCount;
        jewelryCount[3] = playerJewelry.JewelryDiamondHaveCount;

        return jewelryCount;
    }
}
