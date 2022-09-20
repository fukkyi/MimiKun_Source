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
    /// �o�H�T�������x�C�N����
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
    /// �o�H�����A�Z�b�g�Ƃ��č쐬����
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
    /// �S�Ă̌o�H�_���擾����
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
    /// �O���b�h�̃Z�����W����|�C���g���擾����
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
    /// ���[���h���W����|�C���g���擾����
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public GridNavigationPoint GetNavigationPointByWorldPos(Vector3 worldPos)
    {
        Vector3Int cellPos = platformerTilemap.WorldToCell(worldPos);
        GridNavigationPoint originPosPoint = GetNavigationPointByCellPos(cellPos);

        if (originPosPoint != null) return originPosPoint;
        // ����M���M���ɗ����Ă��āA���ꂪ����̂Ɍo�H�_���Ȃ��ꍇ��z�肵���o�H�_���擾����
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
    /// �ڕW�_�܂ł̌o�H���v�Z����
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

        // �ǐՌ��ƒǐՑΏۂ��������W�̏ꍇ�͌v�Z���Ȃ�
        if (fromPoint.CellPos == targetPoint.CellPos) return null;

        List<GridNavigationPoint> navigationPointList = GetNavigationPointList();
        List<GridNavigationPath> openPathList = new List<GridNavigationPath>();
        HashSet<GridNavigationPoint> closePointList = new HashSet<GridNavigationPoint>();

        openPathList = fromPoint.AddOpenLinkPathWithCalcScore(openPathList, targetPoint, null, navigationPointList);
        closePointList.Add(fromPoint);

        while(openPathList.Count > 0)
        {
            GridNavigationPath bestPath = openPathList[0];
            // ��ԃX�R�A���Ⴂ�o�H���擾����
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
                // �ċA�I�Ɏ擾�����p�X�́A�ڕW�_�����_�ɒH�鏇�Ԃœ���ׁA�v�f�̏��Ԃ��t�]������
                navigatePathList.Reverse();

                return navigatePathList;
            }
            // �����B�̌o�H�_�̏ꍇ
            if (!closePointList.Contains(nextPoint))
            {
                // ���̃p�X��ǉ����A�X�R�A�̏����ŕ��ѕς���
                openPathList = nextPoint.AddOpenLinkPathWithCalcScore(openPathList, targetPoint, bestPath, navigationPointList);
                closePointList.Add(nextPoint);
            }
        }


        return null;
    }

    #endregion

    #region GenerateNavigation
    /// <summary>
    /// �o�H�T���̌��ƂȂ�_�𐶐�����
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
                // ���݂̃O���b�h���󔒂ŁA���̃O���b�h�Ƀ^�C��������ꍇ�̂ݓ_��ǉ�����
                if (hasTileGrid || !hasTileDownGrid) continue;

                bool hasTileRightGrid = platformerTilemap.HasTile(gridPos + Vector3Int.right);
                bool hasTileDownRightGrid = platformerTilemap.HasTile(gridPos + Vector3Int.down + Vector3Int.right);
                GridNavigationPointType pointType;
                // ���ꂷ�łɎn�܂��Ă���ꍇ
                if (platformStarted)
                {
                    // �E�̃O���b�h�Ƀ^�C�������邩�A�E���̃O���b�h���󔒂ł���ꍇ
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
                    // �E�̃O���b�h�Ƀ^�C�������邩�A�E���̃O���b�h���󔒂ł���ꍇ
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
                // ���ꂪ�r�؂ꂽ��C���f�b�N�X�����Z����
                if (!platformStarted)
                {
                    currentPlatformIndex++;
                }
            }
        }
    }

    /// <summary>
    /// �����ē��B�ł���o�H�𐶐�����
    /// </summary>
    private void GenerateWalkNavigationPath()
    {
        foreach(GridNavigationPoint navigationPoint in navigationPointList)
        {
            // ��_�͉��������̑傫���ŃX�e�[�W�����m����
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
    /// �������ē��B�ł���o�H�𐶐�����
    /// </summary>
    private void GenerateFallNavagationPath()
    {
        foreach(GridNavigationPoint navigationPoint in navigationPointList)
        {
            // �[�łȂ��ꍇ�͗����ł��Ȃ����ߌv�Z���Ȃ�
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

                // �����痎���ł��鑫��̏ꍇ
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

                // �E���痎���ł��鑫��̏ꍇ
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
    /// �W�����v���ē��B�ł���o�H�𐶐�����
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
                    // ���W�����v�̌o�H�v�Z
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

                    // �E�W�����v�̌o�H�v�Z
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
    /// TileMap�̃Z�����W����Z���^�����O���ꂽ���[���h���W�ɕϊ�����
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
    /// �ǐՎ҂�����̌o�H�_�ł̒��S���W���v�Z����
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
    /// �ǐՎ҂�����̍��W�ɂ��鎞�X�e�[�W�ƐڐG���邩
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
    /// �����}�[�W����������ǐՎ҂̃R���C�_�[�T�C�Y���擾����
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
    /// �d�͂��l���������n�n�_�ɂ���o�H�_���擾����
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
            // �X�e�[�W�ɓ�����܂ŃV�~�����[�V�����𑱂���
            if (!collisionAgent)
            {
                currentStepCount++;
                beforeSimulatePos = simulatePos;
                continue;
            }
            // �~�����n�܂��Ă��Ȃ��ꍇ�͖���
            Vector2 diffStepPos = beforeSimulatePos - simulatePos;
            if (diffStepPos.y < 0) return null;

            int castCount = Physics2D.LinecastNonAlloc(beforeSimulatePos, simulatePos, stageRaycastHitBuffer);
            // �Փ˂������ꂪ������Ȃ��ꍇ�͖���
            if (castCount == 0) return null;

            Vector3 landingPos = stageRaycastHitBuffer[0].point;
            landingPos.y += agentCollisionOffset;

            Vector3Int landingCellPos = platformerTilemap.WorldToCell(landingPos);
            GridNavigationPoint landingPoint = GetNavigationPointByCellPos(landingCellPos, navigationPointList);
            // ���n�����ꏊ�Ɍo�H�_���Ȃ������ꍇ�͖���
            if (landingPoint == null) return null;
            // ���n�����ꏊ�̌o�H�_���X�^�[�g�_�Ɠ�������̏ꍇ�͖���
            if (startPoint.PlatformIndex == landingPoint.PlatformIndex) return null;
            // ���n�����ꏊ�ɒǐՎ҂����������ɃX�e�[�W�ƏՓ˂���ꍇ�͖���
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
            // �o�H�_��`�悷��
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
            // ������o�H��`�悷��
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
    /// ���̓_�̌o�H��ǉ�����
    /// </summary>
    /// <param name="navigationPath"></param>
    public void AddLinkPath(GridNavigationPath navigationPath)
    {
        // �����^�C�v�œ����ڕW�̃p�X�������ꍇ�͒ǉ����Ȃ�
        if (linkPathList.Exists(path => path.Type == navigationPath.Type && path.ToPointIndex == navigationPath.ToPointIndex)) return;

        linkPathList.Add(navigationPath);
    }

    /// <summary>
    /// ���̓_�̌o�H��S�Ď擾����
    /// </summary>
    /// <returns></returns>
    public List<GridNavigationPath> GetLinkPathList()
    {
        return linkPathList;
    }

    /// <summary>
    /// �o�H���̃X�R�A�v�Z���s�����X�g�ɒǉ�����
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
    /// ����̌o�H�_�Ɍ������p�X�����邩
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
    /// �o�H�_�̃C���X�^���X���擾����
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
    /// �o�H��_�̃C���X�^���X���擾����
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
    /// ���̌o�H�̃X�R�A���v�Z���ANavigationScore�ɑ������
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
    /// ���̃p�X�܂ł̌o�H�R�X�g���ċA�I�Ɍv�Z����
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
    /// ���̃p�X�܂ł̌o�H���ċA�I�Ɍv�Z����
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
    Platform,       // ���[�ɐڑ��ł��鑫��
    LeftEdge,       // ���[�̑���
    RightEdge,      // �E�[�̑���
    Solo,           // ���[�ɐڑ��ł��Ȃ�����
}

public enum GridNavigationPathType
{
    Walk,           // �����Ĉړ�����p�X
    Fall,           // �����Ĉړ�����p�X
    Jump,           // �W�����v���Ĉړ�����p�X
}