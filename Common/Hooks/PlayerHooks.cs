using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse.Common.Hooks;

public class PlayerHooks : ModHook {
    public static MethodInfo isZombie = ((Delegate)PlayerExtensions.IsZombie).Method;
    public static FieldInfo netMode = typeof(Main).GetField("netMode", BindingFlags.Public | BindingFlags.Static);

    public override void Apply() {
        IL_Player.QuickHeal_GetItemToUse += QuickHeal_GetItemToUse;
        IL_Player.QuickMana_GetItemToUse += QuickMana_GetItemToUse;
        IL_Player.QuickMount_GetItemToUse += QuickMount_GetItemToUse;
        IL_Player.QuickStackAllChests += QuickStackAllChests;
        IL_Player.ScrollHotbar += ScrollHotbar;

        On_Player.InOpposingTeam += OnInOpposingTeam;
    }

    public override void Unapply() {
        IL_Player.QuickHeal_GetItemToUse -= QuickHeal_GetItemToUse;
        IL_Player.QuickMana_GetItemToUse -= QuickMana_GetItemToUse;
        IL_Player.QuickMount_GetItemToUse -= QuickMount_GetItemToUse;
        IL_Player.QuickStackAllChests -= QuickStackAllChests;
        IL_Player.ScrollHotbar -= ScrollHotbar;

        On_Player.InOpposingTeam -= OnInOpposingTeam;
    }

    private bool OnInOpposingTeam(On_Player.orig_InOpposingTeam orig, Player self, Player otherPlayer) =>
        orig(self, otherPlayer) || (((self.IsZombie() && !otherPlayer.IsZombie()) || (otherPlayer.IsZombie() && !self.IsZombie())) && ZombieApocalypseConfig.GetInstance().ZombiesCanFightOtherPlayersWithoutPvP);

    private void QuickHeal_GetItemToUse(ILContext il) {
        try {
            ILCursor c = new(il);
            c.GotoNext(MoveType.After, i => i.MatchLdcI4(98),
                i => i.MatchStloc3());
            c.GotoNext(i => i.MatchLdcI4(0));
            ILLabel vanilla = il.DefineLabel();
            c.Emit(OpCodes.Ldarg_0); // load player var
            c.Emit(OpCodes.Call, isZombie);
            c.Emit(OpCodes.Brfalse_S, vanilla);
            c.Emit(OpCodes.Ldc_I4, 4); // muahahaha
            c.Emit(OpCodes.Stloc_3);
            c.MarkLabel(vanilla);
            DumpIL(il);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private static void ReplaceDefaultInventorySize(ILCursor c, ILContext il) {
        c.GotoNext(MoveType.After, i => i.MatchLdcI4(58));
        ILLabel vanilla = il.DefineLabel();
        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Call, isZombie);
        c.Emit(OpCodes.Brfalse_S, vanilla);
        c.Emit(OpCodes.Pop);
        c.Emit(OpCodes.Ldc_I4, 4);
        c.MarkLabel(vanilla);
    }

    private void QuickMana_GetItemToUse(ILContext il) {
        try {
            ILCursor c = new(il);
            ReplaceDefaultInventorySize(c, il);
            c.GotoNext(MoveType.After, i => i.MatchLdcI4(40));
            ILLabel vanilla2 = il.DefineLabel();
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Call, isZombie);
            c.Emit(OpCodes.Brfalse_S, vanilla2);
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4, 0); // too lazy to make a label that just skips the if statement
            c.MarkLabel(vanilla2);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void QuickMount_GetItemToUse(ILContext il) {
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

    private void ScrollHotbar(ILContext il) { // not working properly
        try {
            ILCursor c = new(il);
            c.GotoNext(MoveType.After, i => i.MatchLdcI4(9));
            ILLabel vanilla = il.DefineLabel();
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Call, isZombie);
            c.Emit(OpCodes.Brfalse_S, vanilla);
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4, 4);
            c.MarkLabel(vanilla);
            for (int _ = 0; _ < 2; _++) { // do twice
                if (_ == 0) // why is it compiled like this
                    c.GotoPrev(MoveType.After, i => i.MatchLdcI4(4));
                else
                    c.GotoNext(MoveType.After, i => i.MatchLdcI4(10));
                ILLabel vanilla2 = il.DefineLabel();
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Call, isZombie);
                c.Emit(OpCodes.Brfalse_S, vanilla2);
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_I4, 5);
                c.MarkLabel(vanilla2);
            }
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }
}
