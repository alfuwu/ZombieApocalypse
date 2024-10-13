using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse.Content.ExtraZombs;

public class Zombunny : ModNPC {
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Bunny];
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifier = new() {
            Velocity = 1f
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        NPCID.Sets.Zombies[Type] = true;
        NPCID.Sets.ShimmerTransformToNPC[Type] = NPCID.Bunny;
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.Zombie);
        NPC.aiStyle = -1;
        NPC.damage = 14;
        NPC.lifeMax = 20;
        NPC.defense = 0;
        NPC.value = 4f;
        NPC.DeathSound = SoundID.NPCDeath1;
        Banner = NPCID.Zombie;
        BannerItem = ItemID.ZombieBanner;
    }

    public override void AI() {
        this.BasicFighterAI(false);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

            new FlavorTextBestiaryInfoElement($"{ZombieApocalypse.Localization}.Bestiary.Zombunny")
        ]);
    }

    public override void FindFrame(int frameHeight) {
        if (NPC.velocity.Y == 0f) {
            if (NPC.direction == 1)
                NPC.spriteDirection = 1;
            else if (NPC.direction == -1)
                NPC.spriteDirection = -1;

            if (NPC.velocity.X == 0f) {
                NPC.frame.Y = 0;
                NPC.frameCounter = 0.0;
                return;
            }

            NPC.frameCounter += Math.Abs(NPC.velocity.X) * 1f;
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter > 6.0) {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0.0;
            }

            if (NPC.frame.Y / frameHeight >= Main.npcFrameCount[Type])
                NPC.frame.Y = 0;
        } else if (NPC.velocity.Y < 0f) {
            NPC.frameCounter = 0.0;
            NPC.frame.Y = frameHeight * 4;
        } else if (NPC.velocity.Y > 0f) {
            NPC.frameCounter = 0.0;
            NPC.frame.Y = frameHeight * 6;
        }
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode != NetmodeID.Server && NPC.life <= 0) {
            int headGore = Mod.Find<ModGore>($"{Name}_Gore_Head").Type;
            int legGore = Mod.Find<ModGore>($"{Name}_Gore_Leg").Type;
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 6), NPC.velocity, legGore);
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => ZombieApocalypseConfig.GetInstance().EvenMoreZomb && Main.IsItDay() && spawnInfo.Player.ZoneForest ? 0.005f : 0f;
}
