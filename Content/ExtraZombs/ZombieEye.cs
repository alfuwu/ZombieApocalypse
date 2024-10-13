using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZombieApocalypse.Content.ExtraZombs;

public class ZombieEye : ModNPC {
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.DemonEye];
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifier = new() {
            Position = new(6f, -15f),
            PortraitPositionYOverride = -35f
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        NPCID.Sets.DemonEyes[Type] = true;
        NPCID.Sets.Zombies[Type] = true;
        NPCID.Sets.ShimmerTransformToNPC[Type] = NPCID.DemonEye;
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.DemonEye);
        NPC.damage = 18;
        NPC.defense = 2;
        NPC.lifeMax = 60;
        NPC.value = 75f;
        NPC.knockBackResist = 0.8f;
        Banner = NPCID.Zombie;
        BannerItem = ItemID.ZombieBanner;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

            new FlavorTextBestiaryInfoElement($"{ZombieApocalypse.Localization}.Bestiary.ZombieEye")
        ]);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode != NetmodeID.Server && NPC.life <= 0) {
            int headGore = Mod.Find<ModGore>($"{Name}_Gore_Head").Type;
            int legGore = Mod.Find<ModGore>($"{Name}_Gore_Body").Type;
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 6), NPC.velocity, legGore);
        }
    }

    public override void FindFrame(int frameHeight) {
        if (NPC.velocity.X > 0f) {
            NPC.spriteDirection = 1;
            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);
        }

        if (NPC.velocity.X < 0f) {
            NPC.spriteDirection = -1;
            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 3.14f;
        }

        NPC.frameCounter += 1.0;
        if (NPC.frameCounter >= 8) {
            NPC.frame.Y += frameHeight;
            NPC.frameCounter = 0.0;
        }

        if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type])
            NPC.frame.Y = 0;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => ZombieApocalypseConfig.GetInstance().EvenMoreZomb && !Main.IsItDay() && spawnInfo.Player.ZoneForest ? 0.3f : 0f;
}
