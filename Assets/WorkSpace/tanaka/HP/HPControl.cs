using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class HPControl : MonoBehaviour
{
    [SerializeField]
    GameObject HitPoints;
    //HP�̐F�ύX�p
    [SerializeField]
    Material material;

    public void InitGauge(int maxHp)
    {
        //�S�폜
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        //�ő�HP������
        for (int i = 0; i < maxHp; i++)
        {
            Instantiate<GameObject>(HitPoints, transform);
        }
        //HP��ΐF�ɕύX
        material.color = Color.green;
    }

    public void UpdateGauge(int hp)
    {
        if(hp >= 0)
        {
            for (int i = 0; i < 1; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            if(hp == 2)
            {
                //HP�����F�ɕύX
                material.color = Color.yellow;
            }
            else if(hp == 1)
            {
                //HP��ԐF�ɕύX
                material.color = Color.red;
            }
            else
            {
                //HP��ΐF�ɕύX
                material.color = Color.green;
            }
        }
        #region �f�o�b�O�p(�����h��)
        /*
        else
        {
            hp = 3;
            //�ő�HP������
            for (int i = 0; i < Maxhp; i++)
            {
                Instantiate<GameObject>(HitPoints, transform);
            }
            //HP��ΐF�ɕύX
            material.color = Color.green;
        }
        */
        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        #region �f�o�b�O�p(F1�L�[�Ŕ�_���[�W)
        /*
        if (Input.GetKeyDown(KeyCode.F1) && hp >= 0)
        {
            UpdateGauge();
        }
        */
        #endregion
    }
}
