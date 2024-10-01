using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse.Content.ExtraZombs;

public class ZombieWithGun : ModNPC {
    public const int fireRate = 6;
    public const float fireRange = 1000f;
    public const int fireDamage = 30;

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];
        NPCID.Sets.Zombies[Type] = true;
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.Zombie);
        NPC.lifeMax *= 2;
        NPC.defense += 5;
        NPC.aiStyle = -1;
    }

    public override void AI() {
        Player player = Main.player[NPC.target];
        NPC.TargetClosest(true);

        if (!player.active || player.dead) {
            NPC.TargetClosest(false);
            NPC.ai[1] = 0;
            return;
        }

        if (Collision.CanHitLine(NPC.Center, 1, 1, player.position, player.width, player.height)) {
            if (NPC.ai[1] >= fireRate && Vector2.Distance(NPC.Center, player.Center) <= fireRange) {
                NPC.ai[1] = 0;
                Vector2 shootDirection = player.Center - NPC.Center;
                shootDirection.Normalize();
                shootDirection *= 10f;
                if (player.position.X > NPC.position.X)
                    NPC.spriteDirection = 1;
                else
                    NPC.spriteDirection = 0;

                int projectileType = ProjectileID.Bullet;
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, shootDirection, projectileType, fireDamage, 1f, Main.myPlayer);
                Main.projectile[proj].friendly = false;
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].ArmorPenetration = 5;
                Main.projectile[proj].OriginalArmorPenetration = 5;
            }
            NPC.ai[1]++;
        } else {
            this.BasicFighterAI();
        }

        if (NPC.velocity.X > 0)
            NPC.direction = 1;
        else if (NPC.velocity.X < 0)
            NPC.direction = -1;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<M4A1Carbine>(), 200));
        npcLoot.Add(ItemDropRule.Common(ItemID.ZombieArm, 250));
    }
}
