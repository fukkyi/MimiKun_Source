using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
  fileName = "StageNavigationData",
  menuName = "ScriptableObject/StageNavigationData")
]
public class StageNavigationData : ScriptableObject
{
    public List<GridNavigationPoint> navigationPointList = new List<GridNavigationPoint>();
}
