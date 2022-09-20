using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GridPlatformerNavAgent : MonoBehaviour
{
    [SerializeField]
    private float calcNavInterval = 0.1f;
    [SerializeField]
    private float snapDistance = 0.05f;
    [SerializeField]
    private float agentMoveSpeed = 5.0f;
    [SerializeField]
    private float agentFallOffset = 0.01f;
    [SerializeField]
    private Vector2 agentColliderSize = new Vector2();
    [SerializeField]
    private Vector3 targetPos = new Vector3();

    private Actor targetActor = null;
    private Rigidbody2D myRigidbody = null;
    private ActionTimer calcNavPathTimer = new ActionTimer();
    private LineRenderer navLineRenderer = null;
    private List<GridNavigationPath> currentNavPathList = null;
    private Vector3 navStartPos = Vector3.zero;

    private int currentNavIndex = 0;
    private bool startedWalkToPoint = false;
    private bool startedHeightMove = false;

    private void Awake()
    {
        targetActor = GetComponent<Actor>();
        myRigidbody = GetComponent<Rigidbody2D>();
        navLineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitCalcPathActionTimer();
    }

    // Update is called once per frame
    void Update()
    {
        calcNavPathTimer.UpdateTimer();
        UpdateNavLine();
    }

    private void FixedUpdate()
    {
        NavigateToTarget();
    }

    /// <summary>
    /// 追跡対象をセットする
    /// </summary>
    /// <param name="targetPos"></param>
    public void SetNavTarget(Vector3 targetPos)
    {
        this.targetPos = targetPos;
    }

    /// <summary>
    /// 特定の座標までの横の距離を計算する
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public float CalcHorizontalDistanceToPos(Vector3 position)
    {
        return Mathf.Abs(position.x - transform.position.x);
    }

    /// <summary>
    /// 特定の座標に到達できるか調べる
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool CanNavigateToPos(Vector3 position)
    {
        return GameSceneController.Instance.Navigator.CalcNavigatePath(transform.position, position) != null;
    }

    /// <summary>
    /// 追跡対象に向かう経路を更新する
    /// </summary>
    /// <returns></returns>
    private void UpdateNavigatePathToTarget()
    {
        // 空中にいる間は経路を更新しない
        if (!targetActor.IsGround())
        {
            calcNavPathTimer.elapsedTime = calcNavInterval;
            return;
        }

        List<GridNavigationPath> newNavPathList = GameSceneController.Instance.Navigator.CalcNavigatePath(transform.position, targetPos);
        // 経路が計算出来なかった場合は更新しない
        if (newNavPathList == null) return;

        currentNavPathList = newNavPathList;
        navStartPos = GameSceneController.Instance.Navigator.GetNavigationPointByWorldPos(transform.position).WorldPos;
        currentNavIndex = 0;
        startedWalkToPoint = false;
        startedHeightMove = false;
    }

    /// <summary>
    /// 経路計算用のタイマーを初期化する
    /// </summary>
    private void InitCalcPathActionTimer()
    {
        calcNavPathTimer.activateTime = calcNavInterval;
        calcNavPathTimer.action = () => {
            UpdateNavigatePathToTarget();
        };
        calcNavPathTimer.repeatAction = true;
    }

    /// <summary>
    /// 経路情報を用いて目標点へ向かわせる
    /// </summary>
    private void NavigateToTarget()
    {
        GridNavigationPath currentMovePath = GetCurrentNavPath(currentNavIndex);

        if (currentMovePath == null) return;

        GridNavigationPoint currentMoveToPoint = currentMovePath.GetToPoint();
        GridNavigationPoint currentFromToPoint = currentMovePath.GetFromPoint();

        if (currentMovePath.Type == GridNavigationPathType.Walk)
        {
            if (IsReachPosByWalk(currentMoveToPoint.WorldPos))
            {
                currentNavIndex++;
                GridNavigationPath nextMovePath = GetCurrentNavPath(currentNavIndex);
                if (nextMovePath == null)
                {
                    // 目標点に到達
                    startedWalkToPoint = false;
                    myRigidbody.velocity = Vector2.zero;
                    return;
                }
                else if (nextMovePath.Type == GridNavigationPathType.Jump)
                {
                    // 次のパスがジャンプ経路の場合は移動を止める
                    myRigidbody.velocity = Vector3.zero;
                }
            }
            else 
            {
                SetVelocityToPointForWalk(currentMoveToPoint.WorldPos);
                startedWalkToPoint = true;
            }
        }
        else if (currentMovePath.Type == GridNavigationPathType.Fall)
        {
            if (startedHeightMove)
            {
                // ジャンプ直後の誤判定防止のために落下が始まっている状態から接地判定を行う
                if (targetActor.IsGround())
                {
                    currentNavIndex++;
                    GridNavigationPath nextMovePath = GetCurrentNavPath(currentNavIndex);

                    startedHeightMove = false;
                    myRigidbody.velocity = Vector2.zero;

                    if (nextMovePath == null)
                    {
                        // 目標点に到達
                        return;
                    }
                }
                else
                {
                    // 空中を移動中もベクトルをセットする
                    SetVelocityForAir(currentMovePath, currentMoveToPoint);
                }
            }
            else
            {
                Vector2 halfAgentSize = agentColliderSize / 2;
                Vector2 cellHalfSize = GameSceneController.Instance.Navigator.TileCellSize / 2;
                Vector3 fallPoint = currentFromToPoint.WorldPos;
                float fallOffset = halfAgentSize.x + cellHalfSize.x + agentFallOffset;
                if (currentFromToPoint.Type == GridNavigationPointType.LeftEdge)
                {
                    fallPoint.x -= fallOffset;
                }
                else if (currentFromToPoint.Type == GridNavigationPointType.RightEdge)
                {
                    fallPoint.x += fallOffset;
                }
                else if (currentFromToPoint.Type == GridNavigationPointType.Solo)
                {
                    // 単独の経路点の場合は、次の経路点の座標で降りる方向を決める
                    SetAgentDirectionByPos(currentMoveToPoint.WorldPos);
                    if (targetActor.CurrentDirection == ActorDirection.Left)
                    {
                        fallPoint.x -= fallOffset;
                    }
                    else if (targetActor.CurrentDirection == ActorDirection.Right)
                    {
                        fallPoint.x += fallOffset;
                    }
                }
                // 落下するためのポイントにいない場合は移動させる
                if (IsReachPosByWalk(fallPoint))
                {
                    // 経路の移動を正しく動作させるため、座標を代入する
                    transform.position = fallPoint;
                    startedWalkToPoint = false;
                    startedHeightMove = true;

                    SetVelocityToPointForFall(currentMovePath, currentMoveToPoint);
                }
                else
                {
                    SetVelocityToPointForWalk(fallPoint);
                    startedWalkToPoint = true;
                }
            }
        }
        else if (currentMovePath.Type == GridNavigationPathType.Jump)
        {
            if (startedHeightMove)
            {
                // ジャンプ直後の誤判定防止のために落下が始まっている状態から接地判定を行う
                if (myRigidbody.velocity.y <= 0 && targetActor.IsGround())
                {
                    currentNavIndex++;
                    GridNavigationPath nextMovePath = GetCurrentNavPath(currentNavIndex);

                    startedHeightMove = false;
                    myRigidbody.velocity = Vector2.zero;

                    if (nextMovePath == null)
                    {
                        // 目標点に到達
                        return;
                    }
                }
                else
                {
                    // 空中を移動中もベクトルをセットする
                    SetVelocityForAir(currentMovePath, currentMoveToPoint);
                }
            }
            else
            {
                // ジャンプするためのポイントにいない場合は移動させる
                if (IsReachPosByWalk(currentFromToPoint.WorldPos))
                {
                    // 経路の移動を正しく動作させるため、座標を代入する
                    transform.position = currentFromToPoint.WorldPos;
                    startedWalkToPoint = false;
                    startedHeightMove = true;

                    SetVelocityToPointByJump(currentMovePath, currentMoveToPoint);
                }
                else
                {
                    SetVelocityToPointForWalk(currentFromToPoint.WorldPos);
                    startedWalkToPoint = true;
                }
            }
        }
    }

    /// <summary>
    /// 現在の移動に使用している経路を取得する
    /// </summary>
    /// <returns></returns>
    private GridNavigationPath GetCurrentNavPath(int index)
    {
        if (currentNavPathList == null) return null;
        if (currentNavPathList.Count <= 0) return null;
        if (currentNavPathList.Count <= index || index < 0) return null;

        return currentNavPathList[index];
    }

    /// <summary>
    /// 特定の座標に向けて向きを変える
    /// </summary>
    /// <param name="point"></param>
    private void SetAgentDirectionByPos(Vector3 position)
    {
        if (transform.position.x > position.x)
        {
            targetActor.ChangeActorDirection(ActorDirection.Left);
        }
        else
        {
            targetActor.ChangeActorDirection(ActorDirection.Right);
        }
    }

    /// <summary>
    /// 特定の座標に歩いて移動させるようにベクトルをセットする
    /// </summary>
    private void SetVelocityToPointForWalk(Vector3 position)
    {
        SetAgentDirectionByPos(position);

        float moveVecX = agentMoveSpeed;
        if (targetActor.CurrentDirection == ActorDirection.Left)
        {
            moveVecX = -moveVecX;
        }

        Vector3 moveVec = myRigidbody.velocity;
        moveVec.x = moveVecX;

        myRigidbody.velocity = moveVec;
    }

    /// <summary>
    /// 特定のポイントに落下して移動させるようにベクトルをセットする
    /// </summary>
    /// <param name="jumpPath"></param>
    /// <param name="jumpToPoint"></param>
    private void SetVelocityToPointForFall(GridNavigationPath fallPath, GridNavigationPoint fallToPoint)
    {
        SetAgentDirectionByPos(fallToPoint.WorldPos);

        Vector3 fallVec = Vector3.right * fallPath.MoveSpeed;
        myRigidbody.velocity = fallVec;
    }

    /// <summary>
    /// 特定のポイントにジャンプして移動させるようにベクトルをセットする
    /// </summary>
    /// <param name="jumpPath"></param>
    /// <param name="jumpToPoint"></param>
    private void SetVelocityToPointByJump(GridNavigationPath jumpPath, GridNavigationPoint jumpToPoint)
    {
        SetAgentDirectionByPos(jumpToPoint.WorldPos);

        Vector3 jumpVec = Vector3.right * jumpPath.MoveSpeed + Vector3.up * jumpPath.JumpSpeed;
        myRigidbody.velocity = jumpVec;
    }

    /// <summary>
    /// 特定にポイントに向けて空中で移動できるようにベクトルをセットする
    /// </summary>
    /// <param name="airPath"></param>
    /// <param name="landingPoint"></param>
    private void SetVelocityForAir(GridNavigationPath airPath, GridNavigationPoint landingPoint)
    {
        SetAgentDirectionByPos(landingPoint.WorldPos);

        Vector3 airMoveVec = myRigidbody.velocity;
        airMoveVec.x = airPath.MoveSpeed;

        myRigidbody.velocity = airMoveVec;
    }

    /// <summary>
    /// 歩きで特定の座標に到達したか
    /// </summary>
    /// <param name="currentDistance"></param>
    /// <param name="beforeDistance"></param>
    /// <returns></returns>
    private bool IsReachPosByWalk(Vector3 position)
    {
        if (startedWalkToPoint)
        {
            if (targetActor.CurrentDirection == ActorDirection.Left)
            {
                return transform.position.x < position.x;
            }
            else if (targetActor.CurrentDirection == ActorDirection.Right)
            {
                return transform.position.x > position.x;
            }
        }
        else
        {
            return CalcHorizontalDistanceToPos(position) < snapDistance;
        }

        return false;
    }

    private void UpdateNavLine()
    {
        if (navLineRenderer == null) return;
        if (currentNavPathList == null || currentNavPathList.Count == 0) return;

        int linePosZ = -1;

        Vector3 currentPointPos = navStartPos;
        currentPointPos.z = linePosZ;

        int pointIndex = 0;
        navLineRenderer.positionCount = currentNavPathList.Count + 1;
        navLineRenderer.SetPosition(pointIndex, currentPointPos);

        foreach (GridNavigationPath path in currentNavPathList)
        {
            Vector3 lineStartPos = currentPointPos;
            Vector3 lineEndPos = path.GetToPoint().WorldPos;

            lineStartPos.z = linePosZ;
            lineEndPos.z = linePosZ;

            currentPointPos = lineEndPos;

            switch (path.Type)
            {
                case GridNavigationPathType.Walk:
                    break;
                case GridNavigationPathType.Fall:
                    break;
                case GridNavigationPathType.Jump:
                    break;
            }

            pointIndex++;
            navLineRenderer.SetPosition(pointIndex, lineEndPos);
        }
    }

    private void OnDrawGizmos()
    {
        if (currentNavPathList == null || currentNavPathList.Count == 0) return;

        Vector3 currentPointPos = navStartPos;

        foreach (GridNavigationPath path in currentNavPathList)
        {
            Vector3 lineStartPos = currentPointPos;
            Vector3 lineEndPos = path.GetToPoint().WorldPos;

            currentPointPos = lineEndPos;

            switch (path.Type)
            {
                case GridNavigationPathType.Walk:
                    Gizmos.color = Color.cyan;
                    break;
                case GridNavigationPathType.Fall:
                    Gizmos.color = Color.magenta;
                    break;
                case GridNavigationPathType.Jump:
                    Gizmos.color = Color.yellow;
                    break;
            }

            Gizmos.DrawLine(lineStartPos, lineEndPos);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(targetPos, 0.5f);
    }
}
