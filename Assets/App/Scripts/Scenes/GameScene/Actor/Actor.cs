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
    /// �ڒn���Ă��邩
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
    /// �R���C�_�[�̃T�C�Y���擾����
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMyColliderSize()
    {
        return myCollider.size;
    }

    /// <summary>
    /// �v���C���[�̌�����ς���
    /// </summary>
    /// <param name="direction"></param>
    public void ChangeActorDirection(ActorDirection direction)
    {
        CurrentDirection = direction;
        // �g�̃p�[�c�̌��������E�ŕς���
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
