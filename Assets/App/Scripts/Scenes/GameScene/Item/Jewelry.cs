using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Jewelry : Item
{
    public bool IsEnabledReceive { get; protected set; } = true;
    public bool IsDroped { get; protected set; } = false;

    [SerializeField]
    protected JewelryType jewelryType = JewelryType.Emerald;
    [SerializeField]
    protected float receiveEnabledTime = 1.0f;

    protected ActionTimer dropEnabledTimer = new ActionTimer();
    protected Rigidbody2D myRigidbody = null;

    protected virtual void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        SetupDropEnabledTimer();
    }

    protected virtual void Update()
    {
        dropEnabledTimer.UpdateTimer();
    }

    /// <summary>
    /// �h���b�v�\�^�C�}�[���Z�b�g�A�b�v����
    /// </summary>
    protected void SetupDropEnabledTimer()
    {
        dropEnabledTimer.action = () => { IsEnabledReceive = true; };
        dropEnabledTimer.activateTime = receiveEnabledTime;
    }

    public override void ReceivedByPlayer(Player player)
    {
        if (!IsEnabledReceive) return;

        player.AddJewelryByType(jewelryType);
        AudioManager.Instance.PlaySE("GetJewelry", volume: 0.3f);

        Destroy(gameObject);
    }

    public override void ReceivedByHero(Hero hero)
    {
        if (!IsEnabledReceive) return;
        if (hero.Binding) return;

        hero.JewelryPossessionStatus.AddJewelryCountByType(jewelryType);
        hero.SetBinding(GameSceneController.Instance.JewelryData.GetBindTimeByType(jewelryType));

        Destroy(gameObject);
    }

    /// <summary>
    /// ����̕����Ƌ����ŕ�΂��h���b�v������
    /// </summary>
    /// <param name="dropDirection"></param>
    /// <param name="dropPower"></param>
    public void DropToDirection(Vector2 dropDirection, float dropPower)
    {
        myRigidbody.velocity = dropDirection.normalized * dropPower;

        IsEnabledReceive = false;
        IsDroped = true;
        dropEnabledTimer.ResetTimer();
    }
}
