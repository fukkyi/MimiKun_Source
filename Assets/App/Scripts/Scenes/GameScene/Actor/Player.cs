using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputMover))]
public class Player : Actor
{
    public Rigidbody2D MyRigidBody { get; private set; } = null;
    public JewelryPossessionStatus JewelryPossessionStatus { get; } = new JewelryPossessionStatus();
    public int CurrentHp { get; private set; }
    public MimiKunMobility CurrentMobility { get; private set; } = null;
    public bool IsDead { get; private set; }

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private MasterDataMimiKun playerMasterData = null;
    [SerializeField]
    private CircleCollider2D itemGetCollider = null;
    [SerializeField]
    private Transform jewelryDropPoint = null;
    [SerializeField]
    private Vector2 knockBackDirection = new Vector2();
    [SerializeField]
    private Vector2 dropDirection = new Vector2();
    [SerializeField]
    private float dropPower = 5.0f;
    [SerializeField]
    private float damageDisabledTime = 2.0f;
    [SerializeField]
    private int maxTrapHaveCount = 1;
    [SerializeField]
    private int maxPlayerHp = 3;

    private PlayerInputMover inputMover = null;
    private Collider2D[] overlapItemBuffer = new Collider2D[10];
    private ActionTimer knockBackTimer = new ActionTimer();
    private ActionTimer damageEnableTimer = new ActionTimer();
    private Vector2 currentKnockBackVec = new Vector2();

    private bool knockBacking = false;
    private bool enableDamage = true;
    private int trapHaveCount = 0;

    private void Awake()
    {
        MyRigidBody = GetComponent<Rigidbody2D>();
        inputMover = GetComponent<PlayerInputMover>();

        CurrentHp = maxPlayerHp;

        GameSceneController.Instance.GameSceneUICanvas.PlayerHpUI.InitGauge(maxPlayerHp);
        GameSceneController.Instance.GameSceneUICanvas.TrapHaveUI.UpdateIcon(trapHaveCount);

        inputMover.Init();
        AddActionPlayerInput();
    }

    // Start is called before the first frame update
    private void Start()
    {
        InitTimers();
        InitMobility();
    }

    // Update is called once per frame
    private void Update()
    {
        CheckItemGetCollider();     
        if (knockBacking)
        {
            UpdateMyVelocityForKnockBack();
            knockBackTimer.UpdateTimer();
        }
        damageEnableTimer.UpdateTimer();
        UpdateAnimate();
    }

    private void OnDestroy()
    {
        inputMover.InputAction.Dispose();
    }

    /// <summary>
    /// 移動入力コンポーネントにプレイヤーアクションを追加する
    /// </summary>
    private void AddActionPlayerInput()
    {
        inputMover.InputAction.Game.DropJewelry.performed += (context) => { DropJewelryToBack(); };
        inputMover.InputAction.Game.DropTrap.performed += (context) => { DropTrapToBack(); };
        inputMover.InputAction.Game.JewelrySelectLeft.performed += (context) => { SelectJewelryToLeft(); };
        inputMover.InputAction.Game.JewelrySelectRight.performed += (context) => { SelectJewelryToRight(); };
    }

    /// <summary>
    /// 各種タイマーを初期化する
    /// </summary>
    private void InitTimers()
    {
        knockBackTimer.activateTime = 0;
        knockBackTimer.action = () =>
        {
            knockBacking = false;
            animator.SetBool("Damage", false);
        };
        damageEnableTimer.activateTime = damageDisabledTime;
        damageEnableTimer.action = () =>
        {
            enableDamage = true;
            myCollider.gameObject.layer = LayerTagUtil.LayerNumberPlayer;
        };
    }

    /// <summary>
    /// 機動力のステータスを初期化する
    /// </summary>
    private void InitMobility()
    {
        CurrentMobility = playerMasterData.mobilityTable.SelectMobilityByJewelryCount(JewelryPossessionStatus.GetTotalJewelryCount());
    }

    /// <summary>
    /// 宝石を後ろへドロップする
    /// </summary>
    private void DropJewelryToBack()
    {
        if (!IsCanInputtedMove()) return;

        JewelryType dropJewelryType = GameSceneController.Instance.GameSceneUICanvas.JewelrySelectUI.nowJewelry;
        if (JewelryPossessionStatus.GetJewelryCountByType(dropJewelryType) <= 0) return;

        Jewelry dropJewelry = Instantiate(GameSceneController.Instance.JewelryData.GetJewelryObjByType(dropJewelryType), jewelryDropPoint.position, Quaternion.identity);

        Vector2 dropVector = dropDirection;
        if (CurrentDirection == ActorDirection.Right)
        {
            dropVector.x = -dropVector.x;
        }

        dropJewelry.DropToDirection(dropVector, dropPower);

        AddJewelryByType(dropJewelryType, -1);
    }

    /// <summary>
    /// 罠を後ろへドロップする
    /// </summary>
    private void DropTrapToBack()
    {
        if (!IsCanInputtedMove()) return;
        if (trapHaveCount <= 0) return;

        Trap dropTrap = Instantiate(GameSceneController.Instance.TrapObj, jewelryDropPoint.position, Quaternion.identity);

        Vector2 dropVector = dropDirection;
        if (CurrentDirection == ActorDirection.Right)
        {
            dropVector.x = -dropVector.x;
        }

        dropTrap.DropAndActivateToDirection(dropVector, dropPower);

        AddTrap(-1);
    }

    /// <summary>
    /// 宝石選択UIの選択している宝石を左方向から変える
    /// </summary>
    private void SelectJewelryToLeft()
    {
        if (!GameSceneController.Instance.IsCanPlayerInput()) return;

        GameSceneController.Instance.GameSceneUICanvas.JewelrySelectUI.leftChangeJewelry();
    }

    /// <summary>
    /// 宝石選択UIの選択している宝石を右方向から変える
    /// </summary>
    private void SelectJewelryToRight()
    {
        if (!GameSceneController.Instance.IsCanPlayerInput()) return;

        GameSceneController.Instance.GameSceneUICanvas.JewelrySelectUI.rightChangeJewelry();
    }

    /// <summary>
    /// ノックバック中の移動ベクトルを更新する
    /// </summary>
    private void UpdateMyVelocityForKnockBack()
    {
        Vector3 currentVec = MyRigidBody.velocity;
        float knockBackSpeedRate = knockBackTimer.GetElapsedRate();
        // ノックバックスピードはイージングさせる
        float easingSpeedRate = (1 - knockBackSpeedRate) * (1 - knockBackSpeedRate);

        currentVec.x = currentKnockBackVec.x * easingSpeedRate;

        MyRigidBody.velocity = currentVec;
    }

    /// <summary>
    /// プレイヤーのアニメーションを更新する
    /// </summary>
    private void UpdateAnimate()
    {
        if (!GameSceneController.Instance.IsCanPlayerInput()) return;

        animator.SetFloat("VelocityY", MyRigidBody.velocity.y);

        bool moving = inputMover.InputAction.Game.Move.ReadValue<Vector2>().x != 0;
        animator.SetBool("Run", moving);
    }

    /// <summary>
    /// アイテムが取得範囲にあるかチェックする
    /// </summary>
    private void CheckItemGetCollider()
    {
        if (itemGetCollider == null) return;

        int itemOverlapedCount = Physics2D.OverlapCircleNonAlloc(transform.position, itemGetCollider.radius, overlapItemBuffer, LayerTagUtil.GetLayerMaskItem());

        if (itemOverlapedCount == 0) return;

        for (int i = 0; i < itemOverlapedCount; i++)
        {
            Item overlapedItem = overlapItemBuffer[i].GetComponentInParent<Item>();
            overlapedItem.ReceivedByPlayer(this);
        }
    }

    /// <summary>
    /// 罠の所持数を増やす
    /// </summary>
    /// <param name="addCount"></param>
    public void AddTrap(int addCount = 1)
    {
        trapHaveCount = Mathf.Clamp(trapHaveCount + addCount, 0, maxTrapHaveCount);
        GameSceneController.Instance.GameSceneUICanvas.TrapHaveUI.UpdateIcon(trapHaveCount);
    }

    /// <summary>
    /// 罠アイテムを取得できるか
    /// </summary>
    /// <returns></returns>
    public bool IsCanPickTrap()
    {
        return trapHaveCount < maxTrapHaveCount;
    }

    /// <summary>
    /// 移動できる状態か
    /// </summary>
    /// <returns></returns>
    public bool IsCanInputtedMove()
    {
        if (GameSceneController.Instance == null) return false;

        return !knockBacking && !IsDead && GameSceneController.Instance.IsCanPlayerInput();
    }

    /// <summary>
    /// ノックバックベクトルをセットする
    /// </summary>
    /// <param name="knockBackActorDirection"></param>
    /// <param name="knockBackPower"></param>
    /// <param name="bindTime"></param>
    public void SetVelocityForKnockBack(ActorDirection knockBackActorDirection, float knockBackPower, float bindTime = 1.0f)
    {
        Vector2 knockBackVec = knockBackDirection * knockBackPower;
        if (knockBackActorDirection == ActorDirection.Left)
        {
            knockBackVec.x = -knockBackVec.x;
        }

        currentKnockBackVec = knockBackVec;
        MyRigidBody.velocity = currentKnockBackVec;

        knockBacking = true;
        knockBackTimer.activateTime = bindTime;
        knockBackTimer.ResetTimer();
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damageValue"></param>
    /// <param name="knockBackDirection"></param>
    /// <param name="knockBackPower"></param>
    /// <param name="bindTime"></param>
    public void TakeDamage(int damageValue = 1, ActorDirection knockBackDirection = ActorDirection.Left, float knockBackPower = 0, float bindTime = 0.8f)
    {
        if (!enableDamage) return;

        if (knockBackPower > 0)
        {
            SetVelocityForKnockBack(knockBackDirection, knockBackPower, bindTime);
        }

        myCollider.gameObject.layer = LayerTagUtil.LayerNumberDamagedPlayer;
        enableDamage = false;
        damageEnableTimer.ResetTimer();

        animator.SetBool("Damage", true);

        if (GameSceneController.Instance.StageCleared) return;
        if (IsDead) return;

        CurrentHp = Mathf.Clamp(CurrentHp - damageValue, 0, maxPlayerHp);
        GameSceneController.Instance.GameSceneUICanvas.PlayerHpUI.UpdateGauge(CurrentHp);

        if (!IsDead && CurrentHp <= 0)
        {
            IsDead = true;
            animator.SetBool("Damage", false);
            animator.SetBool("Dead", true);
            GameSceneController.Instance.GameOverCurrentStage();
        }
    }

    /// <summary>
    /// 特定の種類の宝石の所持数を加算する
    /// </summary>
    /// <param name="jewelryType"></param>
    /// <param name="addCount"></param>
    public void AddJewelryByType(JewelryType jewelryType, int addCount = 1)
    {
        JewelryPossessionStatus.AddJewelryCountByType(jewelryType, addCount);
        if (addCount > 0)
        {
            GameSceneController.Instance.GameSceneUICanvas.JewelrySelectUI.pickUp();
        }
        else if (addCount < 0)
        {
            GameSceneController.Instance.GameSceneUICanvas.JewelrySelectUI.discard();
        }

        GameSceneController.Instance.GameSceneUICanvas.ScoreUI.WriteScore(JewelryPossessionStatus);
        CurrentMobility = playerMasterData.mobilityTable.SelectMobilityByJewelryCount(JewelryPossessionStatus.GetTotalJewelryCount());

        inputMover.moveSpeed = CurrentMobility.moveSpeed;
        inputMover.jumpHeight = CurrentMobility.jumpHeight;
        bodyTrans.localScale = Vector3.one * CurrentMobility.scale;
    }

    /// <summary>
    /// プレイヤーの入力を無効にする
    /// </summary>
    public void DisablePlayerInput()
    {
        inputMover.InputAction.Disable();
    }

    /// <summary>
    /// プレイヤーの入力を有効にする
    /// </summary>
    public void EnablePlayerInput()
    {
        inputMover.InputAction.Enable();
    }
}

public enum ActorDirection
{
    Right,
    Left
}