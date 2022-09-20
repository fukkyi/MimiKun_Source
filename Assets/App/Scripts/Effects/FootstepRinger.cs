using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepRinger : MonoBehaviour
{
    protected static readonly float FootStepVolume = 0.2f;

    [SerializeField]
    protected string footstepSoundName = string.Empty;

    /// <summary>
    /// ‘«‰¹‚Ì‰¹º‚ğ–Â‚ç‚·
    /// </summary>
    /// <param name="soundName"></param>
    /// <param name="position"></param>
    protected void PlayFootstepSound()
    {
        if (footstepSoundName == string.Empty) return;

        AudioManager.Instance.PlayRandomPitchSE(footstepSoundName, null, FootStepVolume);
    }
}
