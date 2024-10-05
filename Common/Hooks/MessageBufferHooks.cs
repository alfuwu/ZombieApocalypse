using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace ZombieApocalypse.Common.Hooks;

public class MessageBufferHooks : ModHook {
    public static readonly FieldInfo player = typeof(Main).GetField("player", BindingFlags.Public | BindingFlags.Static);
    public static readonly FieldInfo whoAmI = typeof(MessageBuffer).GetField("whoAmI", BindingFlags.Public | BindingFlags.Instance);

    public override void Apply() {
        IL_MessageBuffer.GetData += GetData;
    }

    public override void Unapply() {
        IL_MessageBuffer.GetData -= GetData;
    }

    private void GetData(ILContext il) {
        try {
            ILCursor c = new(il);
            c.GotoNext(i => i.MatchLdelemRef(),
                i => i.MatchLdfld(PlayerHooks.hostile),
                i => i.MatchBrfalse(out _));
            c.GotoNext(MoveType.After, i => i.MatchLdfld(PlayerHooks.hostile),
                i => i.MatchBrfalse(out _));
            ILLabel skipHostile = il.DefineLabel();
            c.MarkLabel(skipHostile);
            c.GotoPrev(i => i.MatchLdsfld(player),
                i => i.MatchLdloc(453)); // god why are there so many locals
            c.Emit(OpCodes.Ldsfld, player);
            c.Emit(OpCodes.Ldloc, 453);
            c.Emit(OpCodes.Nop); // match stack size
            c.Emit(OpCodes.Nop);
            c.Emit(OpCodes.Ldelem_Ref);
            c.Emit(OpCodes.Call, PlayerHooks.isZombie);
            c.Emit(OpCodes.Ldsfld, player);
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldfld, whoAmI);
            c.Emit(OpCodes.Ldelem_Ref);
            c.Emit(OpCodes.Call, PlayerHooks.isZombie);
            c.Emit(OpCodes.Bne_Un, skipHostile); // if the two players are zombie and human, skip the hostile check
            DumpIL(il);
            // why does this not work?
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }
}
