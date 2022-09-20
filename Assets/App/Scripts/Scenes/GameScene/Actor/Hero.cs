using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridPlatformerNavAgent))]
public class Hero : Actor
{
    public JewelryPossessionStatus JewelryPossessionStatus { get; } = new JewelryPossessionStatus();
    public bool Binding { get; private set; }

    [SerializeField]
    private Animator animator = null;
    [SerializeField]
    private CircleCollider2D itemGetCollider = null;
    [SerializeField]
    private Transform jewelryDropPoint = null;
    [SerializeField]
    private AudioSource runSoundSource = null;
    [SerializeField]
    private MinMaxRange jewelryDropPower = new MinMaxRange(-200, 200);
    [SerializeField]
    private float attackKnockBackPower = 50.0f;
    [SerializeField]
    private float predictPlayerRaycastDistance = 50.0f;

    private Rigidbody2D myRigidbody = null;
    private GridPlatformerNavAgent navAgent = null;
    private Jewelry navTargetJewelry = null;
    private Collider2D[] overlapItemBuffer = new Collider2D[10];
    private RaycastHit2D[] predictPlayerRaycastBuffer = new RaycastHit2D[1];
    private ActionTimer bindTimer = new ActionTimer();

    private void Awake()
    {
        navAgent = GetComponent<GridPlatformerNavAgent>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitBindTimer();
    }

    // Update is called once per frame
    void Update()
    {
        bindTimer.UpdateTimer();
        UpdateNavigationTarget();
        CheckItemGetCollider();
        UpdateRunSound();
    }

    /// <summary>
    /// 一定時間動けない状態にする
    /// </summary>
    /// <param name="bindTime"></param>
    public void SetBinding(float bindTime)
    {
        bindTimer.activateTime = bindTime;
        bindTimer.ResetTimer();

        Vector3 bindVec = myRigidbody.velocity;
        bindVec.x = 0;
        myRigidbody.velocity = bindVec;

        Binding = true;
        navAgent.enabled = false;
    }

    /// <summary>
    /// 動けない状態を解除する
    /// </summary>
    public void ReleaseBinding()
    {
        Binding = false;
        navAgent.enabled = true;

        animator.SetBool("Attack", false);
    }

    /// <summary>
    /// 宝石を全て正面に落とす
    /// </summary>
    public void DropAllJewelry()
    {
        while(true)
        {
            JewelryType? selectedJewelryType = JewelryPossessionStatus.SelectRandomHaveJewelryType();
            if (selectedJewelryType == null) break;

            DropJewelryOcneByType((JewelryType)selectedJewelryType);
        }
    }

    /// <summary>
    /// 特定の種類の宝石を正面に一つ落とす
    /// </summary>
    public void DropJewelryOcneByType(JewelryType dropJewelryType)
    {
        if (JewelryPossessionStatus.GetJewelryCountByType(dropJewelryType) <= 0) return;

        Jewelry dropJewelry = Instantiate(GameSceneController.Instance.JewelryData.GetJewelryObjByType(dropJewelryType), jewelryDropPoint.position, Quaternion.identity);

        Vector2 dropVector = Vector2.left;
        if (CurrentDirection == ActorDirection.Right)
        {
            dropVector.x = -dropVector.x;
        }

        float dropPower = jewelryDropPower.RandOfRange();
        dropJewelry.DropToDirection(dropVector, dropPower);

        JewelryPossessionStatus.AddJewelryCountByType(dropJewelryType, -1);
    }

    /// <summary>
    /// 動けない状態を計測するタイマーを初期化する
    /// </summary>
    private void InitBindTimer()
    {
        bindTimer.action = () => { ReleaseBinding(); };
        bindTimer.repeatAction = false;
    }

    /// <summary>
    /// 追跡対象を更新する
    /// </summary>
    private void UpdateNavigationTarget()
    {
        if (Binding) return;
        // ターゲットしている宝石がない場合は近くの宝石を調べる
        if (navTargetJewelry == null)
        {
            Jewelry nearestJewelry = GameSceneController.Instance.FetchNearestDropedJewelryByPos(transform.position);
            // 到達できる場所にある宝石のみターゲットする
            if (nearestJewelry != null && navAgent.CanNavigateToPos(nearestJewelry.transform.position))
            {
                navTargetJewelry = nearestJewelry;
            }
        }
        // ターゲットしている宝石がない状態で近くにも宝石がない場合はプレイヤーを追う
        if (navTargetJewelry == null)
        {
            Player player = GameSceneController.Instance.Player;
            if (player != null)
            {
                SetNavigationToPlayer(player);
            }
        }
        else
        {
            navAgent.SetNavTarget(navTargetJewelry.transform.position);
        }
    }
    
    /// <summary>
    /// 勇者の走っている音を再生するか停止するか更新する
    /// </summary>
    private void UpdateRunSound()
    {
        bool playSound = IsGround() && navAgent.enabled;
        if (!playSound && runSoundSource.isPlaying)
        {
            runSoundSource.Stop();
        }
        else if (playSound && !runSoundSource.isPlaying)
        {
            runSoundSource.Play();
        }
    }

    /// <summary>
    /// プレイヤーをターゲットに設定する
    /// </summary>
    /// <param name="player"></param>
    private void SetNavigationToPlayer(Player player)
    {
        Vector3 playerPos = player.transform.position;
        if (player.IsGround())
        {
            navAgent.SetNavTarget(playerPos);
        }
        else
        {
            // プレイヤーが空中にいる場合はある程度落下地点予測し、経路を設定する。
            Vector3 playerMoveDirection = player.MyRigidBody.velocity.normalized;
            if (playerMoveDirection.y >= 0)
            {
                playerMoveDirection = Vector3.down;
            }

            int hitStageCount = Physics2D.RaycastNonAlloc(
                playerPos,
                playerMoveDirection,
                predictPlayerRaycastBuffer,
                predictPlayerRaycastDistance,
                LayerTagUtil.GetLayerMaskIgnoreCharacter()
            );
            // ステージに当たっていない場合
            if (hitStageCount == 0) return;

            Vector3 predictPlayerLandingPos = predictPlayerRaycastBuffer[0].point;
            navAgent.SetNavTarget(predictPlayerLandingPos);
        }
    }

    /// <summary>
    /// アイテムが取得範囲にあるかチェックする
    /// </summary>
    private void CheckItemGetCollider()
    {
        if (Binding) return;
        if (itemGetCollider == null) return;

        int itemOverlapedCount = Physics2D.OverlapCircleNonAlloc(transform.position, itemGetCollider.radius, overlapItemBuffer, LayerTagUtil.GetLayerMaskItem());

        if (itemOverlapedCount == 0) return;

        for (int i = 0; i < itemOverlapedCount; i++)
        {
            Item overlapedItem = overlapItemBuffer[i].GetComponentInParent<Item>();
            overlapedItem.ReceivedByHero(this);
        }
    }

    /// <summary>
    /// プレイヤーと衝突した時の処理
    /// </summary>
    /// <param name="collision"></param>
    private void CollisionToPlayer(Collision2D collision)
    {
        if (Binding) return;

        Player collisionPlayer = collision.gameObject.GetComponentInParent<Player>();

        if (collisionPlayer == null) return;

        // ノックバックさせる方向はプレイヤーの相対座標によって決める
        ActorDirection knockBackDirection = ActorDirection.Left;
        if (transform.position.x < collisionPlayer.transform.position.x)
        {
            knockBackDirection = ActorDirection.Right;
        }

        collisionPlayer.TakeDamage(
            knockBackDirection: knockBackDirection,
            knockBackPower: attackKnockBackPower
        );

        animator.SetBool("Attack", true);
        SetBinding(0.5f);

        AudioManager.Instance.PlaySE("HeroAttack");
    }

    /// <summary>
    /// スライムと衝突した時の処理
    /// </summary>
    /// <param name="collision"></param>
    private void CollisionToSlime(Collision2D collision)
    {
        if (Binding) return;

        Slime collisionSlime = collision.gameObject.GetComponentInParent<Slime>();

        if (collisionSlime == null) return;

        collisionSlime.Explosion();

        animator.SetBool("Attack", true);
        SetBinding(0.5f);

        AudioManager.Instance.PlaySE("HeroAttack");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // プレイヤーと衝突した場合はダメージを与える
        if (collision.gameObject.layer == LayerTagUtil.LayerNumberPlayer)
        {
            CollisionToPlayer(collision);
        }
        else if (collision.gameObject.layer == LayerTagUtil.LayerNumberEnemy)
        {
            CollisionToSlime(collision);
        }
    }
}
