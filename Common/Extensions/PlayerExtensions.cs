using Terraria;

namespace ZombieApocalypse.Common.Extensions;

public static class PlayerExtensions {
    public static bool IsZombie(this Player player) => player.GetModPlayer<ZombifiablePlayer>().Zombified;

    public static void SetZombie(this Player player, bool zombie) {
        player.GetModPlayer<ZombifiablePlayer>().Zombified = zombie;
        if (zombie && ZombieApocalypseConfig.GetInstance(out var cfg).ZombiesHaveADifferentSkinColor)
            player.skinColor = cfg.ZombieSkinColor;
        else
            player.skinColor = player.GetModPlayer<ZombifiablePlayer>().OriginalSkinColor;
    }
}
