using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse.Common.Hooks;

public class PlayerHooks : ModHook {
    public const int inventorySize = 58;
    public const int voidBagSize = 40;
    public const int zombieInventorySize = 5;
    public const int defaultMaxAccSlots = 8;
    public static readonly MethodInfo isZombie = typeof(PlayerExtensions).GetMethod("IsZombie", BindingFlags.Public | BindingFlags.Static);
    public static readonly FieldInfo netMode = typeof(Main).GetField("netMode", BindingFlags.Public | BindingFlags.Static);
    public static readonly FieldInfo active = typeof(Player).GetField("active", BindingFlags.Public | BindingFlags.Instance);
    public static readonly FieldInfo hostile = typeof(Player).GetField("hostile", BindingFlags.Public | BindingFlags.Instance);

    // config
    public static readonly MethodInfo getConfig = typeof(ZombieApocalypseConfig).GetMethod("GetInstance", BindingFlags.Public | BindingFlags.Static, []);
    public static readonly MethodInfo ZombiesHaveSmallerInventories = typeof(ZombieApocalypseConfig).GetMethod("get_ZombiesHaveSmallerInventories", BindingFlags.Public | BindingFlags.Instance);
    public static readonly MethodInfo ZombiesCanUseAmmoAndCoinSlots = typeof(ZombieApocalypseConfig).GetMethod("get_ZombiesCanUseAmmoAndCoinSlots", BindingFlags.Public | BindingFlags.Instance);
    public static readonly MethodInfo ZombiesCanFightOtherPlayersWithoutPvP = typeof(ZombieApocalypseConfig).GetMethod("get_ZombiesCanFightOtherPlayersWithoutPvP", BindingFlags.Public | BindingFlags.Instance);

    private static ILHook autoSelectHook;

    public override void Apply() {
        IL_Player.ChooseAmmo += ChooseAmmo;
        IL_Player.ConsumeItem += ConsumeItem;
        IL_Player.CountItem += QuickMana_GetItemToUse;
        IL_Player.DoCoins += DoCoins;
        //IL_Player.DropAnglerAccByMissing += ... // probably fine to not il edit
        // ^ yeah vanilla still counts unusable accessories in that function soooo
        IL_Player.FillAmmo += FillAmmo;
        IL_Player.FindItem_int += QuickMana_GetItemToUse;
        IL_Player.FindItem_List1 += QuickMana_GetItemToUse;
        IL_Player.FindItem_BooleanArray += QuickMana_GetItemToUse;
        IL_Player.FindPaintOrCoating += FindPaintOrCoating;
        IL_Player.Fishing_GetBait += Fishing_GetBait;
        IL_Player.Fishing_GetBestFishingPole += QuickMana_GetItemToUse;
        IL_Player.GetBestPickaxe += GetBestPickaxe;
        IL_Player.HasItem_int += QuickMana_GetItemToUse;
        IL_Player.ItemCheck_Inner += ItemCheck_Inner;
        IL_Player.ItemCheck_MeleeHitPVP += ItemCheck_MeleeHitPVP;
        IL_Player.ItemSpace += ItemSpace;
        IL_Player.PutItemInInventoryFromItemUsage += QuickMana_GetItemToUse;
        IL_Player.QuickHeal_GetItemToUse += QuickHeal_GetItemToUse;
        IL_Player.QuickMana_GetItemToUse += QuickMana_GetItemToUse;
        IL_Player.QuickBuff += QuickHeal_GetItemToUse;
        IL_Player.QuickBuff_PickBestFoodItem += QuickMana_GetItemToUse;
        IL_Player.QuickGrapple_GetItemToUse += QuickMana_GetItemToUse;
        IL_Player.QuickMount_GetItemToUse += QuickMana_GetItemToUse;
        IL_Player.QuickStackAllChests += QuickStackAllChests;
        IL_Player.RefreshInfoAccs += RefreshInfoAccs;
        IL_Player.ScrollHotbar += ScrollHotbar;
        IL_Player.SmartSelect_GetAvailableToolRanges += GetBestPickaxe;
        IL_Player.SmartSelect_PickToolForStrategy += GetBestPickaxe;
        autoSelectHook = new(typeof(TileLoader).GetMethod("AutoSelect", BindingFlags.Public | BindingFlags.Static), AutoSelect);
        autoSelectHook.Apply();
        IL_Player.UpdateBuffs += UpdateBuffs;
        IL_Player.UpdateEquips += UpdateEquips;
        IL_Player.useVoidBag += QuickMana_GetItemToUse;

        On_Player.GetAmountOfExtraAccessorySlotsToShow += OnGetAmountOfExtraAccessorySlotsToShow;
        On_Player.GetItem_FillEmptyInventorySlot += OnGetItem_FillEmptyInventorySlot;
        On_Player.GetItem_FillIntoOccupiedSlot += OnGetItem_FillIntoOccupiedSlot;
        On_Player.InOpposingTeam += OnInOpposingTeam;
        On_Player.IsItemSlotUnlockedAndUsable += OnIsItemSlotUnlockedAndUsable;
        On_Player.Spawn_SetPositionAtWorldSpawn += OnSpawn_SetPositionAtWorldSpawn;
        On_Player.Update += OnUpdate;
        On_Player.useVoidBag += OnUseVoidbag;
    }

    public override void Unapply() {
        IL_Player.ChooseAmmo -= ChooseAmmo;
        IL_Player.ConsumeItem -= ConsumeItem;
        IL_Player.CountItem -= QuickMana_GetItemToUse;
        IL_Player.DoCoins -= DoCoins;
        IL_Player.FillAmmo -= FillAmmo;
        IL_Player.FindItem_int -= QuickMana_GetItemToUse;
        IL_Player.FindItem_List1 -= QuickMana_GetItemToUse;
        IL_Player.FindItem_BooleanArray -= QuickMana_GetItemToUse;
        IL_Player.FindPaintOrCoating -= FindPaintOrCoating;
        IL_Player.Fishing_GetBait -= Fishing_GetBait;
        IL_Player.Fishing_GetBestFishingPole -= QuickMana_GetItemToUse;
        IL_Player.GetBestPickaxe -= GetBestPickaxe;
        IL_Player.HasItem_int -= QuickMana_GetItemToUse;
        IL_Player.ItemCheck_Inner -= ItemCheck_Inner;
        IL_Player.ItemCheck_MeleeHitPVP -= ItemCheck_MeleeHitPVP;
        IL_Player.ItemSpace -= ItemSpace;
        IL_Player.PutItemInInventoryFromItemUsage -= QuickMana_GetItemToUse;
        IL_Player.QuickHeal_GetItemToUse -= QuickHeal_GetItemToUse;
        IL_Player.QuickMana_GetItemToUse -= QuickMana_GetItemToUse;
        IL_Player.QuickBuff -= QuickHeal_GetItemToUse;
        IL_Player.QuickBuff_PickBestFoodItem -= QuickMana_GetItemToUse;
        IL_Player.QuickGrapple_GetItemToUse -= QuickMana_GetItemToUse;
        IL_Player.QuickMount_GetItemToUse -= QuickMana_GetItemToUse;
        IL_Player.QuickStackAllChests -= QuickStackAllChests;
        IL_Player.ScrollHotbar -= ScrollHotbar;
        IL_Player.SmartSelect_GetAvailableToolRanges -= GetBestPickaxe;
        IL_Player.SmartSelect_PickToolForStrategy -= GetBestPickaxe;
        autoSelectHook.Undo();
        IL_Player.UpdateEquips -= UpdateEquips;
        IL_Player.useVoidBag -= QuickMana_GetItemToUse;

        On_Player.GetAmountOfExtraAccessorySlotsToShow -= OnGetAmountOfExtraAccessorySlotsToShow;
        On_Player.GetItem_FillEmptyInventorySlot -= OnGetItem_FillEmptyInventorySlot;
        On_Player.GetItem_FillIntoOccupiedSlot -= OnGetItem_FillIntoOccupiedSlot;
        On_Player.InOpposingTeam -= OnInOpposingTeam;
        On_Player.IsItemSlotUnlockedAndUsable -= OnIsItemSlotUnlockedAndUsable;
        On_Player.Spawn_SetPositionAtWorldSpawn -= OnSpawn_SetPositionAtWorldSpawn;
        On_Player.Update -= OnUpdate;
        On_Player.useVoidBag -= OnUseVoidbag;
    }

    // magic numbers go brrr
    private int OnGetAmountOfExtraAccessorySlotsToShow(On_Player.orig_GetAmountOfExtraAccessorySlotsToShow orig, Player self) {
        if (self.IsZombie() && ZombieApocalypseConfig.GetInstance().ZombiesHaveLessAccessories) {
            int addedSlots = !(!Main.expertMode && !Main.gameMenu) && self.extraAccessory ?
                7 - defaultMaxAccSlots : 6 - defaultMaxAccSlots;
            if (!Main.masterMode && !Main.gameMenu)
                addedSlots--;
            return addedSlots;
        }
        return orig(self);
    }

    private bool OnIsItemSlotUnlockedAndUsable(On_Player.orig_IsItemSlotUnlockedAndUsable orig, Player self, int slot) {
        if (self.IsZombie() && ZombieApocalypseConfig.GetInstance().ZombiesHaveLessAccessories) {
            int addedSlots = !(!Main.expertMode && !Main.gameMenu) && self.extraAccessory ? 5 : 4;
            if (!Main.masterMode && !Main.gameMenu)
                addedSlots--;
            return slot < 10 && slot < addedSlots + 2 || slot >= 10 && slot < addedSlots + 14;
        }
        return orig(self, slot);
    }

    private bool OnInOpposingTeam(On_Player.orig_InOpposingTeam orig, Player self, Player otherPlayer) =>
        orig(self, otherPlayer) || (self.IsZombie() && !otherPlayer.IsZombie() || otherPlayer.IsZombie() && !self.IsZombie()) && ZombieApocalypseConfig.GetInstance().ZombiesCanFightOtherPlayersWithoutPvP;

    private bool OnGetItem_FillEmptyInventorySlot(On_Player.orig_GetItem_FillEmptyInventorySlot orig, Player self, int plr, Item newItem, GetItemSettings settings, Item returnItem, int i) =>
        (!self.IsZombie() || i < zombieInventorySize || !ZombieApocalypseConfig.GetInstance().ZombiesHaveSmallerInventories) && orig(self, plr, newItem, settings, returnItem, i);

    private bool OnGetItem_FillIntoOccupiedSlot(On_Player.orig_GetItem_FillIntoOccupiedSlot orig, Player self, int plr, Item newItem, GetItemSettings settings, Item returnItem, int i) =>
        (!self.IsZombie() || i < zombieInventorySize || !ZombieApocalypseConfig.GetInstance().ZombiesHaveSmallerInventories) && orig(self, plr, newItem, settings, returnItem, i);

    private static void Spawn_SetPosition(Player player, int floorX, int floorY) { // copy of Spawn_SetPosition from Player class
        player.position.X = floorX * 16 + 8 - player.width / 2;
        player.position.Y = floorY * 16 - player.height;
    }

    private static bool Spawn_IsAreaAValidWorldSpawn(int floorX, int floorY) {
        for (int i = floorX - 1; i < floorX + 2; i++)
            for (int j = floorY - 3; j < floorY; j++)
                if (i >= 0 && i <= Main.maxTilesX && j >= 0 && j <= Main.maxTilesY && (Main.tile[i, j].HasUnactuatedTile && Main.tileSolid[Main.tile[i, j].TileType] && !Main.tileSolidTop[Main.tile[i, j].TileType] || Main.tile[i, j].LiquidAmount > 0))
                    return false;
        return true;
    }

    private static void Spawn_ForceClearArea(int floorX, int floorY) {
        for (int i = floorX - 1; i < floorX + 2; i++) {
            for (int j = floorY - 3; j < floorY; j++) {
                if (Main.tile[i, j] != null) {
                    if (Main.tile[i, j].HasUnactuatedTile && Main.tileSolid[Main.tile[i, j].TileType] && !Main.tileSolidTop[Main.tile[i, j].TileType])
                        WorldGen.KillTile(i, j);

                    if (Main.tile[i, j].LiquidAmount > 0) {
                        Tile tile = Main.tile[i, j];
                        tile.LiquidType = 0;
                        tile.LiquidAmount = 0;
                        WorldGen.SquareTileFrame(i, j);
                    }
                }
            }
        }
    }

    private static bool Spawn_GetPositionAtWorldSpawn(ref int floorX, ref int floorY) {
        int spawnTileX = floorX;
        int num = floorY;
        if (!Spawn_IsAreaAValidWorldSpawn(spawnTileX, num)) {
            bool flag = false;
            if (!flag) {
                for (int i = 0; i < 30; i++) {
                    if (Spawn_IsAreaAValidWorldSpawn(spawnTileX, num - i)) {
                        num -= i;
                        flag = true;
                        break;
                    }
                }
            }

            if (!flag) {
                for (int j = 0; j < 30; j++) {
                    if (Spawn_IsAreaAValidWorldSpawn(spawnTileX, num - j)) {
                        num -= j;
                        flag = true;
                        break;
                    }
                }
            }

            if (flag) {
                floorX = spawnTileX;
                floorY = num;
                return true;
            }

            return false;
        }

        num = Spawn_DescendFromDefaultSpace(spawnTileX, num);
        floorX = spawnTileX;
        floorY = num;
        return false;
    }
    
    private static int Spawn_DescendFromDefaultSpace(int x, int y) {
        for (int i = y; i < Main.maxTilesY; i++) {
            for (int j = -1; j <= 1; j++) {
                Tile tile = Main.tile[x + j, i];
                if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                    return i;
            }
        }

        return y;
    }

    private static void DoSpawnThingy(Player player, float x, float y) {
        int floorX = (int)x;
        int floorY = (int)y;
        bool bl = Spawn_GetPositionAtWorldSpawn(ref floorX, ref floorY);
        Spawn_SetPosition(player, floorX, floorY);
        if (bl && !Spawn_IsAreaAValidWorldSpawn(floorX, floorY))
            Spawn_ForceClearArea(floorX, floorY);
    }

    // this hook is so scuffed
    // but it seems to work as intended so
    private void OnSpawn_SetPositionAtWorldSpawn(On_Player.orig_Spawn_SetPositionAtWorldSpawn orig, Player self) {
        if ((ZombieApocalypseConfig.GetInstance(out var cfg).AlwaysRespawnZombiesWhereTheyDied && self.IsZombie() && !cfg.UnzombifyPlayersOnDeath) || ((cfg.RespawnNewZombiesWhereTheyDied || cfg.AlwaysRespawnZombiesWhereTheyDied) && !self.IsZombie() && self.IsZombifiableDeath()))
            DoSpawnThingy(self, self.position.X / 16, self.position.Y / 16);
        else if (cfg.ZombifiedPlayersSpawnAtOceans && (self.IsZombifiableDeath() || self.IsZombie()))
            DoSpawnThingy(self, Main.rand.NextBool() ? Main.maxTilesX - 300 : 300, 0);
        else
            orig(self);
        if (cfg.ZombifyPlayersOnRespawn && !self.IsZombie() && self.IsZombifiableDeath()) {
            self.statLife = self.statLifeMax / 2;
            self.SetZombie(true);
            self.GetModPlayer<ZombifiablePlayer>().LastDeathReason = null;
        }
    }

    private void OnUpdate(On_Player.orig_Update orig, Player self, int i) {
        if (self.IsZombie() && ZombieApocalypseConfig.GetInstance().ZombiesHaveSmallerInventories) { // hacky solution right 'ere
            PlayerInput.Triggers.Current.Hotbar6 = false;
            PlayerInput.Triggers.Current.Hotbar7 = false;
            PlayerInput.Triggers.Current.Hotbar8 = false;
            PlayerInput.Triggers.Current.Hotbar9 = false;
            PlayerInput.Triggers.Current.Hotbar10 = false;
        }
        orig(self, i);
    }

    private bool OnUseVoidbag(On_Player.orig_useVoidBag orig, Player self) =>
        (!self.IsZombie() || ZombieApocalypseConfig.GetInstance().AllowZombiesToUseVoidbags) && orig(self);

    private void FillAmmo(ILContext il) { // oops
        try { // is the same as FindPaintOrCoating il hook
            // whatevr
            if (!ZombieApocalypseConfig.GetInstance(out var cfg).ZombiesCanUseAmmoAndCoinSlots || !cfg.ZombiesHaveSmallerInventories) {
                ILCursor c = new(il);
                for (int _ = 0; _ < 2; _++)
                    ReplaceDefaultInventorySize(c, il);
            }
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void QuickHeal_GetItemToUse(ILContext il) { // needs voidbag support
        try {
            ILCursor c = new(il);
            c.GotoNext(i => i.MatchLdcI4(0),
                i => i.MatchStloc(4));
            c.GotoNext(i => i.MatchLdcI4(0));
            ILLabel vanilla = il.DefineLabel();
            c.Emit(OpCodes.Call, getConfig);
            c.Emit(OpCodes.Call, ZombiesHaveSmallerInventories);
            c.Emit(OpCodes.Brfalse_S, vanilla);
            c.Emit(OpCodes.Ldarg_0); // load player var
            c.Emit(OpCodes.Call, isZombie);
            c.Emit(OpCodes.Brfalse_S, vanilla);
            c.Emit(OpCodes.Ldc_I4, zombieInventorySize); // muahahaha
            c.Emit(OpCodes.Stloc_3);
            c.MarkLabel(vanilla);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    public static void ReplaceDefaultInventorySize(ILCursor c, ILContext il, int size = inventorySize, int newSize = zombieInventorySize, bool checkAmmoToo = false) {
        c.GotoNext(MoveType.After, i => i.MatchLdcI4(size));
        ILLabel vanilla = il.DefineLabel();
        if (checkAmmoToo) {
            ILLabel modded = il.DefineLabel();
            c.Emit(OpCodes.Call, getConfig);
            c.Emit(OpCodes.Call, ZombiesCanUseAmmoAndCoinSlots);
            c.Emit(OpCodes.Brfalse_S, modded);
            c.Emit(OpCodes.Call, getConfig);
            c.Emit(OpCodes.Call, ZombiesHaveSmallerInventories);
            c.Emit(OpCodes.Brtrue_S, modded);
            c.Emit(OpCodes.Br_S, vanilla);
            c.MarkLabel(modded);
        } else {
            c.Emit(OpCodes.Call, getConfig);
            c.Emit(OpCodes.Call, ZombiesHaveSmallerInventories);
            c.Emit(OpCodes.Brfalse_S, vanilla);
        }
        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Call, isZombie);
        c.Emit(OpCodes.Brfalse_S, vanilla);
        c.Emit(OpCodes.Pop);
        c.Emit(OpCodes.Ldc_I4, newSize);
        c.MarkLabel(vanilla);
    }

    private void QuickMana_GetItemToUse(ILContext il) {
        try {
            ReplaceDefaultInventorySize(new(il), il);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void QuickStackAllChests(ILContext il) {
        try {
            ILCursor c = new(il);
            for (int _ = 0; _ < 2; _++)
                ReplaceDefaultInventorySize(c, il, 50);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void ScrollHotbar(ILContext il) {
        try {
            ILCursor c = new(il);
            ReplaceDefaultInventorySize(c, il, 9, zombieInventorySize - 1);
            for (int _ = 0; _ < 2; _++) { // do twice
                if (_ == 0) // why is it compiled like this
                    c.GotoPrev(MoveType.After, i => i.MatchLdcI4(10));
                else
                    c.GotoNext(MoveType.After, i => i.MatchLdcI4(10));
                ILLabel vanilla = il.DefineLabel();
                c.Emit(OpCodes.Call, getConfig);
                c.Emit(OpCodes.Call, ZombiesHaveSmallerInventories);
                c.Emit(OpCodes.Brfalse_S, vanilla);
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Call, isZombie);
                c.Emit(OpCodes.Brfalse_S, vanilla);
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_I4, zombieInventorySize);
                c.MarkLabel(vanilla);
            }
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void FindPaintOrCoating(ILContext il) {
        try {
            ILCursor c = new(il);
            for (int _ = 0; _ < 2; _++)
                ReplaceDefaultInventorySize(c, il);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void Fishing_GetBait(ILContext il) {
        try {
            ILCursor c = new(il);
            ReplaceDefaultInventorySize(c, il);
            ReplaceDefaultInventorySize(c, il, 50);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void GetBestPickaxe(ILContext il) {
        try {
            ReplaceDefaultInventorySize(new(il), il, 50);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void ChooseAmmo(ILContext il) {
        try {
            ILCursor c = new(il);
            ReplaceDefaultInventorySize(c, il, 50, 0, true);
            ReplaceDefaultInventorySize(c, il, 4, zombieInventorySize, true);
            ReplaceDefaultInventorySize(c, il, 58, zombieInventorySize, true);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void ConsumeItem(ILContext il) {
        try {
            ILCursor c = new(il);
            ReplaceDefaultInventorySize(c, il);
            ReplaceDefaultInventorySize(c, il, 57, 3);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void DoCoins(ILContext il) {
        try {
            ReplaceDefaultInventorySize(new(il), il, 54, zombieInventorySize, true);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void ItemCheck_Inner(ILContext il) {
        try {
            ReplaceDefaultInventorySize(new(il), il);
            ReplaceDefaultInventorySize(new(il), il, 54, zombieInventorySize, true);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void ItemSpace(ILContext il) {
        try {
            ILCursor c = new(il);
            ReplaceDefaultInventorySize(c, il, 50);
            ReplaceDefaultInventorySize(c, il, 54, zombieInventorySize, true);
            for (int _ = 0; _ < 2; _++)
                ReplaceDefaultInventorySize(c, il);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void RefreshInfoAccs(ILContext il) {
        try {
            ReplaceDefaultInventorySize(new(il), il);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void UpdateEquips(ILContext il) {
        try {
            ReplaceDefaultInventorySize(new(il), il);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void AutoSelect(ILContext il) {
        try {
            ILCursor c = new(il);
            c.GotoNext(MoveType.After, i => i.MatchLdcI4(50));
            ILLabel vanilla = il.DefineLabel();
            c.Emit(OpCodes.Call, getConfig);
            c.Emit(OpCodes.Call, ZombiesHaveSmallerInventories);
            c.Emit(OpCodes.Brfalse_S, vanilla);
            c.Emit(OpCodes.Ldarg_2); // player is passed into TileLoader.AutoSelect as the third argument
            c.Emit(OpCodes.Call, isZombie);
            c.Emit(OpCodes.Brfalse_S, vanilla);
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4, zombieInventorySize);
            c.MarkLabel(vanilla);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void ItemCheck_MeleeHitPVP(ILContext il) {
        try {
            ILCursor c = new(il);
            ILLabel t = null;
            c.GotoNext(i => i.MatchLdarg0(),
                i => i.MatchLdfld(hostile),
                i => i.MatchBrtrue(out t));
            c.Emit(OpCodes.Call, getConfig);
            c.Emit(OpCodes.Call, ZombiesCanFightOtherPlayersWithoutPvP);
            c.Emit(OpCodes.Brtrue_S, t);
            c.GotoNext(MoveType.After, i => i.MatchLdloc1(),
                i => i.MatchLdfld(active),
                i => i.MatchBrfalse(out _));
            ILLabel skipHostile = il.DefineLabel();
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Call, isZombie);
            c.Emit(OpCodes.Ldloc_1);
            c.Emit(OpCodes.Call, isZombie);
            c.Emit(OpCodes.Bne_Un_S, skipHostile);
            ILLabel skipToEnd = il.DefineLabel();
            c.GotoNext(MoveType.After, i => i.MatchLdloc1(),
                i => i.MatchLdfld(hostile),
                i => i.MatchBrfalse(out skipToEnd));
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldfld, hostile);
            c.Emit(OpCodes.Brfalse, skipToEnd);
            c.MarkLabel(skipHostile);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void UpdateBuffs(ILContext il) {
        try {
            ILCursor c = new(il);
            c.GotoNext(i => i.MatchLdarg0(),
                i => i.MatchLdfld(hostile),
                i => i.MatchBrfalse(out _));
            ILLabel t = il.DefineLabel();
            c.Emit(OpCodes.Call, getConfig);
            c.Emit(OpCodes.Call, ZombiesCanFightOtherPlayersWithoutPvP);
            c.Emit(OpCodes.Brtrue_S, t);
            c.GotoNext(i => i.MatchLdcI4(0));
            c.MarkLabel(t);
            c.GotoNext(i => i.MatchLdloc(11),
                i => i.MatchLdfld(hostile),
                i => i.MatchBrfalse(out _));
            ILLabel skipHostile = il.DefineLabel();
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Call, isZombie);
            c.Emit(OpCodes.Ldloc_S, (byte)11);
            c.Emit(OpCodes.Call, isZombie);
            c.Emit(OpCodes.Bne_Un_S, skipHostile);
            ILLabel skipToEnd = il.DefineLabel();
            c.GotoNext(MoveType.After, i => i.MatchLdloc(11),
                i => i.MatchLdfld(hostile),
                i => i.MatchBrfalse(out skipToEnd));
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldfld, hostile);
            c.Emit(OpCodes.Brfalse, skipToEnd);
            c.MarkLabel(skipHostile);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }
}
