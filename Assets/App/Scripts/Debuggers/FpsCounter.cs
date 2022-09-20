using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FpsCounter : DebuggerObject
{
    [SerializeField]
    private TextMeshProUGUI fpsValueText = null;
    [SerializeField]
    private float fpsCountInterval = 0.5f;

    private int frameCount = 0;
    private float prevTime = 0;

    public override void OnDebugModeEnabled()
    {
        frameCount = 0;
        prevTime = Time.realtimeSinceStartup;
    }

    public override void UpdateManage()
    {
        CalcFps();
    }

    /// <summary>
    /// FPS‚ðŒvŽZ‚·‚é
    /// </summary>
    private void CalcFps()
    {
        frameCount++;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= fpsCountInterval)
        {
            int fps = (int)(frameCount / time);
            fpsValueText.SetText("{0}", fps);

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }
    }
}
