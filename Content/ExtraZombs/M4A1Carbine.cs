using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace ZombieApocalypse.Content.ExtraZombs;

public class M4A1Carbine : ModItem {
    public override void SetDefaults() {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Bullet, 5, 16f, true);

        Item.SetWeaponValues(23, 1f);

        Item.width = 54;
        Item.height = 22;
        Item.rare = ItemRarityID.Pink; 
        Item.UseSound = SoundID.Item11;
    }

    public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextFloat() >= 0.18f;

    public override bool NeedsAmmo(Player player) => true;

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
        velocity = velocity.RotatedByRandom(MathHelper.ToRadians(3));
    }

    public override Vector2? HoldoutOffset() => new(-6f, -2f);
}
