using Microsoft.Xna.Framework;
using System.ComponentModel;
using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using ZombieApocalypse.Common;

namespace ZombieApocalypse {
	public class ZombieApocalypse : Mod {
		public static Mod Mod { get; private set; }
        public const string Localization = $"Mods.{nameof(ZombieApocalypse)}";

        public override void Load() {
            Mod = this;
        }

        public override object Call(params object[] args) {
            string msgType = (string)args[0];
            switch (msgType) {
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
                    ZombifiablePlayer.HandleZombification(reader, whoAmI);
                    break;
                default:
                    Logger.WarnFormat("Unknown network message type: {0}", msgType);
                    break;
            }
        }
    }

    public class ZombieApocalypseConfig : ModConfig {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(false)]
        public bool OnlyTransformPlayerIfKilledByZombie { get; set; }

        [DefaultValue(false)]
        public bool ZombiesCanTradeWithNPCs { get; set; }

        [DefaultValue(true)]
        public bool ZombiesHaveADifferentSkinColor { get; set; }

        [DefaultValue(true)]
        public bool ZombiesCanFightOtherPlayersWithoutPvP { get; set; }

        [ReloadRequired]
        public Color ZombieSkinColor { get; set; } = new(219, 214, 138);

        [DefaultValue(3f)]
        [Range(0.1f, 20f)]
        public float ZombieSpawnChanceMult { get; set; }

        [DefaultValue(true)]
        public bool HostileNPCsAreMostlyFriendlyToZombies { get; set; }

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
        [ReloadRequired]
        public bool ZombiesCanUseAmmoAndCoinSlots { get; set; }

        [DefaultValue(true)]
        public bool AllowZombiesToUseVoidbags { get; set; }

        [DefaultValue(true)]
        public bool ZombiesGetWeaknessWhenExposedToSun { get; set; }

        public static ZombieApocalypseConfig GetInstance() => ModContent.GetInstance<ZombieApocalypseConfig>();

        public static ZombieApocalypseConfig GetInstance(out ZombieApocalypseConfig config) {
            config = GetInstance();
            return config;
        }
    }
}
