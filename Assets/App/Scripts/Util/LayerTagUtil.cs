using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerTagUtil
{
    public static readonly string TagNamePlayer = "Player";
    public static readonly string TagNameJewelry = "Jewelry";
    public static readonly string TagNameHero = "Hero";

    public static readonly string LayerNamePlayer = "Player";
    public static readonly string LayerNameEnemy = "Enemy";
    public static readonly string LayerNameStage = "Stage";
    public static readonly string LayerNameItem = "Item";
    public static readonly string LayerNameItemGetZone = "ItemGetZone";
    public static readonly string LayerNameCameraBounding = "CameraBounding";
    public static readonly string LayerNameHero = "Hero";
    public static readonly string LayerNameDamagedPlayer = "DamagedPlayer";
    public static readonly string LayerNameTriggerBounding = "TriggerBounding";
    public static readonly string LayerNamePlayerOnly = "PlayerOnly";
    public static readonly string LayerNameDeadEnemyOnly = "DeadEnemy";

    public static readonly int LayerNumberPlayer = 6;
    public static readonly int LayerNumberEnemy = 7;
    public static readonly int LayerNumberStage = 8;
    public static readonly int LayerNumberItem = 9;
    public static readonly int LayerNumberItemGetZone = 10;
    public static readonly int LayerNumberCameraBounding = 11;
    public static readonly int LayerNumberHero = 12;
    public static readonly int LayerNumberDamagedPlayer = 13;
    public static readonly int LayerNumberTriggerBounding = 14;
    public static readonly int LayerNumberPlayerOnly = 15;
    public static readonly int LayerNumberDeadEnemy = 16;

    private static readonly string[] CharacterLayers = { LayerNamePlayer, LayerNameEnemy, LayerNameItem, LayerNameItemGetZone, LayerNameCameraBounding, LayerNameHero, LayerNameDamagedPlayer, LayerNameTriggerBounding };
    private static readonly string[] PlayerLayers = { LayerNamePlayer, LayerNameDamagedPlayer};
    private static readonly string[] ItemLayers = { LayerNameItem };

    /// <summary>
    /// プレイヤーとエネミーとアイテムを除いたレイヤーマスクを取得する
    /// </summary>
    /// <returns></returns>
    public static int GetLayerMaskIgnoreCharacter()
    {
        return ~LayerMask.GetMask(CharacterLayers);
    }

    /// <summary>
    /// アイテムのレイヤーマスクを取得する
    /// </summary>
    /// <returns></returns>
    public static int GetLayerMaskItem()
    {
        return LayerMask.GetMask(ItemLayers);
    }

    /// <summary>
    /// プレイヤーのレイヤーマスクを取得する
    /// </summary>
    /// <returns></returns>
    public static int GetLayerMaskPlayer()
    {
        return LayerMask.GetMask(PlayerLayers);
    }

    /// <summary>
    /// プレイヤー関連のレイヤーかどうか比較する
    /// </summary>
    /// <returns></returns>
    public static bool CompareLayerForPlayer(int layer)
    {
        return layer == LayerNumberPlayer || layer == LayerNumberDamagedPlayer;
    }
}
