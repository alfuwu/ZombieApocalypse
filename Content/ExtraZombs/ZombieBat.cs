using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZombieApocalypse.Content.ExtraZombs;

public class ZombieBat : ModNPC {
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.CaveBat];
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifier = new() {
            Position = new(0f, -13f),
            PortraitPositionYOverride = -30f
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        NPCID.Sets.Zombies[Type] = true;
        NPCID.Sets.ShimmerTransformToNPC[Type] = NPCID.CaveBat;
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.CaveBat);
        NPC.damage = 13;
        NPC.defense = 2;
        NPC.lifeMax = 16;
        NPC.value = 90f;
        NPC.knockBackResist = 0.8f;
        Banner = NPCID.Zombie;
        BannerItem = ItemID.ZombieBanner;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

            new FlavorTextBestiaryInfoElement($"{ZombieApocalypse.Localization}.Bestiary.ZombieBat")
        ]);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>($"{Name}_Gore").Type, 1f);
    }

    public override void FindFrame(int frameHeight) {
        if (NPC.velocity.X > 0f)
            NPC.spriteDirection = 1;

        if (NPC.velocity.X < 0f)
            NPC.spriteDirection = -1;

        NPC.rotation = NPC.velocity.X * 0.1f;
        NPC.frameCounter += 1.0;
        int num292 = 6;
        int num293 = Main.npcFrameCount[Type] - 1;

        if (NPC.frameCounter >= num292) {
            NPC.frame.Y += frameHeight;
            NPC.frameCounter = 0.0;
        }

        if (NPC.frame.Y >= frameHeight * num293)
            NPC.frame.Y = 0;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => ZombieApocalypseConfig.GetInstance().EvenMoreZomb && spawnInfo.Player.ZoneNormalCaverns ? 0.1f : 0f;
}
