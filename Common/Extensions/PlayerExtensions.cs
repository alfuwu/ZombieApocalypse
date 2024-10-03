using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace ZombieApocalypse.Common.Extensions;

public static class PlayerExtensions {
    public static bool IsZombie(this Player player) => player.TryGetModPlayer(out ZombifiablePlayer plr) && plr.Zombified;

    public static void SetZombie(this Player player, bool zombie) {
        ZombifiablePlayer p = player.GetModPlayer<ZombifiablePlayer>();
        if (p.OriginalSkinColor == new Color(0, 0, 0) && zombie)
            p.OriginalSkinColor = player.skinColor;
        p.Zombified = zombie;
        if (zombie && ZombieApocalypseConfig.GetInstance(out var cfg).ZombiesHaveADifferentSkinColor)
            player.skinColor = cfg.ZombieSkinColor;
        else
            player.skinColor = p.OriginalSkinColor;
    }

    public static bool IsZombifiableDeath(this Player player, PlayerDeathReason deathReason = null) {
        PlayerDeathReason damageSource = deathReason ?? player.GetModPlayer<ZombifiablePlayer>().LastDeathReason;
        return damageSource != null && player.whoAmI == Main.myPlayer && (!ZombieApocalypseConfig.GetInstance().OnlyTransformPlayerIfKilledByZombie || (damageSource.TryGetCausingNPC(out NPC npc) && NPCID.Sets.Zombies[npc.type]) || (damageSource.TryGetCausingEntity(out Entity entity) && entity is Player p && p.IsZombie()));
    }
}
