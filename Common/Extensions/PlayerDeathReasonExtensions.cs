using Terraria;
using Terraria.DataStructures;

namespace ZombieApocalypse.Common.Extensions;

public static class PlayerDeathReasonExtensions {
    public static bool TryGetCausingNPC(this PlayerDeathReason damageSource, out NPC npc) {
        if (Main.npc.IndexInRange(damageSource.SourceNPCIndex)) {
            npc = Main.npc[damageSource.SourceNPCIndex];
            return true;
        }
        npc = null;
        return false;
    }
}
