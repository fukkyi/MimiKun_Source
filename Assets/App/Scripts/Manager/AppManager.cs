using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AppManager : AutoGenerateManagerBase<AppManager>
{
    [SerializeField]
    private EventSystem eventSystem = null;
    public EventSystem EventSystem { get { return eventSystem; } }
}