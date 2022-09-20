using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public ActorDirection CurrentDirection { get; protected set; } = ActorDirection.Right;

    [SerializeField]
    protected Transform bodyTrans = null;
    [SerializeField]
    protected BoxCollider2D myCollider = null;
    [SerializeField]
    protected float groundDetectDistance = 0.01f;

    protected RaycastHit2D[] groundHitResult = new RaycastHit2D[1];

    /// <summary>
    /// 接地しているか
    /// </summary>
    /// <returns></returns>
    public bool IsGround()
    {
        Vector2 groundRayOrigin = myCollider.transform.position;
        groundRayOrigin += myCollider.offset * bodyTrans.localScale;
        Vector2 groundRaySize = myCollider.size * bodyTrans.localScale;

        int groundHitCount = Physics2D.BoxCastNonAlloc(groundRayOrigin, groundRaySize, 0, Vector2.down, groundHitResult, groundDetectDistance, LayerTagUtil.GetLayerMaskIgnoreCharacter());

        return groundHitCount != 0;
    }

    /// <summary>
    /// コライダーのサイズを取得する
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMyColliderSize()
    {
        return myCollider.size;
    }

    /// <summary>
    /// プレイヤーの向きを変える
    /// </summary>
    /// <param name="direction"></param>
    public void ChangeActorDirection(ActorDirection direction)
    {
        CurrentDirection = direction;
        // 身体パーツの向きを左右で変える
        if (CurrentDirection == ActorDirection.Left)
        {
            bodyTrans.rotation = Quaternion.Euler(0, -180, 0);
        }
        else if (CurrentDirection == ActorDirection.Right)
        {
            bodyTrans.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
