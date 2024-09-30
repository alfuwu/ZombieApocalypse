using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ZombieApocalypse.Content;

public class ZombieRarity : ModRarity {
    public override Color RarityColor => new(140 + Main.DiscoR / 10, 200 + (byte)(Main.DiscoG / 1.5f), 150 + Main.DiscoB / 7);
}
