using Terraria;
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

    public override void SpawnNPC(int npc, int tileX, int tileY) {
        if (NPCID.Sets.Zombies[npc])
            for (int _ = 0; _ < Main.rand.Next(2) + 3; _++)
                SpawnNPC(npc, tileX, tileY);
    }
}
