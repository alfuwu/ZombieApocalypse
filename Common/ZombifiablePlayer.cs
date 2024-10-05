﻿using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse.Common;

public class ZombifiablePlayer : ModPlayer {
    public bool Zombified { get; set; } = false;
    public Color OriginalSkinColor { get; set; } // hacky skin color solution
    public PlayerDeathReason LastDeathReason { get; set; }
    private bool aggroModified = false;

    public static bool ExposedToSky(Player player, bool affectedByWind = false) {
        Point playerTileCoordinate = player.Center.ToTileCoordinates();
        Vector2 headPosition = player.position - new Vector2(0f, player.height);

        bool canHitSky = Collision.CanHit(new Vector2(headPosition.X + player.width / 2, headPosition.Y), 1, 1, new Vector2(Main.screenPosition.X + (Main.screenWidth / 2) + (affectedByWind ? -Main.screenWidth * Main.windSpeedCurrent * (MathHelper.Pi / 10) : 0), Main.screenPosition.Y), 1, 1);

        return !(playerTileCoordinate.Y <= 50 && playerTileCoordinate.Y > Main.rockLayer) && canHitSky;
    }

    public static void ZombificationDusts(Player player) {
        for (int i = 0; i < 500 + Main.rand.Next(100); i++) {
            float ang = Main.rand.NextFloat(0, MathHelper.TwoPi);
            Vector2 dustPosition = player.Center + new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang)) * Main.rand.NextFloat(0, 50);
            Dust dust = Dust.NewDustDirect(dustPosition, 0, 0, DustID.Smoke, Main.rand.NextFloat() - 0.5f, Main.rand.NextFloat() - 0.5f, 150, new(1f, 0.2f, 0.3f), 1.5f);

            dust.velocity *= 3f;
            dust.noGravity = true;
            dust.fadeIn = 1.3f;
        }
        SoundEngine.PlaySound(SoundID.NPCHit14, player.Center); // too lazy to separate this into two config options
    }

    public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot) => !ZombieApocalypseConfig.GetInstance().HostileNPCsAreMostlyFriendlyToZombies || !Zombified || npc.target == Player.whoAmI;

    public override bool CanBeHitByProjectile(Projectile proj) => !ZombieApocalypseConfig.GetInstance().ZombiesAreImmuneToHostileProjectiles || !Zombified || (proj.TryGetOwner(out Player p) && p.InOpposingTeam(Player));

    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource) {
        if (!Zombified && Player.IsZombifiableDeath(damageSource)) {
            if (ZombieApocalypseConfig.GetInstance().ZombifyPlayersOnRespawn) {
                LastDeathReason = damageSource;
            } else {
                Player.statLife = Player.statLifeMax / 2;
                Player.SetZombie(true);
                ClientHandleZombification();
                return false;
            }
        }
        return true;
    }

    public override void OnRespawn() {
        if (Player.whoAmI == Main.myPlayer) {
            if (ZombieApocalypseConfig.GetInstance(out var cfg).UnzombifyPlayersOnDeath && Player.IsZombie()) {
                Player.SetZombie(false);
                ClientHandleZombification();
            }
        }
    }

    public override bool CanHitNPC(NPC target) => !ZombieApocalypseConfig.GetInstance().HostileNPCsAreMostlyFriendlyToZombies || !Zombified || (Zombified && target.friendly) || target.target == Player.whoAmI;

    public override void SaveData(TagCompound tag) {
        tag["SkinColor"] = OriginalSkinColor;
        if (Zombified)
            tag["Zombified"] = true;
    }

    public override void LoadData(TagCompound tag) {
        Zombified = tag.TryGet("Zombified", out bool zombified) && zombified;
        OriginalSkinColor = tag.TryGet("SkinColor", out Color color) ? color : Player.skinColor;
        if (ZombieApocalypseConfig.GetInstance(out var cfg).ZombiesHaveADifferentSkinColor && Zombified)
            Player.skinColor = cfg.ZombieSkinColor;
    }

    public void ClientHandleZombification(bool fromInfection = false) {
        if (Zombified)
            OriginalSkinColor = Player.skinColor;
        if ((ZombieApocalypseConfig.GetInstance(out var cfg).BroadcastZombificationText && Zombified) || cfg.BroadcastUnzombificationText)
            Main.NewText(Language.GetTextValue($"{ZombieApocalypse.Localization}.Player{(Zombified ?  "Zombified" : "Unzombified")}{(fromInfection ? "Infection" : "")}", Player.name), 50, 255, 130);
        if (Main.netMode == NetmodeID.MultiplayerClient)
            SendZombificationStatusChange(Player.whoAmI, Zombified, fromInfection);
        if (Zombified && cfg.ZombificationParticles)
            ZombificationDusts(Player);
    }

    public override void OnEnterWorld() {
        Filters.Scene[ZombieApocalypse.VisionShader].GetShader().UseIntensity(-1);
        Filters.Scene.Activate(ZombieApocalypse.VisionShader);
    }

    // aggro modifications
    public override void PreUpdate() {
        if (Zombified && ZombieApocalypseConfig.GetInstance().HostileNPCsAreMostlyFriendlyToZombies && Player.aggro > -1000) {
            Player.aggro = -1000;
            aggroModified = true;
        }
    }

    public override void PostUpdate() {
        if (!Zombified && aggroModified) {
            Player.aggro = 0;
            aggroModified = false;
        } else if (Zombified && Player.whoAmI == Main.myPlayer) {
            if (ZombieApocalypseConfig.GetInstance(out var cfg).HostileNPCsAreMostlyFriendlyToZombies) {
                Player.aggro = -1000;
                aggroModified = true;
            }
            if (cfg.ZombiesGetWeaknessWhenExposedToSun && Main.IsItDay() && ExposedToSky(Player) && !Player.buffImmune[BuffID.Weak])
                if (Player.HasBuff(BuffID.Weak) && Player.buffTime[Player.FindBuffIndex(BuffID.Weak)] <= 301)
                    Player.buffTime[Player.FindBuffIndex(BuffID.Weak)] = 302;
                else if (!Player.HasBuff(BuffID.Weak))
                    Player.AddBuff(BuffID.Weak, 302);
            if (Main.netMode != NetmodeID.Server && cfg.ApplyCustomVisionShaderToZombies && Filters.Scene[ZombieApocalypse.VisionShader].GetShader().Intensity < 0)
                Filters.Scene[ZombieApocalypse.VisionShader].GetShader().UseIntensity(0);
            else if (Main.netMode != NetmodeID.Server && Filters.Scene[ZombieApocalypse.VisionShader].GetShader().Intensity >= 0)
                Filters.Scene[ZombieApocalypse.VisionShader].GetShader().UseIntensity(-1);
        } else if (Main.netMode != NetmodeID.Server && Filters.Scene[ZombieApocalypse.VisionShader].GetShader().Intensity >= 0 && Player.whoAmI == Main.myPlayer) {
            Filters.Scene[ZombieApocalypse.VisionShader].GetShader().UseIntensity(-1);
        }
    }

    public override bool? CanHitNPCWithItem(Item item, NPC target) => target.friendly ? Player.IsZombie() && ZombieApocalypseConfig.GetInstance().ZombiesCanAttackTownNPCs : null;

    public override bool CanSellItem(NPC vendor, Item[] shopInventory, Item item) => ZombieApocalypseConfig.GetInstance().ZombiesCanTradeWithNPCs;

    public override bool CanBuyItem(NPC vendor, Item[] shopInventory, Item item) => ZombieApocalypseConfig.GetInstance().ZombiesCanTradeWithNPCs;

    public override void CopyClientState(ModPlayer targetCopy) {
        if (targetCopy is ZombifiablePlayer zombi) {
            zombi.Zombified = Zombified;
            zombi.OriginalSkinColor = OriginalSkinColor;
        }
    }

    public override void SendClientChanges(ModPlayer clientPlayer) {
        if (clientPlayer is ZombifiablePlayer zombi) {
            zombi.Zombified = Zombified;
            zombi.OriginalSkinColor = OriginalSkinColor;
        }
    }

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
        else if ((ZombieApocalypseConfig.GetInstance(out var cfg).BroadcastZombificationText || fromInfection && cfg.BroadcastInfectionPlayers) && isZombie)
            if (fromInfection) // if logic go brrr
                Main.NewText(Language.GetTextValue($"{ZombieApocalypse.Localization}.PlayerZombifiedInfection", Main.player[player].name), 50, 255, 130);
            else // no braces round these parts officer, no siree
                Main.NewText(Language.GetTextValue($"{ZombieApocalypse.Localization}.PlayerZombified", Main.player[player].name), 50, 255, 130);
        else if (cfg.BroadcastUnzombificationText && !isZombie)
            Main.NewText(Language.GetTextValue($"{ZombieApocalypse.Localization}.PlayerUnzombified", Main.player[player].name), 50, 255, 130);

        if (isZombie && ZombieApocalypseConfig.GetInstance().ZombificationParticles)
            ZombificationDusts(Main.player[player]);
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
