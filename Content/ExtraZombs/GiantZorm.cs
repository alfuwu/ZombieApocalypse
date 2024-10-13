using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZombieApocalypse.Content.ExtraZombs;

public class GiantZormHead : WormHead {
    public override int BodyType => ModContent.NPCType<GiantZormBody>();

    public override int TailType => ModContent.NPCType<GiantZormTail>();

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
        NPC.CloneDefaults(NPCID.DiggerHead);
        NPC.aiStyle = -1;
        NPC.damage = 45;
        NPC.defense = 10;
        NPC.lifeMax = 200;
        NPC.value = 300f;
        NPC.knockBackResist = 0f;
        Banner = NPCID.Zombie;
        BannerItem = ItemID.ZombieBanner;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.Info.AddRange([
			BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
            
			new FlavorTextBestiaryInfoElement($"{ZombieApocalypse.Localization}.Bestiary.GiantZorm")
        ]);
    }

    public override void Init() {
        MinSegmentLength = 10;
        MaxSegmentLength = 14;

        CommonWormInit(this);
    }

    internal static void CommonWormInit(Worm worm) {
        worm.MoveSpeed = 5.5f;
        worm.Acceleration = 0.045f;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => ZombieApocalypseConfig.GetInstance().EvenMoreZomb && !spawnInfo.PlayerSafe && (spawnInfo.Player.ZoneNormalCaverns || spawnInfo.Player.ZoneNormalUnderground) && Main.hardMode ? 0.1f : 0f;
}

public class GiantZormBody : WormBody {
    public override void SetStaticDefaults() {
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifier = new() {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.DiggerBody);
        NPC.aiStyle = -1;
        NPC.damage = 28;
        NPC.defense = 20;
        NPC.lifeMax = 200;
        NPC.value = 300f;
        NPC.knockBackResist = 0f;
    }

    public override void Init() => GiantZormHead.CommonWormInit(this);
}

public class GiantZormTail : WormTail {
    public override void SetStaticDefaults() {
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifier = new() {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.DiggerTail);
        NPC.aiStyle = -1;
        NPC.damage = 26;
        NPC.defense = 30;
        NPC.lifeMax = 200;
        NPC.value = 300f;
        NPC.knockBackResist = 0f;
    }

    public override void Init() => GiantZormHead.CommonWormInit(this);
}