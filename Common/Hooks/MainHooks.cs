using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse.Common.Hooks;

public class MainHooks : ModHook {
    public static readonly MethodInfo localPlayer = typeof(Main).GetMethod("get_LocalPlayer", BindingFlags.Public | BindingFlags.Static);
    
    public override void Apply() {
        IL_Main.GUIHotbarDrawInner += GUIHotbarDrawInner;
    }

    public override void Unapply() {
        IL_Main.GUIHotbarDrawInner -= GUIHotbarDrawInner;
    }

    private void GUIHotbarDrawInner(ILContext il) {
        try {
            ILCursor c = new(il);
            c.GotoNext(MoveType.After, i => i.MatchLdcR4(236f)); // modifying position of item name text
            ILLabel vanilla = il.DefineLabel();
            c.Emit(OpCodes.Call, PlayerHooks.getConfig);
            c.Emit(OpCodes.Call, PlayerHooks.ZombiesHaveSmallerInventories);
            c.Emit(OpCodes.Brfalse_S, vanilla);
            c.Emit(OpCodes.Call, localPlayer); // load local player onto stack
            c.Emit(OpCodes.Call, PlayerHooks.isZombie);
            c.Emit(OpCodes.Brfalse_S, vanilla);
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_R4, 126f);
            c.MarkLabel(vanilla);

            c.GotoNext(MoveType.After, i => i.MatchLdcI4(10)); // modifying amount of hotbar slots shown
            ILLabel vanilla2 = il.DefineLabel();
            c.Emit(OpCodes.Call, PlayerHooks.getConfig);
            c.Emit(OpCodes.Call, PlayerHooks.ZombiesHaveSmallerInventories);
            c.Emit(OpCodes.Brfalse_S, vanilla2);
            c.Emit(OpCodes.Call, localPlayer);
            c.Emit(OpCodes.Call, PlayerHooks.isZombie);
            c.Emit(OpCodes.Brfalse_S, vanilla2);
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4, PlayerHooks.zombieInventorySize);
            c.MarkLabel(vanilla2);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }
}
