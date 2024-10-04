using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using ZombieApocalypse.Common.Extensions;
using Terraria.GameContent.Bestiary;

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
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

            new FlavorTextBestiaryInfoElement($"{ZombieApocalypse.Localization}.Bestiary.BucketZombie")
        ]);
    }

    public override void AI() {
        this.BasicFighterAI();
    }

    public override void FindFrame(int frameHeight) {
        this.BasicFighterFrame(frameHeight);
    }
}
