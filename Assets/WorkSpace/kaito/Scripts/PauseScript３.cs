using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript３ : MonoBehaviour
{
    [SerializeField]
    GameObject pausePanel;
    // Start is called before the first frame update
    void Start()
    {
        //始めPause画面は非表示
        pausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Pボタンが押された時Pause画面が表示される
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
        }
    }
    public void Resume()
    {
        //再開ボタンが押された時Pause画面を非表示にして時間を戻す
        Time.timeScale = 1;
        pausePanel.SetActive(false);

    }
}
