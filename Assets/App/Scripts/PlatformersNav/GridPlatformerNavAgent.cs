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
    /// �ǐՑΏۂ��Z�b�g����
    /// </summary>
    /// <param name="targetPos"></param>
    public void SetNavTarget(Vector3 targetPos)
    {
        this.targetPos = targetPos;
    }

    /// <summary>
    /// ����̍��W�܂ł̉��̋������v�Z����
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public float CalcHorizontalDistanceToPos(Vector3 position)
    {
        return Mathf.Abs(position.x - transform.position.x);
    }

    /// <summary>
    /// ����̍��W�ɓ��B�ł��邩���ׂ�
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool CanNavigateToPos(Vector3 position)
    {
        return GameSceneController.Instance.Navigator.CalcNavigatePath(transform.position, position) != null;
    }

    /// <summary>
    /// �ǐՑΏۂɌ������o�H���X�V����
    /// </summary>
    /// <returns></returns>
    private void UpdateNavigatePathToTarget()
    {
        // �󒆂ɂ���Ԃ͌o�H���X�V���Ȃ�
        if (!targetActor.IsGround())
        {
            calcNavPathTimer.elapsedTime = calcNavInterval;
            return;
        }

        List<GridNavigationPath> newNavPathList = GameSceneController.Instance.Navigator.CalcNavigatePath(transform.position, targetPos);
        // �o�H���v�Z�o���Ȃ������ꍇ�͍X�V���Ȃ�
        if (newNavPathList == null) return;

        currentNavPathList = newNavPathList;
        navStartPos = GameSceneController.Instance.Navigator.GetNavigationPointByWorldPos(transform.position).WorldPos;
        currentNavIndex = 0;
        startedWalkToPoint = false;
        startedHeightMove = false;
    }

    /// <summary>
    /// �o�H�v�Z�p�̃^�C�}�[������������
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
    /// �o�H����p���ĖڕW�_�֌����킹��
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
                    // �ڕW�_�ɓ��B
                    startedWalkToPoint = false;
                    myRigidbody.velocity = Vector2.zero;
                    return;
                }
                else if (nextMovePath.Type == GridNavigationPathType.Jump)
                {
                    // ���̃p�X���W�����v�o�H�̏ꍇ�͈ړ����~�߂�
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
                // �W�����v����̌딻��h�~�̂��߂ɗ������n�܂��Ă����Ԃ���ڒn������s��
                if (targetActor.IsGround())
                {
                    currentNavIndex++;
                    GridNavigationPath nextMovePath = GetCurrentNavPath(currentNavIndex);

                    startedHeightMove = false;
                    myRigidbody.velocity = Vector2.zero;

                    if (nextMovePath == null)
                    {
                        // �ڕW�_�ɓ��B
                        return;
                    }
                }
                else
                {
                    // �󒆂��ړ������x�N�g�����Z�b�g����
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
                    // �P�Ƃ̌o�H�_�̏ꍇ�́A���̌o�H�_�̍��W�ō~�����������߂�
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
                // �������邽�߂̃|�C���g�ɂ��Ȃ��ꍇ�͈ړ�������
                if (IsReachPosByWalk(fallPoint))
                {
                    // �o�H�̈ړ��𐳂������삳���邽�߁A���W��������
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
                // �W�����v����̌딻��h�~�̂��߂ɗ������n�܂��Ă����Ԃ���ڒn������s��
                if (myRigidbody.velocity.y <= 0 && targetActor.IsGround())
                {
                    currentNavIndex++;
                    GridNavigationPath nextMovePath = GetCurrentNavPath(currentNavIndex);

                    startedHeightMove = false;
                    myRigidbody.velocity = Vector2.zero;

                    if (nextMovePath == null)
                    {
                        // �ڕW�_�ɓ��B
                        return;
                    }
                }
                else
                {
                    // �󒆂��ړ������x�N�g�����Z�b�g����
                    SetVelocityForAir(currentMovePath, currentMoveToPoint);
                }
            }
            else
            {
                // �W�����v���邽�߂̃|�C���g�ɂ��Ȃ��ꍇ�͈ړ�������
                if (IsReachPosByWalk(currentFromToPoint.WorldPos))
                {
                    // �o�H�̈ړ��𐳂������삳���邽�߁A���W��������
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
    /// ���݂̈ړ��Ɏg�p���Ă���o�H���擾����
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
    /// ����̍��W�Ɍ����Č�����ς���
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
    /// ����̍��W�ɕ����Ĉړ�������悤�Ƀx�N�g�����Z�b�g����
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
    /// ����̃|�C���g�ɗ������Ĉړ�������悤�Ƀx�N�g�����Z�b�g����
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
    /// ����̃|�C���g�ɃW�����v���Ĉړ�������悤�Ƀx�N�g�����Z�b�g����
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
    /// ����Ƀ|�C���g�Ɍ����ċ󒆂ňړ��ł���悤�Ƀx�N�g�����Z�b�g����
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
    /// �����œ���̍��W�ɓ��B������
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
