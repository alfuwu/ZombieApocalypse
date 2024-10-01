using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZombieApocalypse.Content.ExtraZombs;

public class ZombieEye : ModNPC {
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.DemonEye];
        NPCID.Sets.DemonEyes[Type] = true;
        NPCID.Sets.Zombies[Type] = true;
        NPCID.Sets.ShimmerTransformToNPC[Type] = NPCID.DemonEye;
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.DemonEye);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode != NetmodeID.Server && NPC.life <= 0) {
            int headGore = Mod.Find<ModGore>($"{Name}_Gore_Head").Type;
            int legGore = Mod.Find<ModGore>($"{Name}_Gore_Body").Type;
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 6), NPC.velocity, legGore);
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => ZombieApocalypseConfig.GetInstance().EvenMoreZomb && !Main.IsItDay() && (spawnInfo.Player.ZoneForest || spawnInfo.Player.ZonePurity) ? 0.3f : 0f;
}
