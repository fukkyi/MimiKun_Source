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
    /// ��莞�ԓ����Ȃ���Ԃɂ���
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
    /// �����Ȃ���Ԃ���������
    /// </summary>
    public void ReleaseBinding()
    {
        Binding = false;
        navAgent.enabled = true;

        animator.SetBool("Attack", false);
    }

    /// <summary>
    /// ��΂�S�Đ��ʂɗ��Ƃ�
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
    /// ����̎�ނ̕�΂𐳖ʂɈ���Ƃ�
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
    /// �����Ȃ���Ԃ��v������^�C�}�[������������
    /// </summary>
    private void InitBindTimer()
    {
        bindTimer.action = () => { ReleaseBinding(); };
        bindTimer.repeatAction = false;
    }

    /// <summary>
    /// �ǐՑΏۂ��X�V����
    /// </summary>
    private void UpdateNavigationTarget()
    {
        if (Binding) return;
        // �^�[�Q�b�g���Ă����΂��Ȃ��ꍇ�͋߂��̕�΂𒲂ׂ�
        if (navTargetJewelry == null)
        {
            Jewelry nearestJewelry = GameSceneController.Instance.FetchNearestDropedJewelryByPos(transform.position);
            // ���B�ł���ꏊ�ɂ����΂̂݃^�[�Q�b�g����
            if (nearestJewelry != null && navAgent.CanNavigateToPos(nearestJewelry.transform.position))
            {
                navTargetJewelry = nearestJewelry;
            }
        }
        // �^�[�Q�b�g���Ă����΂��Ȃ���Ԃŋ߂��ɂ���΂��Ȃ��ꍇ�̓v���C���[��ǂ�
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
    /// �E�҂̑����Ă��鉹���Đ����邩��~���邩�X�V����
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
    /// �v���C���[���^�[�Q�b�g�ɐݒ肷��
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
            // �v���C���[���󒆂ɂ���ꍇ�͂�����x�����n�_�\�����A�o�H��ݒ肷��B
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
            // �X�e�[�W�ɓ������Ă��Ȃ��ꍇ
            if (hitStageCount == 0) return;

            Vector3 predictPlayerLandingPos = predictPlayerRaycastBuffer[0].point;
            navAgent.SetNavTarget(predictPlayerLandingPos);
        }
    }

    /// <summary>
    /// �A�C�e�����擾�͈͂ɂ��邩�`�F�b�N����
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
    /// �v���C���[�ƏՓ˂������̏���
    /// </summary>
    /// <param name="collision"></param>
    private void CollisionToPlayer(Collision2D collision)
    {
        if (Binding) return;

        Player collisionPlayer = collision.gameObject.GetComponentInParent<Player>();

        if (collisionPlayer == null) return;

        // �m�b�N�o�b�N����������̓v���C���[�̑��΍��W�ɂ���Č��߂�
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
    /// �X���C���ƏՓ˂������̏���
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
        // �v���C���[�ƏՓ˂����ꍇ�̓_���[�W��^����
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
