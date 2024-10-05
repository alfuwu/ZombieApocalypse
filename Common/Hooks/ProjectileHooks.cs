using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace ZombieApocalypse.Common.Hooks;

public class ProjectileHooks : ModHook {
    public static readonly FieldInfo owner = typeof(Projectile).GetField("owner", BindingFlags.Public | BindingFlags.Instance);
    public static readonly FieldInfo myPlayer = typeof(Main).GetField("myPlayer", BindingFlags.Public | BindingFlags.Static);

    public override void Apply() {
        IL_Projectile.AI_185_LifeDrain += AI_185_LifeDrain;
        IL_Projectile.Damage += Damage;
        IL_Projectile.ghostHeal += ghostHeal;
    }

    public override void Unapply() {
        IL_Projectile.AI_185_LifeDrain -= AI_185_LifeDrain;
        IL_Projectile.Damage -= Damage;
        IL_Projectile.ghostHeal -= ghostHeal;
    }

    private void AI_185_LifeDrain(ILContext il) {
        try {
            ILCursor c = new(il);
            c.GotoNext(MoveType.After, i => i.MatchLdloc(6),
                i => i.MatchLdfld(PlayerHooks.hostile),
                i => i.MatchBrfalse(out _));
            ILLabel skipHostile = il.DefineLabel();
            c.MarkLabel(skipHostile);
            c.GotoPrev(i => i.MatchLdloc(6));
            c.Emit(OpCodes.Ldsfld, MessageBufferHooks.player);
            c.Emit(OpCodes.Ldsfld, myPlayer);
            c.Emit(OpCodes.Ldelem_Ref);
            c.Emit(OpCodes.Call, PlayerHooks.isZombie);
            c.Emit(OpCodes.Ldloc_S, (byte)6);
            c.Emit(OpCodes.Call, PlayerHooks.isZombie);
            c.Emit(OpCodes.Bne_Un, skipHostile);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void Damage(ILContext il) {
        try {
            ILCursor c = new(il);
            c.GotoNext(MoveType.After, i => i.MatchLdsfld(MessageBufferHooks.player),
                i => i.MatchLdsfld(myPlayer),
                i => i.MatchLdelemRef(),
                i => i.MatchLdfld(PlayerHooks.hostile),
                i => i.MatchBrfalse(out _));
            ILLabel skipHostile = il.DefineLabel();
            c.MarkLabel(skipHostile);
            c.GotoPrev(i => i.MatchLdsfld(MessageBufferHooks.player));
            c.Emit(OpCodes.Call, PlayerHooks.getConfig);
            c.Emit(OpCodes.Call, PlayerHooks.ZombiesCanFightOtherPlayersWithoutPvP);
            c.Emit(OpCodes.Brtrue_S, skipHostile);
            ILLabel skipAll = null;
            c.GotoNext(MoveType.After, i => i.MatchLdloc(115),
                i => i.MatchLdfld(PlayerHooks.hostile),
                i => i.MatchBrfalse(out skipAll));
            ILLabel skipHostile2 = il.DefineLabel();
            c.MarkLabel(skipHostile2);
            c.GotoPrev(i => i.MatchLdloc(115));
            c.Emit(OpCodes.Ldsfld, MessageBufferHooks.player);
            c.Emit(OpCodes.Ldsfld, myPlayer);
            c.Emit(OpCodes.Ldelem_Ref);
            c.Emit(OpCodes.Call, PlayerHooks.isZombie);
            c.Emit(OpCodes.Ldloc_S, (byte)115);
            c.Emit(OpCodes.Call, PlayerHooks.isZombie);
            c.Emit(OpCodes.Bne_Un, skipHostile2);
            c.Emit(OpCodes.Ldsfld, MessageBufferHooks.player);
            c.Emit(OpCodes.Ldsfld, myPlayer);
            c.Emit(OpCodes.Ldelem_Ref);
            c.Emit(OpCodes.Ldfld, PlayerHooks.hostile);
            c.Emit(OpCodes.Brfalse, skipAll);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private void ghostHeal(ILContext il) {
        try {
            ILCursor c = new(il);
            ILLabel skip = null;
            c.GotoNext(MoveType.After, i => i.MatchLdsfld(MessageBufferHooks.player),
                i => i.MatchLdloc(4),
                i => i.MatchLdelemRef(),
                i => i.MatchLdfld(PlayerHooks.active),
                i => i.MatchBrfalse(out skip));
            ILLabel vanilla = il.DefineLabel();
            c.Emit(OpCodes.Call, PlayerHooks.getConfig);
            c.Emit(OpCodes.Call, PlayerHooks.ZombiesCanFightOtherPlayersWithoutPvP);
            c.Emit(OpCodes.Brfalse_S, vanilla);
            c.Emit(OpCodes.Ldsfld, MessageBufferHooks.player);
            c.Emit(OpCodes.Ldloc, 4);
            c.Emit(OpCodes.Ldelem_Ref);
            c.Emit(OpCodes.Call, PlayerHooks.isZombie);
            c.Emit(OpCodes.Ldsfld, MessageBufferHooks.player);
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldfld, owner);
            c.Emit(OpCodes.Ldelem_Ref);
            c.Emit(OpCodes.Call, PlayerHooks.isZombie);
            c.Emit(OpCodes.Bne_Un, skip);
            c.MarkLabel(vanilla);
        } catch (Exception e) {
            DumpIL(il);
            throw new ILPatchFailureException(Mod, il, e);
        }
    }
}
