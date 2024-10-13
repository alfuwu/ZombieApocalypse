using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using ZombieApocalypse.Common.Extensions;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;

namespace ZombieApocalypse.Content.ExtraZombs;

public class BucketZombie : ModNPC {
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifier = new() {
            Velocity = 1f,
            PortraitPositionXOverride = 2f
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        NPCID.Sets.Zombies[Type] = true;
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.Zombie);
        NPC.aiStyle = -1;
        NPC.damage = 14;
        NPC.lifeMax = 45;
        NPC.defense = 7;
        NPC.value = 80f;
        NPC.knockBackResist = 0.5f;
        Banner = NPCID.Zombie;
        BannerItem = ItemID.ZombieBanner;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

            new FlavorTextBestiaryInfoElement($"{ZombieApocalypse.Localization}.Bestiary.BucketZombie")
        ]);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode != NetmodeID.Server && NPC.life <= 0) {
            int headGore = Mod.Find<ModGore>($"{Name}_Gore_Head").Type;
            int bodyGore = Mod.Find<ModGore>($"{Name}_Gore_Body").Type;
            int armGore = Mod.Find<ModGore>($"{Name}_Gore_Arm").Type;
            int bucket = Mod.Find<ModGore>("Bucket_Gore").Type;
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, bucket, 1f);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, bodyGore);
        }
    }

    public override void AI() {
        this.BasicFighterAI();
    }

    public override void FindFrame(int frameHeight) {
        this.BasicFighterFrame(frameHeight);
    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo) => ZombieApocalypseConfig.GetInstance().EvenMoreZomb && !Main.IsItDay() && spawnInfo.Player.ZoneForest ? 0.3f : 0f;
}
