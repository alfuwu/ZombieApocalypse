using System;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace ZombieApocalypse.Common.Extensions;

public static class ModNPCExtensions {
    // stripped down Fighter AI code
    public static void BasicFighterAI(this ModNPC modNPC, bool runAwayInDaylight = true) {
        if (Main.player[modNPC.NPC.target].position.Y + Main.player[modNPC.NPC.target].height == modNPC.NPC.position.Y + modNPC.NPC.height)
            modNPC.NPC.directionY = -1;

        bool flag = false;

        bool flag5 = false;
        bool flag6 = false;
        if (modNPC.NPC.velocity.X == 0f)
            flag6 = true;

        if (modNPC.NPC.justHit)
            flag6 = false;

        int num56 = 60;

        bool flag7 = false;
        bool flag8 = true;

        int num64 = modNPC.NPC.type;

        if (modNPC.NPC.velocity.Y == 0f && ((modNPC.NPC.velocity.X > 0f && modNPC.NPC.direction < 0) || (modNPC.NPC.velocity.X < 0f && modNPC.NPC.direction > 0)))
            flag7 = true;

        if (modNPC.NPC.position.X == modNPC.NPC.oldPosition.X || modNPC.NPC.ai[3] >= num56 || flag7)
            modNPC.NPC.ai[3] += 1f;
        else if (Math.Abs(modNPC.NPC.velocity.X) > 0.9f && modNPC.NPC.ai[3] > 0f)
            modNPC.NPC.ai[3] -= 1f;

        if (modNPC.NPC.ai[3] > num56 * 10)
            modNPC.NPC.ai[3] = 0f;

        if (modNPC.NPC.justHit)
            modNPC.NPC.ai[3] = 0f;

        if (modNPC.NPC.ai[3] == num56)
            modNPC.NPC.netUpdate = true;

        if (Main.player[modNPC.NPC.target].Hitbox.Intersects(modNPC.NPC.Hitbox))
            modNPC.NPC.ai[3] = 0f;


        if (modNPC.NPC.ai[3] < num56 && (NPC.DespawnEncouragement_AIStyle3_Fighters_NotDiscouraged(modNPC.NPC.type, modNPC.NPC.position, modNPC.NPC) || !runAwayInDaylight)) {
            if (modNPC.NPC.shimmerTransparency < 1f)
                SoundEngine.PlaySound(SoundID.NPCHit14, new(modNPC.NPC.position.X, modNPC.NPC.position.Y));

            modNPC.NPC.TargetClosest();
            if (modNPC.NPC.directionY > 0 && Main.player[modNPC.NPC.target].Center.Y <= modNPC.NPC.Bottom.Y)
                modNPC.NPC.directionY = -1;
        } else if (!(modNPC.NPC.ai[2] > 0f) || !NPC.DespawnEncouragement_AIStyle3_Fighters_CanBeBusyWithAction(modNPC.NPC.type)) {
            if (Main.IsItDay() && (double)(modNPC.NPC.position.Y / 16f) < Main.worldSurface)
                modNPC.NPC.EncourageDespawn(10);

            if (modNPC.NPC.velocity.X == 0f) {
                if (modNPC.NPC.velocity.Y == 0f) {
                    modNPC.NPC.ai[0] += 1f;
                    if (modNPC.NPC.ai[0] >= 2f) {
                        modNPC.NPC.direction *= -1;
                        modNPC.NPC.spriteDirection = modNPC.NPC.direction;
                        modNPC.NPC.ai[0] = 0f;
                    }
                }
            } else {
                modNPC.NPC.ai[0] = 0f;
            }

            if (modNPC.NPC.direction == 0)
                modNPC.NPC.direction = 1;
        }
        float num104 = 1f + (1f - modNPC.NPC.scale);

        if (modNPC.NPC.velocity.X < 0f - num104 || modNPC.NPC.velocity.X > num104) {
            if (modNPC.NPC.velocity.Y == 0f)
                modNPC.NPC.velocity *= 0.8f;
        } else if (modNPC.NPC.velocity.X < num104 && modNPC.NPC.direction == 1) {
            modNPC.NPC.velocity.X += 0.07f;
            if (modNPC.NPC.velocity.X > num104)
                modNPC.NPC.velocity.X = num104;
        } else if (modNPC.NPC.velocity.X > 0f - num104 && modNPC.NPC.direction == -1) {
            modNPC.NPC.velocity.X -= 0.07f;
            if (modNPC.NPC.velocity.X < 0f - num104)
                modNPC.NPC.velocity.X = 0f - num104;
        }

        if (modNPC.NPC.velocity.Y == 0f || flag) {
            int num181 = (int)(modNPC.NPC.position.Y + modNPC.NPC.height + 7f) / 16;
            int num182 = (int)(modNPC.NPC.position.Y - 9f) / 16;
            int num183 = (int)modNPC.NPC.position.X / 16;
            int num184 = (int)(modNPC.NPC.position.X + modNPC.NPC.width) / 16;
            int num185 = (int)(modNPC.NPC.position.X + 8f) / 16;
            int num186 = (int)(modNPC.NPC.position.X + modNPC.NPC.width - 8f) / 16;
            bool flag22 = false;
            for (int num187 = num185; num187 <= num186; num187++) {
                if (num187 >= num183 && num187 <= num184 && Main.tile[num187, num181] == null) {
                    flag22 = true;
                    continue;
                }

                if (Main.tile[num187, num182] != null && !Main.tile[num187, num182].HasTile && Main.tileSolid[Main.tile[num187, num182].TileType]) {
                    flag5 = false;
                    break;
                }

                if (!flag22 && num187 >= num183 && num187 <= num184 && !Main.tile[num187, num181].HasTile && Main.tileSolid[Main.tile[num187, num181].TileType])
                    flag5 = true;
            }

            if (!flag5 && modNPC.NPC.velocity.Y < 0f)
                modNPC.NPC.velocity.Y = 0f;

            if (flag22)
                return;
        }

        if (modNPC.NPC.velocity.Y >= 0f) {
            int num188 = 0;
            if (modNPC.NPC.velocity.X < 0f)
                num188 = -1;

            if (modNPC.NPC.velocity.X > 0f)
                num188 = 1;

            Vector2 vector39 = modNPC.NPC.position;
            vector39.X += modNPC.NPC.velocity.X;
            int num189 = (int)((vector39.X + (modNPC.NPC.width / 2) + ((modNPC.NPC.width / 2 + 1) * num188)) / 16f);
            int num190 = (int)((vector39.Y + modNPC.NPC.height - 1f) / 16f);
            if (WorldGen.InWorld(num189, num190, 4)) {
                if ((num189 * 16) < vector39.X + modNPC.NPC.width && (num189 * 16 + 16) > vector39.X && ((Main.tile[num189, num190].HasTile && !Main.tile[num189, num190].TopSlope && !Main.tile[num189, num190 - 1].TopSlope && Main.tileSolid[Main.tile[num189, num190].TileType] && !Main.tileSolidTop[Main.tile[num189, num190].TileType]) || (Main.tile[num189, num190 - 1].IsHalfBlock && Main.tile[num189, num190 - 1].HasTile)) && (!Main.tile[num189, num190 - 1].HasTile || !Main.tileSolid[Main.tile[num189, num190 - 1].TileType] || Main.tileSolidTop[Main.tile[num189, num190 - 1].TileType] || (Main.tile[num189, num190 - 1].IsHalfBlock && (!Main.tile[num189, num190 - 4].HasTile || !Main.tileSolid[Main.tile[num189, num190 - 4].TileType] || Main.tileSolidTop[Main.tile[num189, num190 - 4].TileType]))) && (!Main.tile[num189, num190 - 2].HasTile || !Main.tileSolid[Main.tile[num189, num190 - 2].TileType] || Main.tileSolidTop[Main.tile[num189, num190 - 2].TileType]) && (!Main.tile[num189, num190 - 3].HasTile || !Main.tileSolid[Main.tile[num189, num190 - 3].TileType] || Main.tileSolidTop[Main.tile[num189, num190 - 3].TileType]) && (!Main.tile[num189 - num188, num190 - 3].HasTile || !Main.tileSolid[Main.tile[num189 - num188, num190 - 3].TileType])) {
                    float num191 = num190 * 16;
                    if (Main.tile[num189, num190].IsHalfBlock)
                        num191 += 8f;

                    if (Main.tile[num189, num190 - 1].IsHalfBlock)
                        num191 -= 8f;

                    if (num191 < vector39.Y + modNPC.NPC.height) {
                        float num192 = vector39.Y + modNPC.NPC.height - num191;
                        float num193 = 16.1f;
                        if (modNPC.NPC.type == 163 || modNPC.NPC.type == 164 || modNPC.NPC.type == 236 || modNPC.NPC.type == 239 || modNPC.NPC.type == 530)
                            num193 += 8f;

                        if (num192 <= num193) {
                            modNPC.NPC.gfxOffY += modNPC.NPC.position.Y + modNPC.NPC.height - num191;
                            modNPC.NPC.position.Y = num191 - modNPC.NPC.height;
                            if (num192 < 9f)
                                modNPC.NPC.stepSpeed = 1f;
                            else
                                modNPC.NPC.stepSpeed = 2f;
                        }
                    }
                }
            }
        }

        if (flag5) {
            int num194 = (int)((modNPC.NPC.position.X + (modNPC.NPC.width / 2) + (15 * modNPC.NPC.direction)) / 16f);
            int num195 = (int)((modNPC.NPC.position.Y + modNPC.NPC.height - 15f) / 16f);
            
            Tile tile = Main.tile[num194, num195 + 1];
            tile.IsHalfBlock = true;

            /*
			if (Main.tile[num194, num195 - 1].HasTile && (Main.tile[num194, num195 - 1].TileType == 10 || Main.tile[num194, num195 - 1].TileType == 388) && flag8) {
			*/
            if (Main.tile[num194, num195 - 1].HasTile && (TileLoader.IsClosedDoor(Main.tile[num194, num195 - 1]) || Main.tile[num194, num195 - 1].TileType == 388) && flag8) {
                modNPC.NPC.ai[2] += 1f;
                modNPC.NPC.ai[3] = 0f;
                if (modNPC.NPC.ai[2] >= 60f) {
                    bool flag24 = Main.player[modNPC.NPC.target].ZoneGraveyard && Main.rand.NextBool(60);
                    if ((!Main.bloodMoon || Main.getGoodWorld) && !flag24)
                        modNPC.NPC.ai[1] = 0f;

                    modNPC.NPC.velocity.X = 0.5f * (-modNPC.NPC.direction);
                    int num196 = 5;
                    if (Main.tile[num194, num195 - 1].TileType == 388)
                        num196 = 2;

                    modNPC.NPC.ai[1] += num196;
                    modNPC.NPC.ai[2] = 0f;
                    bool flag25 = false;
                    if (modNPC.NPC.ai[1] >= 10f) {
                        flag25 = true;
                        modNPC.NPC.ai[1] = 10f;
                    }

                    WorldGen.KillTile(num194, num195 - 1, fail: true);
                    // wjat is this if statement
                    if ((Main.netMode != NetmodeID.MultiplayerClient || !flag25) && flag25 && Main.netMode != NetmodeID.MultiplayerClient) {
                        if (TileLoader.IsClosedDoor(Main.tile[num194, num195 - 1])) {
                            bool flag26 = WorldGen.OpenDoor(num194, num195 - 1, modNPC.NPC.direction);
                            if (!flag26) {
                                modNPC.NPC.ai[3] = num56;
                                modNPC.NPC.netUpdate = true;
                            }

                            if (Main.netMode == NetmodeID.Server && flag26)
                                NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, num194, num195 - 1, modNPC.NPC.direction);
                        }

                        if (Main.tile[num194, num195 - 1].TileType == 388) {
                            bool flag27 = WorldGen.ShiftTallGate(num194, num195 - 1, closing: false);
                            if (!flag27) {
                                modNPC.NPC.ai[3] = num56;
                                modNPC.NPC.netUpdate = true;
                            }

                            if (Main.netMode == NetmodeID.Server && flag27)
                                NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 4, num194, num195 - 1);
                        }
                    }
                }
            } else {
                int num197 = modNPC.NPC.spriteDirection;
                if ((modNPC.NPC.velocity.X < 0f && num197 == -1) || (modNPC.NPC.velocity.X > 0f && num197 == 1)) {
                    if (modNPC.NPC.height >= 32 && Main.tile[num194, num195 - 2].HasTile && Main.tileSolid[Main.tile[num194, num195 - 2].TileType]) {
                        if (Main.tile[num194, num195 - 3].HasTile && Main.tileSolid[Main.tile[num194, num195 - 3].TileType]) {
                            modNPC.NPC.velocity.Y = -8f;
                            modNPC.NPC.netUpdate = true;
                        } else {
                            modNPC.NPC.velocity.Y = -7f;
                            modNPC.NPC.netUpdate = true;
                        }
                    } else if (Main.tile[num194, num195 - 1].HasTile && Main.tileSolid[Main.tile[num194, num195 - 1].TileType]) {
                        modNPC.NPC.velocity.Y = -6f;
                        modNPC.NPC.netUpdate = true;
                    } else if (modNPC.NPC.position.Y + modNPC.NPC.height - (num195 * 16) > 20f && Main.tile[num194, num195].HasTile && !Main.tile[num194, num195].TopSlope && Main.tileSolid[Main.tile[num194, num195].TileType]) {
                        modNPC.NPC.velocity.Y = -5f;
                        modNPC.NPC.netUpdate = true;
                    } else if (modNPC.NPC.directionY < 0 && modNPC.NPC.type != 67 && (!Main.tile[num194, num195 + 1].HasTile || !Main.tileSolid[Main.tile[num194, num195 + 1].TileType]) && (!Main.tile[num194 + modNPC.NPC.direction, num195 + 1].HasTile || !Main.tileSolid[Main.tile[num194 + modNPC.NPC.direction, num195 + 1].TileType])) {
                        modNPC.NPC.velocity.Y = -8f;
                        modNPC.NPC.velocity.X *= 1.5f;
                        modNPC.NPC.netUpdate = true;
                    } else if (flag8) {
                        modNPC.NPC.ai[1] = 0f;
                        modNPC.NPC.ai[2] = 0f;
                    }

                    if (modNPC.NPC.velocity.Y == 0f && flag6 && modNPC.NPC.ai[3] == 1f)
                        modNPC.NPC.velocity.Y = -5f;

                    if (modNPC.NPC.velocity.Y == 0f && Main.expertMode && Main.player[modNPC.NPC.target].Bottom.Y < modNPC.NPC.Top.Y && Math.Abs(modNPC.NPC.Center.X - Main.player[modNPC.NPC.target].Center.X) < (Main.player[modNPC.NPC.target].width * 3) && Collision.CanHit(modNPC.NPC, Main.player[modNPC.NPC.target])) {
                        if (modNPC.NPC.velocity.Y == 0f) {
                            int num200 = 6;
                            if (Main.player[modNPC.NPC.target].Bottom.Y > modNPC.NPC.Top.Y - (num200 * 16)) {
                                modNPC.NPC.velocity.Y = -7.9f;
                            } else {
                                int num201 = (int)(modNPC.NPC.Center.X / 16f);
                                int num202 = (int)(modNPC.NPC.Bottom.Y / 16f) - 1;
                                for (int num203 = num202; num203 > num202 - num200; num203--) {
                                    if (Main.tile[num201, num203].HasTile && TileID.Sets.Platforms[Main.tile[num201, num203].TileType]) {
                                        modNPC.NPC.velocity.Y = -7.9f;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        } else if (flag8) {
            modNPC.NPC.ai[1] = 0f;
            modNPC.NPC.ai[2] = 0f;
        }

        if (Main.netMode != 1 && modNPC.NPC.type == 120 && modNPC.NPC.ai[3] >= num56) {
            int targetTileX = (int)Main.player[modNPC.NPC.target].Center.X / 16;
            int targetTileY = (int)Main.player[modNPC.NPC.target].Center.Y / 16;
            Vector2 chosenTile = Vector2.Zero;
            if (modNPC.NPC.AI_AttemptToFindTeleportSpot(ref chosenTile, targetTileX, targetTileY, 20, 9)) {
                modNPC.NPC.position.X = chosenTile.X * 16f - (modNPC.NPC.width / 2);
                modNPC.NPC.position.Y = chosenTile.Y * 16f - modNPC.NPC.height;
                modNPC.NPC.ai[3] = -120f;
                modNPC.NPC.netUpdate = true;
            }
        }
    }
}
