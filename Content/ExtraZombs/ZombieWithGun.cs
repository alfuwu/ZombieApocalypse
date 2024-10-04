using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse.Content.ExtraZombs;

public class ZombieWithGun : ModNPC {
    public const int fireRate = 6;
    public const float fireRange = 1000f;
    public const int fireDamage = 20;
    public Vector2 GunOffset => NPC.Center + new Vector2(22 * NPC.direction, -1f);

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifier = new() {
            Velocity = 1f,
            PortraitPositionXOverride = -8f
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        NPCID.Sets.Zombies[Type] = true;
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.Zombie);
        NPC.aiStyle = -1;
        NPC.damage = 40;
        NPC.lifeMax = 200;
        NPC.defense = 18;
        NPC.value = 1000f;
        NPC.knockBackResist = 0.5f;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

            new FlavorTextBestiaryInfoElement($"{ZombieApocalypse.Localization}.Bestiary.ZombieWithGun")
        ]);
    }

    public override void AI() {
        if (!NPC.HasPlayerTarget)
            NPC.TargetClosest(false);
        Player player = Main.player[NPC.target];
        if (NPC.DespawnEncouragement_AIStyle3_Fighters_NotDiscouraged(Type, NPC.position, NPC) && (!player.active || player.dead)) {
            NPC.TargetClosest(false);
            NPC.ai[1] = 0;
        }

        if (NPC.DespawnEncouragement_AIStyle3_Fighters_NotDiscouraged(Type, NPC.position, NPC) && Collision.CanHitLine(GunOffset, 1, 1, player.position, player.width, player.height) && Vector2.Distance(GunOffset, player.Center) <= fireRange) {
            NPC.velocity.X = 0;
            if (NPC.ai[1] >= fireRate) {
                NPC.ai[1] = 0;
                Vector2 shootDirection = player.Center - GunOffset;
                shootDirection.Normalize();
                shootDirection *= 10f;
                if (player.position.X > NPC.position.X)
                    NPC.direction = 1;
                else
                    NPC.direction = -1;

                int projectileType = ProjectileID.BulletDeadeye;
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), GunOffset, shootDirection, projectileType, fireDamage, 1f, Main.myPlayer);
                Main.projectile[proj].friendly = false;
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].ArmorPenetration = 5;
                Main.projectile[proj].OriginalArmorPenetration = 5;
            }
            NPC.ai[1]++;
        } else {
            this.BasicFighterAI();
        }
    }

    public override void FindFrame(int frameHeight) {
        this.BasicFighterFrame(frameHeight);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<M4A1Carbine>(), 200));
        npcLoot.Add(ItemDropRule.Common(ItemID.ZombieArm, 250));
    }
}
