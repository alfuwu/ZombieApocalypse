using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZombieApocalypse.Content.ExtraZombs;

public class ZombieSlime: ModNPC {
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.BlueSlime];
        //NPCID.Sets.CanConvertIntoCopperSlimeTownNPC[Type] = true;
        NPCID.Sets.Zombies[Type] = true;
        NPCID.Sets.ShimmerTransformToNPC[Type] = NPCID.BlueSlime;
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.BlueSlime);
        NPC.damage = 7;
        NPC.defense = 2;
        NPC.lifeMax = 25;
        NPC.value = 25f;
        NPC.knockBackResist = 1f;
        Banner = NPCID.Zombie;
        BannerItem = ItemID.ZombieBanner;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

            new FlavorTextBestiaryInfoElement($"{ZombieApocalypse.Localization}.Bestiary.ZombieSlime")
        ]);
    }

    public override void FindFrame(int frameHeight) {
        int num2 = 0;
        if (NPC.aiAction == 0)
            num2 = NPC.velocity.Y < 0f ? 2 : NPC.velocity.Y > 0f ? 3 : NPC.velocity.X != 0f ? 1 : 0;
        else if (NPC.aiAction == 1)
            num2 = 4;
        NPC.frameCounter += 1.0;
        if (num2 > 0)
            NPC.frameCounter += 1.0;
        if (num2 == 4)
            NPC.frameCounter += 1.0;
        if (NPC.frameCounter >= 8.0) {
            NPC.frame.Y += frameHeight;
            NPC.frameCounter = 0.0;
        }
        if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type])
            NPC.frame.Y = 0;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => ZombieApocalypseConfig.GetInstance().EvenMoreZomb && spawnInfo.Player.ZoneForest ? Main.IsItDay() ? 0.1f : 0.15f : 0f;
}
