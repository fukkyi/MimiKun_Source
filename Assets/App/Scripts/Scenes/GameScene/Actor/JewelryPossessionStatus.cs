using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelryPossessionStatus
{
    public int JewelryEmeraldHaveCount { get; private set; } = 0;
    public int JewelryRubyHaveCount { get; private set; } = 0;
    public int JewelryAmethystHaveCount { get; private set; } = 0;
    public int JewelryDiamondHaveCount { get; private set; } = 0;

    public List<JewelryType> HaveJewelryTypeList { get; private set; } = new List<JewelryType>(4);

    /// <summary>
    /// �S�Ă̕�΂����킹�������擾����
    /// </summary>
    /// <returns></returns>
    public int GetTotalJewelryCount()
    {
        return JewelryEmeraldHaveCount + JewelryRubyHaveCount + JewelryAmethystHaveCount + JewelryDiamondHaveCount;
    }

    /// <summary>
    /// ����̕�΂̌����擾����
    /// </summary>
    /// <param name="jewelryType"></param>
    /// <returns></returns>
    public int GetJewelryCountByType(JewelryType jewelryType)
    {
        int count = 0;
        switch (jewelryType)
        {
            case JewelryType.Emerald:
                count = JewelryEmeraldHaveCount;
                break;

            case JewelryType.Ruby:
                count = JewelryRubyHaveCount;
                break;

            case JewelryType.Amethyst:
                count = JewelryAmethystHaveCount;
                break;

            case JewelryType.Diamond:
                count = JewelryDiamondHaveCount;
                break;
        }

        return count;
    }

    /// <summary>
    /// ����̕�Ώ������𑝂₷
    /// </summary>
    /// <param name="jewelryType"></param>
    /// <param name="addCount"></param>
    public void AddJewelryCountByType(JewelryType jewelryType, int addCount = 1)
    {
        switch (jewelryType)
        {
            case JewelryType.Emerald:
                JewelryEmeraldHaveCount += addCount;
                break;

            case JewelryType.Ruby:
                JewelryRubyHaveCount += addCount;
                break;

            case JewelryType.Amethyst:
                JewelryAmethystHaveCount += addCount;
                break;

            case JewelryType.Diamond:
                JewelryDiamondHaveCount += addCount;
                break;
        }
    }

    /// <summary>
    /// �����Ă����΂̎�ނ��烉���_���Ɉ��ނ�I������
    /// </summary>
    /// <returns></returns>
    public JewelryType? SelectRandomHaveJewelryType()
    {
        UpdateHaveJewelryList();

        int haveJewelryTypeCount = HaveJewelryTypeList.Count;
        if (haveJewelryTypeCount <= 0) return null;

        int randomIndex = Random.Range(0, haveJewelryTypeCount);
        return HaveJewelryTypeList[randomIndex];
    }

    private void UpdateHaveJewelryList()
    {
        HaveJewelryTypeList.Clear();
        if (JewelryEmeraldHaveCount > 0)
        {
            HaveJewelryTypeList.Add(JewelryType.Emerald);
        }
        if (JewelryRubyHaveCount > 0)
        {
            HaveJewelryTypeList.Add(JewelryType.Ruby);
        }
        if (JewelryAmethystHaveCount > 0)
        {
            HaveJewelryTypeList.Add(JewelryType.Amethyst);
        }
        if (JewelryDiamondHaveCount > 0)
        {
            HaveJewelryTypeList.Add(JewelryType.Diamond);
        }
    }
}
