using Terraria;
using Terraria.ModLoader;

namespace ZombieApocalypse.Content;

public class BodyFlux : ModBuff {
    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
    }
}
