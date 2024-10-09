using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using ZombieApocalypse.Common;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse;

public class ZombieApocalypse : Mod {
    public static Mod Mod { get; private set; }
    public const string Localization = $"Mods.{nameof(ZombieApocalypse)}";
    public const string VisionShader = $"{nameof(ZombieApocalypse)}/ZombieVision";

    public override void Load() {
        Mod = this;
        if (!Main.dedServ)
            Filters.Scene[VisionShader] =
                new(new(Assets.Request<Effect>("Content/ZombieVision"), "ScreenPass"), EffectPriority.Medium);
    }

    public override object Call(params object[] args) {
        string msgType = (string)args[0];
        switch (msgType) {
            case "Zombify":
                ((Player)args[1]).SetZombie((bool)args[2]);
                break;
            case "IsZombie":
                return ((Player)args[1]).IsZombie();
            default:
                break;
        }
        return null;
    }

    internal enum MessageType : byte {
        Zombify
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) {
        MessageType msgType = (MessageType)reader.ReadByte();

        switch (msgType) {
            case MessageType.Zombify:
                byte p = reader.ReadByte();
                ZombifiablePlayer zomb = Main.player[p].GetModPlayer<ZombifiablePlayer>();
                bool fromInfection = reader.ReadBoolean();
                bool quiet = reader.ReadBoolean();
                zomb.ReceivePlayerSync(reader, fromInfection);
                zomb.quiet = false; // i love hacky flags

                if (Main.netMode == NetmodeID.Server)
                    zomb.SyncPlayer(-1, whoAmI, false);
                break;
            default:
                Logger.WarnFormat("Unknown network message type: {0}", msgType);
                break;
        }
    }
}

public class ZombieApocalypseConfig : ModConfig { // so many bools...
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [DefaultValue(false)]
    public bool OnlyTransformPlayerIfKilledByZombie { get; set; }

    [DefaultValue(false)]
    public bool ZombiesCanTradeWithNPCs { get; set; }

    [DefaultValue(true)]
    [ReloadRequired]
    public bool ZombiesCanFightOtherPlayersWithoutPvP { get; set; }

    [DefaultValue(true)]
    public bool ZombiesCanAttackTownNPCs { get; set; }

    [DefaultValue(true)]
    [ReloadRequired]
    public bool ZombiesHaveADifferentSkinColor { get; set; }

    [ReloadRequired]
    public Color ZombieSkinColor { get; set; } = new(219, 214, 138);

    [DefaultValue(true)]
    public bool HostileNPCsAreMostlyFriendlyToZombies { get; set; }

    [DefaultValue(true)]
    public bool ZombiesAreFriendlyToZombies { get; set; }

    [DefaultValue(false)]
    public bool ZombiesAreImmuneToHostileProjectiles { get; set; }

    [DefaultValue(false)]
    public bool UnzombifyPlayersOnDeath { get; set; }

    [DefaultValue(true)]
    public bool DropUnusableItemsOnZombification { get; set; }

    [DefaultValue(true)]
    public bool BroadcastZombificationText { get; set; }

    [DefaultValue(false)]
    public bool BroadcastUnzombificationText { get; set; }

    [DefaultValue(1)]
    [Range(1, 255)]
    public int InitialInfectionPlayers { get; set; }

    [DefaultValue(false)]
    public bool BroadcastInfectionPlayers { get; set; }

    [DefaultValue(false)]
    public bool ZombiesCanUseAmmoAndCoinSlots { get; set; }

    [DefaultValue(true)] // too lazy to make these actually configurable with integers n such, deal with booleans
    public bool ZombiesHaveSmallerInventories { get; set; }

    [DefaultValue(true)]
    public bool ZombiesHaveLessAccessories { get; set; }

    [DefaultValue(true)]
    public bool AllowZombiesToUseVoidbags { get; set; }

    [DefaultValue(true)]
    public bool ZombiesGetWeaknessWhenExposedToSun { get; set; }

    [DefaultValue(true)]
    public bool EnableHolyCure { get; set; }

    [DefaultValue(true)]
    public bool EnableSuspiciousLookingFlask { get; set; }

    [DefaultValue(true)]
    public bool EnableBodyFlux { get; set; }

    [DefaultValue(true)]
    [ReloadRequired]
    public bool IncreaseZombieArmDropChance { get; set; }

    [DefaultValue(true)]
    public bool MoreZomb { get; set; }

    [DefaultValue(true)]
    [ReloadRequired]
    public bool StrongerZomb { get; set; }

    [DefaultValue(true)]
    public bool EvenMoreZomb { get; set; }

    [DefaultValue(true)]
    public bool ApplyCustomVisionShaderToZombies { get; set; }

    [DefaultValue(true)]
    public bool ZombifyPlayersOnRespawn { get; set; }

    [DefaultValue(true)] // priority chaos
    public bool RespawnNewZombiesWhereTheyDied { get; set; }

    [DefaultValue(false)]
    public bool AlwaysRespawnZombiesWhereTheyDied { get; set; }

    [DefaultValue(true)]
    public bool ZombifiedPlayersSpawnAtOceans { get; set; }

    [DefaultValue(true)]
    public bool ZombificationParticles { get; set; }

    [DefaultValue(true)]
    public bool ZombiesWinWhenEveryoneIsZombie { get; set; }

    [DefaultValue(false)]
    public bool DisableZombification { get; set; }

    public static ZombieApocalypseConfig GetInstance() => ModContent.GetInstance<ZombieApocalypseConfig>();

    public static ZombieApocalypseConfig GetInstance(out ZombieApocalypseConfig config) {
        config = GetInstance();
        return config;
    }
}
