using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using ZombieApocalypse.Common.Hooks;

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
        if (cfg.DropUnusableItemsOnZombification && cfg.ZombiesHaveSmallerInventories && zombie) {
            IEntitySource itemSource_Death = player.GetSource_Death();

            for (int i = PlayerHooks.zombieInventorySize; i < 59; i++) {
                if (player.inventory[i].stack > 0) {
                    Item itemToDrop = player.inventory[i];
                    if (itemToDrop.stack > 0)
                        player.TryDroppingSingleItem(itemSource_Death, itemToDrop);
                }

                player.inventory[i].TurnToAir();
            }
        }
    }

    public static void SetFromInfection(this Player player, bool fromInfection) => player.GetModPlayer<ZombifiablePlayer>().FromInfection = fromInfection;

    public static bool IsZombifiableDeath(this Player player, PlayerDeathReason deathReason = null) {
        PlayerDeathReason damageSource = deathReason ?? player.GetModPlayer<ZombifiablePlayer>().LastDeathReason;
        return damageSource != null && !ZombieApocalypseConfig.GetInstance(out var cfg).DisableZombification && player.whoAmI == Main.myPlayer && (!cfg.OnlyTransformPlayerIfKilledByZombie || (damageSource.TryGetCausingNPC(out NPC npc) && NPCID.Sets.Zombies[npc.type]) || (damageSource.TryGetCausingEntity(out Entity entity) && entity is Player p && p.IsZombie()));
    }
}
