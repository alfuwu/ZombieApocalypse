using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace ZombieApocalypse.Common.Extensions;

public static class PlayerExtensions {
    public static bool IsZombie(this Player player) => player.TryGetModPlayer(out ZombifiablePlayer plr) && plr.Zombified;

    public static void SetZombie(this Player player, bool zombie, bool quiet = false) {
        ZombifiablePlayer p = player.GetModPlayer<ZombifiablePlayer>();
        if (zombie && p.OriginalSkinColor == new Color(0, 0, 0))
            p.OriginalSkinColor = player.skinColor;
        p.Zombified = zombie;
        if (ZombieApocalypseConfig.GetInstance(out var cfg).ZombiesHaveADifferentSkinColor && zombie)
            player.skinColor = cfg.ZombieSkinColor;
        else if (!quiet)
            player.skinColor = p.OriginalSkinColor;
        if (player.whoAmI == Main.myPlayer) {
            if (!quiet) {
                if (cfg.ZombificationParticles && zombie)
                    ZombifiablePlayer.ZombificationDusts(player);
                p.BroadcastMessage(quiet, p.FromInfection);
            }
            if (cfg.ApplyCustomVisionShaderToZombies)
                Filters.Scene[ZombieApocalypse.VisionShader].GetShader().UseIntensity(zombie ? 1 : -1);
        }
    }

    public static void SetFromInfection(this Player player, bool fromInfection) => player.GetModPlayer<ZombifiablePlayer>().FromInfection = fromInfection;

    public static bool IsZombifiableDeath(this Player player, PlayerDeathReason deathReason = null) {
        PlayerDeathReason damageSource = deathReason ?? player.GetModPlayer<ZombifiablePlayer>().LastDeathReason;
        return damageSource != null && player.whoAmI == Main.myPlayer && (!ZombieApocalypseConfig.GetInstance().OnlyTransformPlayerIfKilledByZombie || (damageSource.TryGetCausingNPC(out NPC npc) && NPCID.Sets.Zombies[npc.type]) || (damageSource.TryGetCausingEntity(out Entity entity) && entity is Player p && p.IsZombie()));
    }
}
