using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Item
{
    [SerializeField]
    protected GameObject sleepObj = null;
    [SerializeField]
    protected GameObject activateObj = null;
    [SerializeField]
    protected Sprite baitSprite = null;
    [SerializeField]
    protected float bindTime = 3.0f;

    protected Rigidbody2D myRigidbody = null;
    protected ActionTimer bindTimer = new ActionTimer();
    protected bool activated = false;
    protected bool binded = false;

    protected virtual void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        bindTimer.action = () => { Destroy(gameObject); };
    }

    protected virtual void Update()
    {
        if (binded)
        {
            bindTimer.UpdateTimer();
        }
    }

    public override void ReceivedByPlayer(Player player)
    {
        if (activated) return;
        if (!player.IsCanPickTrap()) return;

        player.AddTrap();
        Destroy(gameObject);
    }

    public override void ReceivedByHero(Hero hero)
    {
        if (!activated) return;

        hero.SetBinding(bindTime);
        hero.DropAllJewelry();

        bindTimer.activateTime = bindTime;
        bindTimer.ResetTimer();

        activateObj.GetComponent<SpriteRenderer>().sprite = baitSprite;
        transform.position = hero.transform.position;

        binded = true;
    }

    /// <summary>
    /// ����̕����Ƌ�����㩂��h���b�v������
    /// </summary>
    /// <param name="dropDirection"></param>
    /// <param name="dropPower"></param>
    public void DropToDirection(Vector2 dropDirection, float dropPower)
    {
        myRigidbody.velocity = dropDirection.normalized * dropPower;
    }

    /// <summary>
    /// 㩂�L�������A�h���b�v������
    /// </summary>
    /// <param name="dropDirection"></param>
    /// <param name="dropPower"></param>
    public void DropAndActivateToDirection(Vector2 dropDirection, float dropPower)
    {
        activated = true;

        sleepObj.SetActive(false);
        activateObj.SetActive(true);

        DropToDirection(dropDirection, dropPower);
    }
}
