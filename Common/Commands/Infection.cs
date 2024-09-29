using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse.Common.Commands;

public class Infection : ModCommand {
    public override CommandType Type => CommandType.World;

    public override string Command => "infection";
    public override string Description => "Starts a round of infection";

    public override void Action(CommandCaller caller, string input, string[] args) {
        if (Main.netMode == NetmodeID.Server) {
            Player[] players = Main.player.Where(p => p.active).ToArray();
            for (int i = 0; i < Math.Min(players.Length, ZombieApocalypseConfig.GetInstance().InitialInfectionPlayers); i++) {
                int infected = Main.rand.Next(players.Length);
                players[infected].SetZombie(true);
                ZombifiablePlayer.SendZombificationStatusChange(players[infected].whoAmI, true, true, false);
            }
        }
    }
}