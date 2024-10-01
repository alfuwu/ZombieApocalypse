using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZombieApocalypse.Content.ExtraZombs;

public class ZormHead : WormHead {
    public override int BodyType => ModContent.NPCType<ZormBody>();

    public override int TailType => ModContent.NPCType<ZormTail>();

    public override void SetStaticDefaults() {
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifier = new() { 
            CustomTexturePath = Texture.Replace("Head", "_Bestiary"),
            Position = new(40f, 24f),
            PortraitPositionXOverride = 0f,
            PortraitPositionYOverride = 12f
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        NPCID.Sets.Zombies[Type] = true;
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.GiantWormHead);
        NPC.aiStyle = -1;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.Info.AddRange([
			BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
            
			new FlavorTextBestiaryInfoElement($"{ZombieApocalypse.Localization}.Bestiary.Zorm")
        ]);
    }

    public override void Init() {
        MinSegmentLength = 6;
        MaxSegmentLength = 8;

        CommonWormInit(this);
    }

    internal static void CommonWormInit(Worm worm) {
        worm.MoveSpeed = 5.5f;
        worm.Acceleration = 0.045f;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => ZombieApocalypseConfig.GetInstance().EvenMoreZomb && !spawnInfo.PlayerSafe && (spawnInfo.Player.ZoneNormalCaverns || spawnInfo.Player.ZoneNormalUnderground) ? Main.hardMode ? 0.01f : 0.1f : 0f;
}

internal class ZormBody : WormBody {
    public override void SetStaticDefaults() {
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifier = new() {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        // cant add to the zombies set because the mod will try to spawn multiple of the body segments, which wouldnt work as intended
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.GiantWormBody);
        NPC.aiStyle = -1;
    }

    public override void Init() => ZormHead.CommonWormInit(this);
}

internal class ZormTail : WormTail {
    public override void SetStaticDefaults() {
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifier = new() {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.GiantWormTail);
        NPC.aiStyle = -1;
    }

    public override void Init() => ZormHead.CommonWormInit(this);
}