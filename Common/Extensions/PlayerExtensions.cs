using Microsoft.Xna.Framework;
using Terraria;

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
}
