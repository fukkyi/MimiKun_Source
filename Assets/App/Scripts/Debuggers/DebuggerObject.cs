using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DebuggerObject : MonoBehaviour
{
    public virtual void OnDebugModeEnabled() {}
    public virtual void OnDebugModeDisable() {}
    public abstract void UpdateManage();
}
