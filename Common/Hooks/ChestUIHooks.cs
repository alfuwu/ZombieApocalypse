using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.UI;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse.Common.Hooks;

public class ChestUIHooks : ModHook {
    public override void Apply() {
        IL_ChestUI.QuickStack += QuickStack;

        On_ChestUI.MoveCoins += OnMoveCoins;
    }
    public override void Unapply() {
        IL_ChestUI.QuickStack -= QuickStack;

        On_ChestUI.MoveCoins -= OnMoveCoins;
    }

    private long OnMoveCoins(On_ChestUI.orig_MoveCoins orig, Item[] pInv, Item[] cInv, ContainerTransferContext context) =>
        ZombieApocalypseConfig.GetInstance().ZombiesCanUseAmmoAndCoinSlots || pInv != Main.LocalPlayer.inventory || !Main.LocalPlayer.IsZombie() ? orig(pInv, cInv, context) : 0;


    private void QuickStack(ILContext il) {
        try {
            if (ZombieApocalypseConfig.GetInstance().ZombiesHaveSmallerInventories) {
                ILCursor c = new(il);
                c.GotoNext(MoveType.After, i => i.MatchLdcI4(50));
                ILLabel vanilla = il.DefineLabel();
                c.Emit(OpCodes.Ldloc_0); // player is the first local var
                c.Emit(OpCodes.Call, PlayerHooks.isZombie);
                c.Emit(OpCodes.Brfalse_S, vanilla);
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_I4, PlayerHooks.zombieInventorySize);
                c.MarkLabel(vanilla);
            }
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }
}
