using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridPlatformerNavigator : MonoBehaviour
{
    public Vector3 TileCellSize { get { return platformerTilemap.cellSize; } }

    private readonly static string navigationDataPath = "Assets/App/ScriptableObjects/MasterData/NavigationData/";

    [SerializeField]
    private Tilemap platformerTilemap = null;
    [SerializeField]
    private int walkPathCost = 1;
    [SerializeField]
    private int fallPathCost = 5;
    [SerializeField]
    private int jumpPathCost = 10;

    [SerializeField]
    private Vector2 agentColliderSize = new Vector2();
    [SerializeField]
    private float agentCollisionOffset = 0.07f;
    [SerializeField]
    private float agentFallOffset = 0.01f;
    [SerializeField]
    private float agentMaxMoveSpeed = 10;
    [SerializeField]
    private float agentMaxJumpHeight = 5.2f;
    [SerializeField]
    private float agentGravityScale = 5;
    [SerializeField]
    private int maxPhysicsSimulateStep = 100;
    [SerializeField, Range(1, 10)]
    private int simulateStepScale = 2;
    [SerializeField, Range(1, 10)]
    private int simulateMoveScale = 3;
    [SerializeField, Range(1, 10)]
    private int simulateJumpScale = 3;
    [SerializeField]
    private bool showWalkPath = true;
    [SerializeField]
    private bool showFallPath = true;
    [SerializeField]
    private bool showJumpPath = true;

    [SerializeField]
    private StageNavigationData navigationData = null;

    private List<GridNavigationPoint> navigationPointList = new List<GridNavigationPoint>();
    private Collider2D[] stageOverlapColliderBuffer = new Collider2D[1];
    private RaycastHit2D[] stageRaycastHitBuffer = new RaycastHit2D[1];

    private void Start()
    {

    }

    #region PublicMethods

#if UNITY_EDITOR
    /// <summary>
    /// 経路探索情報をベイクする
    /// </summary>
    [ContextMenu("BakeNavigation")]
    public StageNavigationData BakeNavigation()
    {
        navigationPointList.Clear();

        GenerateNavigationPoint();
        GenerateWalkNavigationPath();
        GenerateFallNavagationPath();
        GenerateJumpNavagationPath();

        return SaveNavigationData();
    }

    /// <summary>
    /// 経路情報をアセットとして作成する
    /// </summary>
    private StageNavigationData SaveNavigationData()
    {
        StageNavigationData stageNavigationData = ScriptableObject.CreateInstance<StageNavigationData>();

        stageNavigationData.navigationPointList = new List<GridNavigationPoint>(navigationPointList);
        string fileName = $"{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}_NavigationPath.asset";
        if (!Directory.Exists(navigationDataPath))
        {
            Directory.CreateDirectory(navigationDataPath);
        }

        AssetDatabase.CreateAsset(stageNavigationData, Path.Combine(navigationDataPath, fileName));

        EditorUtility.SetDirty(stageNavigationData);
        AssetDatabase.SaveAssets();

        return stageNavigationData;
    }
#endif

    /// <summary>
    /// 全ての経路点を取得する
    /// </summary>
    /// <returns></returns>
    public List<GridNavigationPoint> GetNavigationPointList()
    {
        if (navigationData == null)
        {
            return navigationPointList;
        }

        return navigationData.navigationPointList;
    }

    /// <summary>
    /// グリッドのセル座標からポイントを取得する
    /// </summary>
    /// <returns></returns>
    public GridNavigationPoint GetNavigationPointByCellPos(Vector3Int cellPos, List<GridNavigationPoint> navigationPointList = null)
    {
        if (navigationPointList == null)
        {
            navigationPointList = GetNavigationPointList();
        }

        return navigationPointList.Find(point => point.CellPos == cellPos);
    }

    /// <summary>
    /// ワールド座標からポイントを取得する
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public GridNavigationPoint GetNavigationPointByWorldPos(Vector3 worldPos)
    {
        Vector3Int cellPos = platformerTilemap.WorldToCell(worldPos);
        GridNavigationPoint originPosPoint = GetNavigationPointByCellPos(cellPos);

        if (originPosPoint != null) return originPosPoint;
        // 足場ギリギリに立っていて、足場があるのに経路点がない場合を想定した経路点を取得する
        float roundedPosX = Mathf.Round(worldPos.x) - worldPos.x;
        if (roundedPosX > 0)
        {
            cellPos.x = cellPos.x + 1;
        }
        else
        {
            cellPos.x = cellPos.x - 1;
        }

        return GetNavigationPointByCellPos(cellPos);
    }

    /// <summary>
    /// 目標点までの経路を計算する
    /// </summary>
    /// <param name="fromPos"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public List<GridNavigationPath> CalcNavigatePath(Vector3 fromPos, Vector3 targetPos)
    {
        GridNavigationPoint fromPoint = GetNavigationPointByWorldPos(fromPos);
        if (fromPoint == null) return null;

        GridNavigationPoint targetPoint = GetNavigationPointByWorldPos(targetPos);
        if (targetPoint == null) return null;

        // 追跡元と追跡対象が同じ座標の場合は計算しない
        if (fromPoint.CellPos == targetPoint.CellPos) return null;

        List<GridNavigationPoint> navigationPointList = GetNavigationPointList();
        List<GridNavigationPath> openPathList = new List<GridNavigationPath>();
        HashSet<GridNavigationPoint> closePointList = new HashSet<GridNavigationPoint>();

        openPathList = fromPoint.AddOpenLinkPathWithCalcScore(openPathList, targetPoint, null, navigationPointList);
        closePointList.Add(fromPoint);

        while(openPathList.Count > 0)
        {
            GridNavigationPath bestPath = openPathList[0];
            // 一番スコアが低い経路を取得する
            for (int i = 1; i < openPathList.Count; i++)
            {
                bestPath = openPathList[i].NavigationScore < bestPath.NavigationScore ? openPathList[i] : bestPath;
            }

            openPathList.Remove(bestPath);

            GridNavigationPoint nextPoint = bestPath.GetToPoint(navigationPointList);

            if (nextPoint.CellPos == targetPoint.CellPos)
            {
                List<GridNavigationPath> navigatePathList = new List<GridNavigationPath>();
                navigatePathList = bestPath.GetUntilPathList(navigatePathList);
                // 再帰的に取得したパスは、目標点から基準点に辿る順番で入る為、要素の順番を逆転させる
                navigatePathList.Reverse();

                return navigatePathList;
            }
            // 初到達の経路点の場合
            if (!closePointList.Contains(nextPoint))
            {
                // 次のパスを追加し、スコアの昇順で並び変える
                openPathList = nextPoint.AddOpenLinkPathWithCalcScore(openPathList, targetPoint, bestPath, navigationPointList);
                closePointList.Add(nextPoint);
            }
        }


        return null;
    }

    #endregion

    #region GenerateNavigation
    /// <summary>
    /// 経路探索の元となる点を生成する
    /// </summary>
    private void GenerateNavigationPoint()
    {
        int gridMinY = platformerTilemap.cellBounds.yMin + 1;
        int gridMaxY = platformerTilemap.cellBounds.yMax + 1;
        int gridMinX = platformerTilemap.cellBounds.xMin;
        int gridMaxX = platformerTilemap.cellBounds.xMax;
        int currentPlatformIndex = 0;
        Vector3Int gridPos = new Vector3Int();

        for (int y = gridMaxY; y >= gridMinY; y--)
        {
            bool platformStarted = false;
            for (int x = gridMinX; x <= gridMaxX; x++)
            {
                gridPos.x = x;
                gridPos.y = y;
                bool hasTileGrid = platformerTilemap.HasTile(gridPos);
                bool hasTileDownGrid = platformerTilemap.HasTile(gridPos + Vector3Int.down);
                // 現在のグリッドが空白で、下のグリッドにタイルがある場合のみ点を追加する
                if (hasTileGrid || !hasTileDownGrid) continue;

                bool hasTileRightGrid = platformerTilemap.HasTile(gridPos + Vector3Int.right);
                bool hasTileDownRightGrid = platformerTilemap.HasTile(gridPos + Vector3Int.down + Vector3Int.right);
                GridNavigationPointType pointType;
                // 足場すでに始まっている場合
                if (platformStarted)
                {
                    // 右のグリッドにタイルがあるか、右下のグリッドが空白である場合
                    if (hasTileRightGrid || !hasTileDownRightGrid)
                    {
                        pointType = GridNavigationPointType.RightEdge;
                        platformStarted = false;
                    }
                    else
                    {
                        pointType = GridNavigationPointType.Platform;
                    }

                    navigationPointList.Add(new GridNavigationPoint(pointType, currentPlatformIndex, gridPos, TileMapCellToWorldCenteringXPos(gridPos)));
                }
                else
                {
                    // 右のグリッドにタイルがあるか、右下のグリッドが空白である場合
                    if (hasTileRightGrid || !hasTileDownRightGrid)
                    {
                        pointType = GridNavigationPointType.Solo;
                    }
                    else
                    {
                        pointType = GridNavigationPointType.LeftEdge;
                        platformStarted = true;
                    }

                    navigationPointList.Add(new GridNavigationPoint(pointType, currentPlatformIndex, gridPos, TileMapCellToWorldCenteringXPos(gridPos)));
                }
                // 足場が途切れたらインデックスを加算する
                if (!platformStarted)
                {
                    currentPlatformIndex++;
                }
            }
        }
    }

    /// <summary>
    /// 歩いて到達できる経路を生成する
    /// </summary>
    private void GenerateWalkNavigationPath()
    {
        foreach(GridNavigationPoint navigationPoint in navigationPointList)
        {
            // 基点は横が半分の大きさでステージを検知する
            Vector2 halfHorizotalAgentSize = GetOffsettingAgentColliderSize();
            halfHorizotalAgentSize.x /= 2;

            if (navigationPoint.Type == GridNavigationPointType.LeftEdge || navigationPoint.Type == GridNavigationPointType.Platform)
            {
                Vector3Int rightCellPos = navigationPoint.CellPos + Vector3Int.right;

                GridNavigationPoint rightNavigationPoint = GetNavigationPointByCellPos(rightCellPos, navigationPointList);

                if (!IsCollisionAgentToStageByPos(navigationPoint.WorldPos, halfHorizotalAgentSize) && !IsCollisionAgentToStageByPos(rightNavigationPoint.WorldPos))
                {
                    GridNavigationPath newRunPath = new GridNavigationPath(
                        GridNavigationPathType.Walk,
                        navigationPointList.IndexOf(rightNavigationPoint),
                        navigationPointList.IndexOf(navigationPoint),
                        walkPathCost
                    );
                    navigationPoint.AddLinkPath(newRunPath);
                }
            }
            if (navigationPoint.Type == GridNavigationPointType.RightEdge || navigationPoint.Type == GridNavigationPointType.Platform)
            {
                Vector3Int leftCellPos = navigationPoint.CellPos + Vector3Int.left;
                GridNavigationPoint leftNavigationPoint = GetNavigationPointByCellPos(leftCellPos, navigationPointList);

                if (!IsCollisionAgentToStageByPos(navigationPoint.WorldPos, halfHorizotalAgentSize) && !IsCollisionAgentToStageByPos(leftNavigationPoint.WorldPos))
                {
                    GridNavigationPath newRunPath = new GridNavigationPath(
                        GridNavigationPathType.Walk,
                        navigationPointList.IndexOf(leftNavigationPoint),
                        navigationPointList.IndexOf(navigationPoint),
                        walkPathCost
                    );
                    navigationPoint.AddLinkPath(newRunPath);
                }
            }
        }
    }

    /// <summary>
    /// 落下して到達できる経路を生成する
    /// </summary>
    private void GenerateFallNavagationPath()
    {
        foreach(GridNavigationPoint navigationPoint in navigationPointList)
        {
            // 端でない場合は落下できないため計算しない
            if (navigationPoint.Type == GridNavigationPointType.Platform) continue;

            Vector2 halfAgentSize = agentColliderSize / 2;
            Vector2 cellHalfSize = TileCellSize / 2;

            for(int i = simulateMoveScale; i >= 0; i--)
            {
                float fallingMoveSpeed = 0;
                if (i > 0)
                {
                    fallingMoveSpeed = agentMaxMoveSpeed / simulateMoveScale * i;
                }

                // 左から落下できる足場の場合
                if (navigationPoint.Type == GridNavigationPointType.LeftEdge || navigationPoint.Type == GridNavigationPointType.Solo)
                {
                    Vector3 fallPoint = navigationPoint.WorldPos;
                    Vector3 fallVector = Vector3.left * fallingMoveSpeed;
                    fallPoint.x -= halfAgentSize.x + cellHalfSize.x + agentFallOffset;
                    GridNavigationPoint landingPoint = GetLandingPointOnPhysicsSimulate(fallPoint, fallVector, navigationPoint);
                    if (landingPoint != null && !navigationPoint.IsExistToPointPath(landingPoint, navigationPointList))
                    {
                        GridNavigationPath newFallPath = new GridNavigationPath(
                            GridNavigationPathType.Fall,
                            navigationPointList.IndexOf(landingPoint),
                            navigationPointList.IndexOf(navigationPoint),
                            fallPathCost,
                            -fallingMoveSpeed
                        );
                        navigationPoint.AddLinkPath(newFallPath);
                    }
                }

                // 右から落下できる足場の場合
                if (navigationPoint.Type == GridNavigationPointType.RightEdge || navigationPoint.Type == GridNavigationPointType.Solo)
                {
                    Vector3 fallPoint = navigationPoint.WorldPos;
                    Vector3 rightVector = Vector3.right * fallingMoveSpeed;
                    fallPoint.x += halfAgentSize.x + cellHalfSize.x + agentFallOffset;
                    GridNavigationPoint landingPoint = GetLandingPointOnPhysicsSimulate(fallPoint, rightVector, navigationPoint);
                    if (landingPoint != null && !navigationPoint.IsExistToPointPath(landingPoint, navigationPointList))
                    {
                        GridNavigationPath newFallPath = new GridNavigationPath(
                            GridNavigationPathType.Fall,
                            navigationPointList.IndexOf(landingPoint),
                            navigationPointList.IndexOf(navigationPoint),
                            fallPathCost,
                            fallingMoveSpeed
                        );
                        navigationPoint.AddLinkPath(newFallPath);
                    }
                }
            }
        }
    }

    /// <summary>
    /// ジャンプして到達できる経路を生成する
    /// </summary>
    private void GenerateJumpNavagationPath()
    {
        foreach (GridNavigationPoint navigationPoint in navigationPointList)
        {
            for (int i = simulateMoveScale; i >= 1; i--)
            {
                float jumpingMoveSpeed = agentMaxMoveSpeed / simulateMoveScale * i;

                for (int j = simulateJumpScale; j >= 1; j--)
                {
                    float jumpHeight = agentMaxJumpHeight / simulateJumpScale * j;
                    float jumpPower = Physics2DUtil.CalcInitialVelocityToReachHeight(jumpHeight, 0, agentGravityScale);
                    // 左ジャンプの経路計算
                    Vector3 jumpVector = Vector3.left * jumpingMoveSpeed + Vector3.up * jumpPower;
                    GridNavigationPoint landingPoint = GetLandingPointOnPhysicsSimulate(navigationPoint.WorldPos, jumpVector, navigationPoint);
                    if (landingPoint != null && !navigationPoint.IsExistToPointPath(landingPoint, navigationPointList))
                    {
                        GridNavigationPath newJumpPath = new GridNavigationPath(
                            GridNavigationPathType.Jump,
                            navigationPointList.IndexOf(landingPoint),
                            navigationPointList.IndexOf(navigationPoint),
                            jumpPathCost,
                            -jumpingMoveSpeed,
                            jumpPower
                        );
                        navigationPoint.AddLinkPath(newJumpPath);
                    }

                    // 右ジャンプの経路計算
                    jumpVector = Vector3.right * jumpingMoveSpeed + Vector3.up * jumpPower;
                    landingPoint = GetLandingPointOnPhysicsSimulate(navigationPoint.WorldPos, jumpVector, navigationPoint);
                    if (landingPoint != null && !navigationPoint.IsExistToPointPath(landingPoint, navigationPointList))
                    {
                        GridNavigationPath newJumpPath = new GridNavigationPath(
                            GridNavigationPathType.Jump,
                            navigationPointList.IndexOf(landingPoint),
                            navigationPointList.IndexOf(navigationPoint),
                            jumpPathCost,
                            jumpingMoveSpeed,
                            jumpPower
                        );
                        navigationPoint.AddLinkPath(newJumpPath);
                    }
                }
            }
        }
    }

    #endregion

    #region PrivateMethods

    /// <summary>
    /// TileMapのセル座標からセンタリングされたワールド座標に変換する
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    private Vector3 TileMapCellToWorldCenteringXPos(Vector3Int cellPos)
    {
        Vector3 cellWorldPos = platformerTilemap.CellToWorld(cellPos);
        Vector3 cellHalfSize = platformerTilemap.cellSize / 2;
        cellHalfSize.y = 0;

        return cellWorldPos + cellHalfSize;
    }

    /// <summary>
    /// 追跡者が特定の経路点での中心座標を計算する
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    private Vector3 CalcAgentCenterPos(Vector3 position)
    {
        Vector3 agentCenterPos = position;
        Vector2 agentColliderHalfSize = agentColliderSize / 2;

        agentCenterPos.y += agentColliderHalfSize.y;

        return agentCenterPos;
    }

    /// <summary>
    /// 追跡者が特定の座標にいる時ステージと接触するか
    /// </summary>
    /// <param name="position"></param>
    /// <param name="overrideSize"></param>
    /// <returns></returns>
    private bool IsCollisionAgentToStageByPos(Vector3 position, Vector3? overrideSize = null)
    {
        Vector3 offsetAgentColliderSize = GetOffsettingAgentColliderSize();
        if (overrideSize != null)
        {
            offsetAgentColliderSize = (Vector3)overrideSize;
        }
        Vector3 agentCenterPos = CalcAgentCenterPos(position);

        int overlapedCount = Physics2D.OverlapBoxNonAlloc(agentCenterPos, offsetAgentColliderSize, 0, stageOverlapColliderBuffer, 1 << LayerTagUtil.LayerNumberStage);

        return overlapedCount != 0;
    }

    /// <summary>
    /// 少しマージンを取った追跡者のコライダーサイズを取得する
    /// </summary>
    /// <returns></returns>
    private Vector3 GetOffsettingAgentColliderSize()
    {
        Vector3 offsettingAgentColliderSize = agentColliderSize;
        offsettingAgentColliderSize.x -= agentCollisionOffset;
        offsettingAgentColliderSize.y -= agentCollisionOffset;

        return offsettingAgentColliderSize;
    }

    /// <summary>
    /// 重力を考慮した着地地点にある経路点を取得する
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="startVector"></param>
    /// <param name="startPoint"></param>
    /// <returns></returns>
    private GridNavigationPoint GetLandingPointOnPhysicsSimulate(Vector3 startPos, Vector3 startVector, GridNavigationPoint startPoint)
    {
        int currentStepCount = 1;
        Vector3 simulatePos = Vector3.zero;
        Vector3 beforeSimulatePos = startPos;
        while (currentStepCount <= maxPhysicsSimulateStep)
        {
            float simulateTime = currentStepCount * Time.fixedDeltaTime * simulateStepScale;

            float halfGravity = Physics2D.gravity.y * agentGravityScale * 0.5f;
            float gravityVector = halfGravity * Mathf.Pow(simulateTime, 2.0f);
            float simulateVectorY = startVector.y + gravityVector;

            simulatePos.x = startPos.x + simulateTime * startVector.x;
            simulatePos.z = startPos.z + simulateTime * startVector.z;
            simulatePos.y = startPos.y + simulateTime * startVector.y + gravityVector;

            bool collisionAgent = IsCollisionAgentToStageByPos(simulatePos, agentColliderSize);
            // ステージに当たるまでシミュレーションを続ける
            if (!collisionAgent)
            {
                currentStepCount++;
                beforeSimulatePos = simulatePos;
                continue;
            }
            // 降下が始まっていない場合は無効
            Vector2 diffStepPos = beforeSimulatePos - simulatePos;
            if (diffStepPos.y < 0) return null;

            int castCount = Physics2D.LinecastNonAlloc(beforeSimulatePos, simulatePos, stageRaycastHitBuffer);
            // 衝突した足場が見つからない場合は無効
            if (castCount == 0) return null;

            Vector3 landingPos = stageRaycastHitBuffer[0].point;
            landingPos.y += agentCollisionOffset;

            Vector3Int landingCellPos = platformerTilemap.WorldToCell(landingPos);
            GridNavigationPoint landingPoint = GetNavigationPointByCellPos(landingCellPos, navigationPointList);
            // 着地した場所に経路点がなかった場合は無効
            if (landingPoint == null) return null;
            // 着地した場所の経路点がスタート点と同じ足場の場合は無効
            if (startPoint.PlatformIndex == landingPoint.PlatformIndex) return null;
            // 着地した場所に追跡者が立った時にステージと衝突する場合は無効
            if (IsCollisionAgentToStageByPos(landingPoint.WorldPos)) return null;

            return landingPoint;
        }

        return null;
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        foreach(GridNavigationPoint navigationPoint in GetNavigationPointList())
        {
            // 経路点を描画する
            switch (navigationPoint.Type)
            {
                case GridNavigationPointType.LeftEdge:
                case GridNavigationPointType.RightEdge:
                    Gizmos.color = Color.green;
                    break;
                case GridNavigationPointType.Platform:
                    Gizmos.color = Color.red;
                    break;
                case GridNavigationPointType.Solo:
                    Gizmos.color = Color.blue;
                    break;
            }
            Gizmos.DrawSphere(navigationPoint.WorldPos, 0.25f);
            // 歩ける経路を描画する
            foreach(GridNavigationPath navigationPath in navigationPoint.GetLinkPathList())
            {
                Vector3 lineStartPos = navigationPoint.WorldPos;
                Vector3 lineEndPos = navigationPath.GetToPoint(GetNavigationPointList()).WorldPos;
                Vector3 lineArrowLeftPos = lineEndPos + Quaternion.Euler(0, 0, 20) * (lineStartPos - lineEndPos).normalized * 0.5f;
                Vector3 lineArrowRightPos = lineEndPos + Quaternion.Euler(0, 0, -20) * (lineStartPos - lineEndPos).normalized * 0.5f;
                switch (navigationPath.Type)
                {
                    case GridNavigationPathType.Walk:
                        if (!showWalkPath) break;
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawLine(lineStartPos, lineEndPos);
                        Gizmos.DrawLine(lineEndPos, lineArrowLeftPos);
                        Gizmos.DrawLine(lineEndPos, lineArrowRightPos);
                        break;
                    case GridNavigationPathType.Fall:
                        if (!showFallPath) break;
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawLine(lineStartPos, lineEndPos);
                        Gizmos.DrawLine(lineEndPos, lineArrowLeftPos);
                        Gizmos.DrawLine(lineEndPos, lineArrowRightPos);
                        break;
                    case GridNavigationPathType.Jump:
                        if (!showJumpPath) break;
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(lineStartPos, lineEndPos);
                        Gizmos.DrawLine(lineEndPos, lineArrowLeftPos);
                        Gizmos.DrawLine(lineEndPos, lineArrowRightPos);
                        break;
                }
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GridPlatformerNavigator))]
public class GridPlatFormerNavigatorEditor : Editor
{
    private GridPlatformerNavigator targetObj = null;

    public void OnEnable()
    {
        targetObj = target as GridPlatformerNavigator;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Bake Navigation"))
        {
            if (targetObj == null) return;

            serializedObject.FindProperty("navigationData").objectReferenceValue = targetObj.BakeNavigation();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif

[System.Serializable]
public class GridNavigationPoint
{
    public GridNavigationPointType Type { get { return type; } }
    [SerializeField]
    private GridNavigationPointType type = GridNavigationPointType.None;

    public int PlatformIndex { get { return platformIndex; } }
    [SerializeField]
    private int platformIndex = 0;

    public Vector3Int CellPos { get { return cellPos; } }
    [SerializeField]
    private Vector3Int cellPos = Vector3Int.zero;

    public Vector3 WorldPos { get { return worldPos; } }
    [SerializeField]
    private Vector3 worldPos = Vector3.zero;
    [SerializeField]
    private List<GridNavigationPath> linkPathList = new List<GridNavigationPath>();

    public GridNavigationPoint(
        GridNavigationPointType type,
        int platformIndex,
        Vector3Int cellPos,
        Vector3 worldPos
    )
    {
        this.type = type;
        this.platformIndex = platformIndex;
        this.cellPos = cellPos;
        this.worldPos = worldPos;
    }

    public override int GetHashCode()
    {
        return cellPos.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        GridNavigationPoint point = obj as GridNavigationPoint;
        if (point == null) return false;

        return cellPos == point.cellPos;
    }

    /// <summary>
    /// この点の経路を追加する
    /// </summary>
    /// <param name="navigationPath"></param>
    public void AddLinkPath(GridNavigationPath navigationPath)
    {
        // 同じタイプで同じ目標のパスだった場合は追加しない
        if (linkPathList.Exists(path => path.Type == navigationPath.Type && path.ToPointIndex == navigationPath.ToPointIndex)) return;

        linkPathList.Add(navigationPath);
    }

    /// <summary>
    /// この点の経路を全て取得する
    /// </summary>
    /// <returns></returns>
    public List<GridNavigationPath> GetLinkPathList()
    {
        return linkPathList;
    }

    /// <summary>
    /// 経路候補のスコア計算を行いつつリストに追加する
    /// </summary>
    /// <param name="openPathList"></param>
    /// <param name="closePathList"></param>
    /// <param name="targetPoint"></param>
    /// <returns></returns>
    public List<GridNavigationPath> AddOpenLinkPathWithCalcScore(
        List<GridNavigationPath> openPathList, GridNavigationPoint targetPoint, GridNavigationPath fromPath = null, List<GridNavigationPoint> pointList = null
    )
    {
        foreach(GridNavigationPath path in linkPathList)
        {
            path.CalcNavigationScore(targetPoint, fromPath, pointList);
            openPathList.Add(path);
        }

        return openPathList;
    }

    /// <summary>
    /// 特定の経路点に向かうパスがあるか
    /// </summary>
    /// <param name="toPoint"></param>
    /// <param name="pointList"></param>
    /// <returns></returns>
    public bool IsExistToPointPath(GridNavigationPoint toPoint, List<GridNavigationPoint> pointList = null)
    {
        bool exist = false;
        foreach (GridNavigationPath path in linkPathList)
        {
            if (path.GetToPoint(pointList).Equals(toPoint))
            {
                exist = true;
                break;
            }
        }

        return exist;
    }
}

[System.Serializable]
public class GridNavigationPath
{
    public GridNavigationPathType Type { get { return type; } }
    [SerializeField]
    private GridNavigationPathType type = GridNavigationPathType.Walk;
    public int ToPointIndex { get { return toPointIndex; } }
    [SerializeField]
    private int toPointIndex = 0;
    public int FromPointIndex { get { return FromPointIndex; } }
    [SerializeField]
    private int fromPointIndex = 0;
    public int Cost { get { return cost; } }
    [SerializeField]
    private int cost = 0;

    public float MoveSpeed { get { return moveSpeed; } }
    [SerializeField]
    private float moveSpeed = 0;
    public float JumpSpeed { get { return jumpSpeed; } }
    [SerializeField]
    private float jumpSpeed = 0;

    public int NavigationScore { get; private set; } = 0;
    public GridNavigationPath FromPath { get; private set; } = null;

    public GridNavigationPath(
        GridNavigationPathType type,
        int toPointIndex,
        int fromPointIndex,
        int cost,
        float moveSpeed = 0,
        float jumpSpeed = 0
    )
    {
        this.type = type;
        this.toPointIndex = toPointIndex;
        this.fromPointIndex = fromPointIndex;
        this.cost = cost;
        this.moveSpeed = moveSpeed;
        this.jumpSpeed = jumpSpeed;
    }

    /// <summary>
    /// 経路点のインスタンスを取得する
    /// </summary>
    /// <returns></returns>
    public GridNavigationPoint GetToPoint(List<GridNavigationPoint> pointList = null)
    {
        if (pointList == null)
        {
            pointList = GameSceneController.Instance.Navigator.GetNavigationPointList();
        }
        return pointList[toPointIndex];
    }

    /// <summary>
    /// 経路基点のインスタンスを取得する
    /// </summary>
    /// <returns></returns>
    public GridNavigationPoint GetFromPoint(List<GridNavigationPoint> pointList = null)
    {
        if (pointList == null)
        {
            pointList = GameSceneController.Instance.Navigator.GetNavigationPointList();
        }
        return pointList[fromPointIndex];
    }

    /// <summary>
    /// この経路のスコアを計算し、NavigationScoreに代入する
    /// </summary>
    /// <param name="currentCost"></param>
    /// <param name="targetPoint"></param>
    public void CalcNavigationScore(GridNavigationPoint targetPoint, GridNavigationPath fromPath = null, List<GridNavigationPoint> pointList = null)
    {
        GridNavigationPoint toPoint = GetToPoint(pointList);
        Vector3Int diffCellPos = toPoint.CellPos - targetPoint.CellPos;
        int distance = Mathf.Abs(diffCellPos.x) + Mathf.Abs(diffCellPos.y);

        FromPath = fromPath;

        NavigationScore = CalcNavigationCost() + distance;
    }

    /// <summary>
    /// このパスまでの経路コストを再帰的に計算する
    /// </summary>
    /// <param name="currentCost"></param>
    /// <returns></returns>
    public int CalcNavigationCost(int currentCost = 0)
    {
        currentCost += Cost;

        if (FromPath != null)
        {
            return FromPath.CalcNavigationCost(currentCost);
        }

        return currentCost;
    }

    /// <summary>
    /// このパスまでの経路を再帰的に計算する
    /// </summary>
    /// <param name="untilPathList"></param>
    /// <returns></returns>
    public List<GridNavigationPath> GetUntilPathList(List<GridNavigationPath> untilPathList)
    {
        untilPathList.Add(this);

        if (FromPath != null)
        {
            return FromPath.GetUntilPathList(untilPathList);
        }

        return untilPathList;
    }
}

public enum GridNavigationPointType
{
    None,
    Platform,       // 両端に接続できる足場
    LeftEdge,       // 左端の足場
    RightEdge,      // 右端の足場
    Solo,           // 両端に接続できない足場
}

public enum GridNavigationPathType
{
    Walk,           // 歩いて移動するパス
    Fall,           // 落ちて移動するパス
    Jump,           // ジャンプして移動するパス
}