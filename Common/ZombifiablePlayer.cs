﻿using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse.Common;

public class ZombifiablePlayer : ModPlayer {
    public bool Zombified { get; set; } = false;
    public Color OriginalSkinColor { get; set; }

    public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot) => !ZombieApocalypseConfig.GetInstance().HostileNPCsAreMostlyFriendlyToZombies || !Zombified || npc.target == Player.whoAmI;

    public override bool CanBeHitByProjectile(Projectile proj) => !ZombieApocalypseConfig.GetInstance().HostileNPCsAreMostlyFriendlyToZombies || !Zombified || (proj.TryGetOwner(out Player p) && p.InOpposingTeam(Player));

    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource) {
        if (!Zombified && Player.whoAmI == Main.myPlayer && (!ZombieApocalypseConfig.GetInstance().OnlyTransformPlayerIfKilledByZombie || (damageSource.TryGetCausingNPC(out NPC npc) && NPCID.Sets.Zombies[npc.type]) || (damageSource.TryGetCausingEntity(out Entity entity) && entity is Player player && player.IsZombie()))) {
            Player.statLife = Player.statLifeMax / 2;
            Player.SetZombie(true);
            ClientHandleZombification();
            return false;
        }
        return true;
    }

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
        if (Player.whoAmI == Main.myPlayer && ZombieApocalypseConfig.GetInstance().UnzombifyPlayersOnDeath && Player.IsZombie()) {
            Player.SetZombie(false);
            ClientHandleZombification();
        }
    }

    public override bool CanHitNPC(NPC target) =>  !ZombieApocalypseConfig.GetInstance().HostileNPCsAreMostlyFriendlyToZombies || !Zombified || (Zombified && target.friendly) || target.target == Player.whoAmI;

    public override void SaveData(TagCompound tag) {
        if (Zombified) {
            tag["Zombified"] = true;
            tag["SkinColor"] = OriginalSkinColor;
        }
    }

    public override void LoadData(TagCompound tag) {
        Zombified = tag.TryGet("Zombified", out bool zombified) && zombified;
        OriginalSkinColor = tag.TryGet("SkinColor", out Color color) ? color : Player.skinColor;
        if (zombified && ZombieApocalypseConfig.GetInstance(out var cfg).ZombiesHaveADifferentSkinColor)
            Player.skinColor = cfg.ZombieSkinColor;
    }

    public void ClientHandleZombification(bool fromInfection = false) {
        if ((ZombieApocalypseConfig.GetInstance(out var cfg).BroadcastZombificationText && Zombified) || cfg.BroadcastUnzombificationText)
            Main.NewText(Language.GetTextValue($"{ZombieApocalypse.Localization}.Player{(Zombified ? "Zombified" : "Unzombified")}", Player.name), 50, 255, 130);
        if (Main.netMode == NetmodeID.MultiplayerClient)
            SendZombificationStatusChange(Player.whoAmI, Zombified, fromInfection);
    }

    // aggro modifications
    public override void PreUpdate() {
        if (!Zombified && ZombieApocalypseConfig.GetInstance().HostileNPCsAreMostlyFriendlyToZombies)
            Player.aggro = 0;
    }

    public override void PostUpdate() { // huh, terraria doesn't like negative 100k aggro apparently
        if (Zombified && ZombieApocalypseConfig.GetInstance().HostileNPCsAreMostlyFriendlyToZombies)
            Player.aggro = -100000;
    }

    public override bool CanBuyItem(NPC vendor, Item[] shopInventory, Item item) => ZombieApocalypseConfig.GetInstance().ZombiesCanTradeWithNPCs;

    /*public override void CopyClientState(ModPlayer targetCopy) {
        if (targetCopy is ZombifiablePlayer zombi) {
            zombi.Zombified = Zombified;
            zombi.OriginalSkinColor = OriginalSkinColor;
        }
    }

    public override void SendClientChanges(ModPlayer clientPlayer) {
        
    }*/

    public static void HandleZombification(BinaryReader reader, int whoAmI) {
        int player = reader.ReadByte();
        if (Main.netMode == NetmodeID.Server)
            player = whoAmI;

        bool isZombie = reader.ReadBoolean();
        bool fromInfection = reader.ReadBoolean();
        if (player != Main.myPlayer)
            Main.player[player].SetZombie(isZombie);

        // if the server receives this message, send it to all other clients to sync the effects
        if (Main.netMode == NetmodeID.Server)
            SendZombificationStatusChange(player, isZombie, fromInfection);
        else if (isZombie && (ZombieApocalypseConfig.GetInstance(out var cfg).BroadcastZombificationText || fromInfection && cfg.BroadcastInfectionPlayers))
            if (fromInfection) // if logic go brrr
                Main.NewText(Language.GetTextValue($"{ZombieApocalypse.Localization}.PlayerZombifiedInfection", Main.player[player].name), 50, 255, 130);
            else // no braces round these parts officer, no siree
                Main.NewText(Language.GetTextValue($"{ZombieApocalypse.Localization}.PlayerZombified", Main.player[player].name), 50, 255, 130);
        else if (!isZombie && ZombieApocalypseConfig.GetInstance().BroadcastUnzombificationText)
            Main.NewText(Language.GetTextValue($"{ZombieApocalypse.Localization}.PlayerUnzombified", Main.player[player].name), 50, 255, 130);
    }

    public static void SendZombificationStatusChange(int whoAmI, bool newZombified, bool fromInfection = false, bool ignoreClient = true) {
        ModPacket packet = ModContent.GetInstance<ZombieApocalypse>().GetPacket();
        packet.Write((byte)ZombieApocalypse.MessageType.Zombify);
        packet.Write((byte)whoAmI);
        packet.Write(newZombified);
        packet.Write(fromInfection);
        packet.Send(ignoreClient: ignoreClient ? whoAmI : -1);
    }
}
