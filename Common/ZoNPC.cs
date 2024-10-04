using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZombieApocalypse.Common;

public class ZoNPC : GlobalNPC { // dont question the name, i couldnt think of anything better
    // shut
    // i know youre still thinking about the name
    // stop
    // now!
    // AAAAA
    // why dont u listen to me ever :c
    // anyway

    public override bool AppliesToEntity(NPC npc, bool lateInstantiation) => NPCID.Sets.Zombies[npc.type];

    public override void SetDefaults(NPC npc) {
        if (ZombieApocalypseConfig.GetInstance().StrongerZomb) {
            npc.damage = (int)(npc.damage * 1.2f);
            npc.defense += 3;
            npc.defDefense += 3;
            npc.knockBackResist *= 1.2f;
            npc.lifeMax = (int)(npc.lifeMax * 1.05f);
            npc.life = (int)(npc.life * 1.05f);
        }
    }

    private static void TrySpawn(NPC npc, int maxAttempts = 10) {
        for (int attempt = 0; attempt < maxAttempts; attempt++) {
            float spawnX = npc.Center.X + Main.rand.NextFloat(-500f, 500f);
            float spawnY = npc.Center.Y + Main.rand.NextFloat(-500f, 500f);

            bool offscreenForAllPlayers = true;
            foreach (Player player in Main.player) {
                if (player.active && player.whoAmI != -1) {
                    if (spawnX > player.Center.X - Main.screenWidth / 2f - 100f &&
                        spawnX < player.Center.X + Main.screenWidth / 2f + 100f &&
                        spawnY > player.Center.Y - Main.screenHeight / 2f - 100f &&
                        spawnY < player.Center.Y + Main.screenHeight / 2f + 100f) {
                        offscreenForAllPlayers = false;
                        break;
                    }
                }
            }

            if (offscreenForAllPlayers && !Collision.SolidCollision(new(spawnX, spawnY), npc.width, npc.height)) {
                NPC.NewNPC(npc.GetSource_Misc("ZombieApocalypse"), (int)spawnX, (int)spawnY, npc.type);
                break;
            }
        }
    }

    public override void OnSpawn(NPC npc, IEntitySource source) {
        if (source is not EntitySource_Misc && ZombieApocalypseConfig.GetInstance().MoreZomb)
            for (int _ = 0; _ < 1 + Main.rand.Next(2); _++)
                TrySpawn(npc);
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        if (ZombieApocalypseConfig.GetInstance().IncreaseZombieArmDropChance)
            foreach (var rule in npcLoot.Get())
                if (rule is CommonDrop drop && drop.itemId == ItemID.ZombieArm)
                    drop.chanceDenominator = 20; // increase to 1/20 (5%) chance
    }

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) {
        if (ZombieApocalypseConfig.GetInstance().MoreZomb)
            maxSpawns = (int)(maxSpawns * 1.5f);
    }
}
