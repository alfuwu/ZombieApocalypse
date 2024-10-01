using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse.Content.ExtraZombs;

public class Zombunny : ModNPC {
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Bunny];
        NPCID.Sets.Zombies[Type] = true;
        NPCID.Sets.ShimmerTransformToNPC[Type] = NPCID.Bunny;
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.Zombie);
        NPC.aiStyle = -1;
        NPC.lifeMax /= 3;
        NPC.damage /= 2;
    }

    public override void AI() {
        this.BasicFighterAI(false);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode != NetmodeID.Server && NPC.life <= 0) {
            int headGore = Mod.Find<ModGore>($"{Name}_Gore_Head").Type;
            int legGore = Mod.Find<ModGore>($"{Name}_Gore_Leg").Type;
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 6), NPC.velocity, legGore);
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => ZombieApocalypseConfig.GetInstance().EvenMoreZomb && Main.IsItDay() && spawnInfo.Player.ZoneOverworldHeight && (spawnInfo.Player.ZoneForest || spawnInfo.Player.ZonePurity || spawnInfo.Player.ZoneRain) ? 0.005f : 0f;
}
