using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZombieApocalypse.Common;

public class ZombieItem : GlobalItem {
    public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.ZombieArm;

    public override void SetDefaults(Item item) {
        //item.StatsModifiedBy.Add(Mod);

        if (ZombieApocalypseConfig.GetInstance().IncreaseZombieArmDropChance)
            item.damage /= 2;
    }
}
