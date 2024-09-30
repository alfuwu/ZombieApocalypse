using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
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

    public override void Apply() {
        IL_Player.ChooseAmmo += ChooseAmmo;
        IL_Player.ConsumeItem += ConsumeItem;
        IL_Player.CountItem += QuickMana_GetItemToUse;
        IL_Player.DoCoins += DoCoins;
        //IL_Player.DropAnglerAccByMissing += ... // probably fine to not il edit
        IL_Player.FillAmmo += Stop;
        IL_Player.FindItem_int += QuickMana_GetItemToUse;
        IL_Player.FindItem_List1 += QuickMana_GetItemToUse;
        IL_Player.FindItem_BooleanArray += QuickMana_GetItemToUse;
        IL_Player.FindPaintOrCoating += FindPaintOrCoating;
        IL_Player.Fishing_GetBait += Fishing_GetBait;
        IL_Player.Fishing_GetBestFishingPole += QuickMana_GetItemToUse;
        IL_Player.GetBestPickaxe += GetBestPickaxe;
        IL_Player.HasItem_int += QuickMana_GetItemToUse;
        IL_Player.ItemCheck_Inner += ItemCheck_Inner;
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
        IL_Player.useVoidBag += QuickMana_GetItemToUse;

        On_Player.GetAmountOfExtraAccessorySlotsToShow += OnGetAmountOfExtraAccessorySlotsToShow;
        On_Player.GetItem_FillEmptyInventorySlot += OnGetItem_FillEmptyInventorySlot;
        On_Player.GetItem_FillIntoOccupiedSlot += OnGetItem_FillIntoOccupiedSlot;
        On_Player.InOpposingTeam += OnInOpposingTeam;
        On_Player.IsItemSlotUnlockedAndUsable += OnIsItemSlotUnlockedAndUsable;
        On_Player.useVoidBag += OnUseVoidbag;
    }

    public override void Unapply() {
        IL_Player.ChooseAmmo -= ChooseAmmo;
        IL_Player.ConsumeItem -= ConsumeItem;
        IL_Player.CountItem -= QuickMana_GetItemToUse;
        IL_Player.DoCoins -= DoCoins;
        IL_Player.FillAmmo -= Stop;
        IL_Player.FindItem_int -= QuickMana_GetItemToUse;
        IL_Player.FindItem_List1 -= QuickMana_GetItemToUse;
        IL_Player.FindItem_BooleanArray -= QuickMana_GetItemToUse;
        IL_Player.FindPaintOrCoating -= FindPaintOrCoating;
        IL_Player.Fishing_GetBait -= Fishing_GetBait;
        IL_Player.Fishing_GetBestFishingPole -= QuickMana_GetItemToUse;
        IL_Player.GetBestPickaxe -= GetBestPickaxe;
        IL_Player.HasItem_int -= QuickMana_GetItemToUse;
        IL_Player.ItemCheck_Inner -= ItemCheck_Inner;
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
        IL_Player.UpdateEquips += UpdateEquips;
        IL_Player.useVoidBag -= QuickMana_GetItemToUse;

        On_Player.GetAmountOfExtraAccessorySlotsToShow -= OnGetAmountOfExtraAccessorySlotsToShow;
        On_Player.GetItem_FillEmptyInventorySlot -= OnGetItem_FillEmptyInventorySlot;
        On_Player.GetItem_FillIntoOccupiedSlot -= OnGetItem_FillIntoOccupiedSlot;
        On_Player.InOpposingTeam -= OnInOpposingTeam;
        On_Player.IsItemSlotUnlockedAndUsable -= OnIsItemSlotUnlockedAndUsable;
        On_Player.useVoidBag -= OnUseVoidbag;
    }

    // magic numbers go brrr
    private int OnGetAmountOfExtraAccessorySlotsToShow(On_Player.orig_GetAmountOfExtraAccessorySlotsToShow orig, Player self) {
        if (self.IsZombie()) {
            int addedSlots = !(!Main.expertMode && !Main.gameMenu) && self.extraAccessory ?
                7 - defaultMaxAccSlots : 6 - defaultMaxAccSlots;
            if (!Main.masterMode && !Main.gameMenu)
                addedSlots--;
            return addedSlots;
        }
        return orig(self);
    }

    private bool OnIsItemSlotUnlockedAndUsable(On_Player.orig_IsItemSlotUnlockedAndUsable orig, Player self, int slot) {
        if (self.IsZombie()) {
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
        (!self.IsZombie() || i < zombieInventorySize) && orig(self, plr, newItem, settings, returnItem, i);

    private bool OnGetItem_FillIntoOccupiedSlot(On_Player.orig_GetItem_FillIntoOccupiedSlot orig, Player self, int plr, Item newItem, GetItemSettings settings, Item returnItem, int i) =>
        (!self.IsZombie() || i < zombieInventorySize) && orig(self, plr, newItem, settings, returnItem, i);

    private bool OnUseVoidbag(On_Player.orig_useVoidBag orig, Player self) =>
        (!self.IsZombie() || ZombieApocalypseConfig.GetInstance().AllowZombiesToUseVoidbags) && orig(self);

    private void Stop(ILContext il) {
        try {
            ILCursor c = new(il);
            c.GotoNext(i => i.MatchStloc0());
            ILLabel vanilla = il.DefineLabel();
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Call, isZombie);
            c.Emit(OpCodes.Brfalse_S, vanilla);
            c.Emit(OpCodes.Ret);
            c.MarkLabel(vanilla);
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
            c.Emit(OpCodes.Ldarg_0); // load player var
            c.Emit(OpCodes.Call, isZombie);
            c.Emit(OpCodes.Brfalse_S, vanilla);
            c.Emit(OpCodes.Ldc_I4, zombieInventorySize); // muahahaha
            c.Emit(OpCodes.Stloc_3);
            c.MarkLabel(vanilla);
            DumpIL(il);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    public static void ReplaceDefaultInventorySize(ILCursor c, ILContext il, int size = inventorySize, int newSize = zombieInventorySize) {
        c.GotoNext(MoveType.After, i => i.MatchLdcI4(size));
        ILLabel vanilla = il.DefineLabel();
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
            c.GotoNext(i => i.MatchLdsfld(netMode));
            ILLabel vanilla = il.DefineLabel();
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Call, isZombie);
            c.Emit(OpCodes.Brfalse_S, vanilla);
            c.Emit(OpCodes.Ret); // :trollface:
            c.MarkLabel(vanilla);
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
                ILLabel vanilla2 = il.DefineLabel();
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Call, isZombie);
                c.Emit(OpCodes.Brfalse_S, vanilla2);
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_I4, zombieInventorySize);
                c.MarkLabel(vanilla2);
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
            if (!ZombieApocalypseConfig.GetInstance().ZombiesCanUseAmmoAndCoinSlots) {
                ILCursor c = new(il);
                ReplaceDefaultInventorySize(c, il, 50, 0);
                ReplaceDefaultInventorySize(c, il, 4, zombieInventorySize);
                ReplaceDefaultInventorySize(c, il, 58, zombieInventorySize);
            }
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
            if (!ZombieApocalypseConfig.GetInstance().ZombiesCanUseAmmoAndCoinSlots)
                ReplaceDefaultInventorySize(new(il), il, 54);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void ItemCheck_Inner(ILContext il) {
        try {
            ReplaceDefaultInventorySize(new(il), il);
            if (!ZombieApocalypseConfig.GetInstance().ZombiesCanUseAmmoAndCoinSlots)
                ReplaceDefaultInventorySize(new(il), il, 54);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void ItemSpace(ILContext il) {
        try {
            ReplaceDefaultInventorySize(new(il), il, 50);
            if (!ZombieApocalypseConfig.GetInstance().ZombiesCanUseAmmoAndCoinSlots)
                ReplaceDefaultInventorySize(new(il), il, 54);
            for (int _ = 0; _ < 2; _++)
                ReplaceDefaultInventorySize(new(il), il);
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
}
