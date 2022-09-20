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
    /// 初期化処理
    /// </summary>
    public void Init()
    {
        targetPlayer = GetComponent<Player>();

        InitInputAction();
        // ジャンプ入力遅延用のアクションを設定する
        jumpInputDelayTimer.action = () => { isEnabledInputtedJump = false; };
    }

    /// <summary>
    /// InputActionの初期化
    /// </summary>
    protected void InitInputAction()
    {
        InputAction = new BasicInputAction();

        InputAction.Game.Jump.performed += (context) => { OnInputJump(); };

        InputAction.Enable();
    }

    /// <summary>
    /// ジャンプボタンが入力された時のアクション
    /// </summary>
    protected void OnInputJump()
    {
        if (!targetPlayer.IsCanInputtedMove()) return;

        isEnabledInputtedJump = true;
        jumpInputDelayTimer.ResetTimer();
    }

    /// <summary>
    /// プレイヤー入力のジャンプアクションを更新する
    /// </summary>
    protected void UpdateInputJumpManage()
    {
        if (!isEnabledInputtedJump) return;
        // 接地していない場合はジャンプさせない
        if (!targetPlayer.IsGround()) return;

        Jump();
        isEnabledInputtedJump = false;
    }

    /// <summary>
    /// プレイヤーの入力によって移動を行う
    /// </summary>
    /// <param name="inputDirection"></param>
    public void MoveByInputDirection(Vector2 inputDirection)
    {
        Vector2 moveDirection = targetPlayer.MyRigidBody.velocity;
        // 横の入力がある場合
        if (Mathf.Abs(inputDirection.x) >= Mathf.Epsilon)
        {
            int inputSign = Math.Sign(inputDirection.x);
            moveDirection.x = inputSign * moveSpeed;

            ActorDirection inputPlayerDirection = inputSign == 1 ? ActorDirection.Right : ActorDirection.Left;
            // 入力からプレイヤーの方向を変える
            GameSceneController.Instance.Player.ChangeActorDirection(inputPlayerDirection);
        }
        else
        {
            moveDirection.x = 0;
        }

        targetPlayer.MyRigidBody.velocity = moveDirection;
    }

    /// <summary>
    /// ジャンプさせる
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
