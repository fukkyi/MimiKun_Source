using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerInputMover : MonoBehaviour
{
    public BasicInputAction InputAction { get; private set; }

    public float moveSpeed = 0;
    public float jumpHeight = 0;

    [SerializeField]
    private ActionTimer jumpInputDelayTimer = new ActionTimer();

    private Player targetPlayer = null;
    private bool isEnabledInputtedJump = false;

    void FixedUpdate()
    {
        if (InputAction == null) return;
        if (!targetPlayer.IsCanInputtedMove()) return;

        MoveByInputDirection(InputAction.Game.Move.ReadValue<Vector2>());
    }

    void Update()
    {
        jumpInputDelayTimer.UpdateTimer();
        UpdateInputJumpManage();
    }

    private void OnDestroy()
    {
        InputAction.Dispose();
    }

    /// <summary>
    /// ����������
    /// </summary>
    public void Init()
    {
        targetPlayer = GetComponent<Player>();

        InitInputAction();
        // �W�����v���͒x���p�̃A�N�V������ݒ肷��
        jumpInputDelayTimer.action = () => { isEnabledInputtedJump = false; };
    }

    /// <summary>
    /// InputAction�̏�����
    /// </summary>
    protected void InitInputAction()
    {
        InputAction = new BasicInputAction();

        InputAction.Game.Jump.performed += (context) => { OnInputJump(); };

        InputAction.Enable();
    }

    /// <summary>
    /// �W�����v�{�^�������͂��ꂽ���̃A�N�V����
    /// </summary>
    protected void OnInputJump()
    {
        if (!targetPlayer.IsCanInputtedMove()) return;

        isEnabledInputtedJump = true;
        jumpInputDelayTimer.ResetTimer();
    }

    /// <summary>
    /// �v���C���[���͂̃W�����v�A�N�V�������X�V����
    /// </summary>
    protected void UpdateInputJumpManage()
    {
        if (!isEnabledInputtedJump) return;
        // �ڒn���Ă��Ȃ��ꍇ�̓W�����v�����Ȃ�
        if (!targetPlayer.IsGround()) return;

        Jump();
        isEnabledInputtedJump = false;
    }

    /// <summary>
    /// �v���C���[�̓��͂ɂ���Ĉړ����s��
    /// </summary>
    /// <param name="inputDirection"></param>
    public void MoveByInputDirection(Vector2 inputDirection)
    {
        Vector2 moveDirection = targetPlayer.MyRigidBody.velocity;
        // ���̓��͂�����ꍇ
        if (Mathf.Abs(inputDirection.x) >= Mathf.Epsilon)
        {
            int inputSign = Math.Sign(inputDirection.x);
            moveDirection.x = inputSign * moveSpeed;

            ActorDirection inputPlayerDirection = inputSign == 1 ? ActorDirection.Right : ActorDirection.Left;
            // ���͂���v���C���[�̕�����ς���
            GameSceneController.Instance.Player.ChangeActorDirection(inputPlayerDirection);
        }
        else
        {
            moveDirection.x = 0;
        }

        targetPlayer.MyRigidBody.velocity = moveDirection;
    }

    /// <summary>
    /// �W�����v������
    /// </summary>
    public void Jump()
    {
        Vector2 jumpedDirection = targetPlayer.MyRigidBody.velocity;

        float jumpSpeed = Physics2DUtil.CalcInitialVelocityToReachHeight(jumpHeight, 0, targetPlayer.MyRigidBody.gravityScale);

        jumpedDirection.y = jumpSpeed;

        targetPlayer.MyRigidBody.velocity = jumpedDirection;

        AudioManager.Instance.PlaySE("MimikunJump");
    }
}
