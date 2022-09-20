using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class HPControl : MonoBehaviour
{
    [SerializeField]
    GameObject HitPoints;
    //HPの色変更用
    [SerializeField]
    Material material;

    public void InitGauge(int maxHp)
    {
        //全削除
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        //最大HP分生成
        for (int i = 0; i < maxHp; i++)
        {
            Instantiate<GameObject>(HitPoints, transform);
        }
        //HPを緑色に変更
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
                //HPを黄色に変更
                material.color = Color.yellow;
            }
            else if(hp == 1)
            {
                //HPを赤色に変更
                material.color = Color.red;
            }
            else
            {
                //HPを緑色に変更
                material.color = Color.green;
            }
        }
        #region デバッグ用(無限蘇生)
        /*
        else
        {
            hp = 3;
            //最大HP分生成
            for (int i = 0; i < Maxhp; i++)
            {
                Instantiate<GameObject>(HitPoints, transform);
            }
            //HPを緑色に変更
            material.color = Color.green;
        }
        */
        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        #region デバッグ用(F1キーで被ダメージ)
        /*
        if (Input.GetKeyDown(KeyCode.F1) && hp >= 0)
        {
            UpdateGauge();
        }
        */
        #endregion
    }
}
