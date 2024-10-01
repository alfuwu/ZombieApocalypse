using Terraria;
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
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => ZombieApocalypseConfig.GetInstance().EvenMoreZomb && spawnInfo.Player.ZoneOverworldHeight && (spawnInfo.Player.ZoneForest || spawnInfo.Player.ZonePurity) ? Main.IsItDay() ? 0.1f : 0.15f : 0f;
}
