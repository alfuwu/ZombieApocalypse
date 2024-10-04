using System;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ZombieApocalypse.Common.Extensions;
using Terraria.GameContent.Bestiary;

namespace ZombieApocalypse.Content.ExtraZombs;

public class ZombifiedMummy : ModNPC {
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Mummy];
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifier = new() {
            Velocity = 0.5f
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        NPCID.Sets.Zombies[Type] = true;
        NPCID.Sets.ShimmerTransformToNPC[Type] = NPCID.Mummy;
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.Mummy);
        NPC.damage = 50;
        NPC.defense = 16;
        NPC.lifeMax = 130;
        NPC.value = 600f;
        NPC.knockBackResist = 0.5f;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,

            new FlavorTextBestiaryInfoElement($"{ZombieApocalypse.Localization}.Bestiary.ZombifiedMummy")
        ]);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode != NetmodeID.Server) {
            if (NPC.life > 0) {
                for (int num730 = 0; num730 < hit.Damage / (double)NPC.lifeMax * 50.0; num730++) {
                    int num731 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 31, 0f, 0f, 0, default, 1.5f);
                    Dust dust = Main.dust[num731];
                    dust.velocity *= 2f;
                    Main.dust[num731].noGravity = true;
                }

                return;
            }

            for (int num732 = 0; num732 < 20; num732++) {
                int num733 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 31, 0f, 0f, 0, default, 1.5f);
                Dust dust = Main.dust[num733];
                dust.velocity *= 2f;
                Main.dust[num733].noGravity = true;
            }

            int num734 = Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.position.X, NPC.position.Y - 10f), new Vector2(hit.HitDirection, 0f), 61, NPC.scale);
            Gore gore2 = Main.gore[num734];
            gore2.velocity *= 0.3f;
            num734 = Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.position.X, NPC.position.Y + (NPC.height / 2) - 10f), new Vector2(hit.HitDirection, 0f), 62, NPC.scale);
            gore2 = Main.gore[num734];
            gore2.velocity *= 0.3f;
            num734 = Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.position.X, NPC.position.Y + NPC.height - 10f), new Vector2(hit.HitDirection, 0f), 63, NPC.scale);
            gore2 = Main.gore[num734];
            gore2.velocity *= 0.3f;
        }
    }

    public override void AI() {
        this.BasicFighterAI();
    }

    public override void FindFrame(int frameHeight) {
        if (NPC.velocity.Y == 0f) {
            if (NPC.direction == 1)
                NPC.spriteDirection = 1;
            else if (NPC.direction == -1)
                NPC.spriteDirection = -1;

            int num237 = Main.npcFrameCount[Type] - NPCID.Sets.AttackFrameCount[Type];
            if (NPC.ai[0] == 23f) {
                NPC.frameCounter += 1.0;
                int num238 = NPC.frame.Y / frameHeight;
                int num17 = num237 - num238;
                if ((uint)(num17 - 1) > 1 && (uint)(num17 - 4) > 1 && num238 != 0) {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0.0;
                }

                int num239 = NPC.frameCounter >= 6.0 ? num237 - 4 : num237 - 5;
                if (NPC.ai[1] < 6f)
                    num239 = num237 - 5;

                NPC.frame.Y = frameHeight * num239;
            } else if (NPC.ai[0] >= 20f && NPC.ai[0] <= 22f) {
                int num240 = NPC.frame.Y / frameHeight;
                NPC.frame.Y = num240 * frameHeight;
            } else if (NPC.ai[0] == 2f) {
                NPC.frameCounter += 1.0;
                if (NPC.frame.Y / frameHeight == num237 - 1 && NPC.frameCounter >= 5.0) {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0.0;
                } else if (NPC.frame.Y / frameHeight == 0 && NPC.frameCounter >= 40.0) {
                    NPC.frame.Y = frameHeight * (num237 - 1);
                    NPC.frameCounter = 0.0;
                } else if (NPC.frame.Y != 0 && NPC.frame.Y != frameHeight * (num237 - 1)) {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0.0;
                }
            } else if (NPC.ai[0] == 11f) {
                NPC.frameCounter += 1.0;
                if (NPC.frame.Y / frameHeight == num237 - 1 && NPC.frameCounter >= 50.0) {
                    if (NPC.frameCounter == 50.0) {
                        int num242 = Main.rand.Next(4);
                        for (int m = 0; m < 3 + num242; m++) {
                            int num243 = Dust.NewDust(NPC.Center + Vector2.UnitX * -NPC.direction * 8f - Vector2.One * 5f + Vector2.UnitY * 8f, 3, 6, 216, -NPC.direction, 1f);
                            Dust dust = Main.dust[num243];
                            dust.velocity /= 2f;
                            Main.dust[num243].scale = 0.8f;
                        }

                        if (Main.rand.NextBool(30)) {
                            int num244 = Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitX * -NPC.direction * 8f, Vector2.Zero, Main.rand.Next(580, 583));
                            Gore gore = Main.gore[num244];
                            gore.velocity /= 2f;
                            Main.gore[num244].velocity.Y = Math.Abs(Main.gore[num244].velocity.Y);
                            Main.gore[num244].velocity.X = (0f - Math.Abs(Main.gore[num244].velocity.X)) * NPC.direction;
                        }
                    }

                    if (NPC.frameCounter >= 100.0 && Main.rand.NextBool(20)) {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0.0;
                    }
                } else if (NPC.frame.Y / frameHeight == 0 && NPC.frameCounter >= 20.0) {
                    NPC.frame.Y = frameHeight * (num237 - 1);
                    NPC.frameCounter = 0.0;
                    EmoteBubble.NewBubble(89, new(NPC), 90);
                } else if (NPC.frame.Y != 0 && NPC.frame.Y != frameHeight * (num237 - 1)) {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0.0;
                }
            } else if (NPC.ai[0] == 5f) {
                NPC.frame.Y = frameHeight * (num237 - 3);
                if (Type == 637)
                    NPC.frame.Y = frameHeight * 19;

                NPC.frameCounter = 0.0;
            } else if (NPC.ai[0] == 6f) {
                NPC.frameCounter += 1.0;
                int num245 = NPC.frame.Y / frameHeight;
                int num17 = num237 - num245;
                if ((uint)(num17 - 1) > 1u && (uint)(num17 - 4) > 1u && num245 != 0) {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0.0;
                }

                int num246 = NPC.frameCounter >= 10.0 ? NPC.frameCounter < 16.0 ? num237 - 5 : NPC.frameCounter < 46.0 ? num237 - 4 : NPC.frameCounter < 60.0 ? num237 - 5 : NPC.frameCounter >= 66.0 ? NPC.frameCounter < 72.0 ? num237 - 5 : NPC.frameCounter < 102.0 ? num237 - 4 : NPC.frameCounter < 108.0 ? num237 - 5 : NPC.frameCounter >= 114.0 ? NPC.frameCounter < 120.0 ? num237 - 5 : NPC.frameCounter < 150.0 ? num237 - 4 : NPC.frameCounter < 156.0 ? num237 - 5 : NPC.frameCounter >= 162.0 ? NPC.frameCounter < 168.0 ? num237 - 5 : NPC.frameCounter < 198.0 ? num237 - 4 : NPC.frameCounter < 204.0 ? num237 - 5 : NPC.frameCounter >= 210.0 ? NPC.frameCounter < 216.0 ? num237 - 5 : NPC.frameCounter < 246.0 ? num237 - 4 : NPC.frameCounter < 252.0 ? num237 - 5 : NPC.frameCounter >= 258.0 ? NPC.frameCounter < 264.0 ? num237 - 5 : NPC.frameCounter < 294.0 ? num237 - 4 : NPC.frameCounter < 300.0 ? num237 - 5 : 0 : 0 : 0 : 0 : 0 : 0 : 0;
                if (num246 == num237 - 4 && num245 == num237 - 5) {
                    Vector2 vector4 = NPC.Center + new Vector2(10 * NPC.direction, -4f);
                    for (int n = 0; n < 8; n++) {
                        int num247 = Main.rand.Next(139, 143);
                        int num248 = Dust.NewDust(vector4, 0, 0, num247, NPC.velocity.X + NPC.direction, NPC.velocity.Y - 2.5f, 0, default, 1.2f);
                        Main.dust[num248].velocity.X += NPC.direction * 1.5f;
                        Dust dust = Main.dust[num248];
                        dust.position -= new Vector2(4f);
                        dust = Main.dust[num248];
                        dust.velocity *= 2f;
                        Main.dust[num248].scale = 0.7f + Main.rand.NextFloat() * 0.3f;
                    }
                }

                NPC.frame.Y = frameHeight * num246;
                if (NPC.frameCounter >= 300.0)
                    NPC.frameCounter = 0.0;
            } else if ((NPC.ai[0] == 7f || NPC.ai[0] == 19f)) {
                NPC.frameCounter += 1.0;
                int num249 = NPC.frame.Y / frameHeight;
                int num17 = num237 - num249;
                if ((uint)(num17 - 1) > 1u && (uint)(num17 - 4) > 1u && num249 != 0) {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0.0;
                }

                int num250 = 0;
                if (NPC.frameCounter < 16.0)
                    num250 = 0;
                else if (NPC.frameCounter == 16.0)
                    EmoteBubble.NewBubbleNPC(new(NPC), 112);
                else if (NPC.frameCounter < 128.0)
                    num250 = NPC.frameCounter % 16.0 < 8.0 ? num237 - 2 : 0;
                else if (NPC.frameCounter < 160.0)
                    num250 = 0;
                else if (NPC.frameCounter != 160.0)
                    num250 = NPC.frameCounter < 220.0 ? NPC.frameCounter % 12.0 < 6.0 ? num237 - 2 : 0 : 0;
                else
                    EmoteBubble.NewBubbleNPC(new(NPC), 60);

                NPC.frame.Y = frameHeight * num250;
                if (NPC.frameCounter >= 220.0)
                    NPC.frameCounter = 0.0;
            } else if (NPC.ai[0] == 9f) {
                NPC.frameCounter += 1.0;
                int num251 = NPC.frame.Y / frameHeight;
                int num17 = num237 - num251;
                if ((uint)(num17 - 1) > 1u && (uint)(num17 - 4) > 1u && num251 != 0) {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0.0;
                }

                int num252 = NPC.frameCounter >= 10.0 ? NPC.frameCounter >= 16.0 ? num237 - 4 : num237 - 5 : 0;
                if (NPC.ai[1] < 16f)
                    num252 = num237 - 5;

                if (NPC.ai[1] < 10f)
                    num252 = 0;

                NPC.frame.Y = frameHeight * num252;
            } else if (NPC.ai[0] == 18f) {
                NPC.frameCounter += 1.0;
                int num253 = NPC.frame.Y / frameHeight;
                int num17 = num237 - num253;
                if ((uint)(num17 - 1) > 1u && (uint)(num17 - 4) > 1u && num253 != 0) {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0.0;
                }

                int num254 = Main.npcFrameCount[Type] - 2;
                NPC.frame.Y = frameHeight * num254;
            } else if (NPC.ai[0] == 10f || NPC.ai[0] == 13f) {
                NPC.frameCounter += 1.0;
                int num255 = NPC.frame.Y / frameHeight;
                int num17 = num255 - num237;
                if ((uint)num17 > 3u && num255 != 0) {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0.0;
                }

                int num256 = 10;
                int num257 = 6;
                if (Type == 633) {
                    num256 = 0;
                    num257 = 2;
                }

                int num258 = NPC.frameCounter >= num256 ? NPC.frameCounter < num256 + num257 ? num237 : NPC.frameCounter < num256 + num257 * 2 ? num237 + 1 : NPC.frameCounter < num256 + num257 * 3 ? num237 + 2 : NPC.frameCounter < num256 + num257 * 4 ? num237 + 3 : 0 : 0;
                NPC.frame.Y = frameHeight * num258;
            } else if (NPC.ai[0] == 15f) {
                NPC.frameCounter += 1.0;
                int num259 = NPC.frame.Y / frameHeight;
                int num17 = num259 - num237;
                if ((uint)num17 > 3u && num259 != 0) {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0.0;
                }

                float num260 = NPC.ai[1] / NPCID.Sets.AttackTime[Type];
                int num261 = num260 > 0.65f ? num237 : num260 > 0.5f ? num237 + 1 : num260 > 0.35f ? num237 + 2 : num260 > 0f ? num237 + 3 : 0;
                NPC.frame.Y = frameHeight * num261;
            } else if (NPC.ai[0] == 25f) {
                NPC.frame.Y = frameHeight;
            } else if (NPC.ai[0] == 12f) {
                NPC.frameCounter += 1.0;
                int num262 = NPC.frame.Y / frameHeight;
                int num17 = num262 - num237;
                if ((uint)num17 > 4u && num262 != 0) {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0.0;
                }

                int num263 = num237 + NPC.GetShootingFrame(NPC.ai[2]);
                NPC.frame.Y = frameHeight * num263;
            } else if (NPC.ai[0] == 14f || NPC.ai[0] == 24f) {
                NPC.frameCounter += 1.0;
                int num264 = NPC.frame.Y / frameHeight;
                int num17 = num264 - num237;
                if ((uint)num17 > 1u && num264 != 0) {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0.0;
                }

                int num265 = 12;
                int num266 = NPC.frameCounter % num265 * 2.0 < num265 ? num237 : num237 + 1;
                NPC.frame.Y = frameHeight * num266;
                if (NPC.ai[0] == 24f) {
                    if (NPC.frameCounter == 60.0)
                        EmoteBubble.NewBubble(87, new WorldUIAnchor(NPC), 60);
                    else if (NPC.frameCounter == 150.0)
                        EmoteBubble.NewBubble(3, new WorldUIAnchor(NPC), 90);
                    else if (NPC.frameCounter >= 240.0)
                        NPC.frame.Y = 0;
                }
            } else if (NPC.ai[0] == 1001f) {
                NPC.frame.Y = frameHeight * (num237 - 1);
                NPC.frameCounter = 0.0;
            } else if (NPC.CanTalk && (NPC.ai[0] == 3f || NPC.ai[0] == 4f)) {
                NPC.frameCounter += 1.0;
                int num267 = NPC.frame.Y / frameHeight;
                int num17 = num237 - num267;
                if ((uint)(num17 - 1) > 1u && (uint)(num17 - 4) > 1u && num267 != 0) {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0.0;
                }

                bool flag12 = NPC.ai[0] == 3f;
                int num268 = 0;
                int num269 = 0;
                int num270 = -1;
                int num271 = -1;
                if (NPC.frameCounter < 10.0)
                    num268 = 0;
                else if (NPC.frameCounter < 16.0)
                    num268 = num237 - 5;
                else if (NPC.frameCounter < 46.0)
                    num268 = num237 - 4;
                else if (NPC.frameCounter < 60.0)
                    num268 = num237 - 5;
                else if (NPC.frameCounter < 216.0)
                    num268 = 0;
                else if (NPC.frameCounter == 216.0 && Main.netMode != 1)
                    num270 = 70;
                else if (NPC.frameCounter < 286.0)
                    num268 = ((NPC.frameCounter % 12.0 < 6.0) ? (num237 - 2) : 0);
                else if (NPC.frameCounter < 320.0)
                    num268 = 0;
                else if (NPC.frameCounter != 320.0 || Main.netMode == 1)
                    num268 = ((NPC.frameCounter < 420.0) ? ((NPC.frameCounter % 16.0 < 8.0) ? (num237 - 2) : 0) : 0);
                else
                    num270 = 100;

                if (NPC.frameCounter < 70.0)
                    num269 = 0;
                else if (NPC.frameCounter != 70.0 || Main.netMode == 1)
                    num269 = NPC.frameCounter < 160.0 ? NPC.frameCounter % 16.0 < 8.0 ? num237 - 2 : 0 : NPC.frameCounter < 166.0 ? num237 - 5 : NPC.frameCounter < 186.0 ? num237 - 4 : NPC.frameCounter < 200.0 ? num237 - 5 : NPC.frameCounter >= 320.0 ? NPC.frameCounter < 326.0 ? num237 - 1 : 0 : 0;
                else
                    num271 = 90;

                if (flag12) {
                    NPC nPC = Main.npc[(int)NPC.ai[2]];
                    if (num271 != -1 && nPC.CanTalk)
                        EmoteBubble.NewBubbleNPC(new(nPC), num271, new(NPC));
                    else if (num270 != -1)
                        EmoteBubble.NewBubbleNPC(new(NPC), num270, new(nPC));
                }

                NPC.frame.Y = frameHeight * (flag12 ? num268 : num269);
                if (NPC.frameCounter >= 420.0)
                    NPC.frameCounter = 0.0;
            } else if (NPC.velocity.X == 0f) {
                NPC.frame.Y = 0;
                NPC.frameCounter = 0.0;
            } else {
                int num287 = 6;
                NPC.frameCounter += Math.Abs(NPC.velocity.X) * 2f;
                NPC.frameCounter += 1.0;

                int num288 = frameHeight * 2;
                if (NPC.frame.Y < num288)
                    NPC.frame.Y = num288;

                if (NPC.frameCounter > num287) {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0.0;
                }

                if (NPC.frame.Y / frameHeight >= Main.npcFrameCount[Type])
                    NPC.frame.Y = num288;
            }
            return;
        }

        NPC.frameCounter = 0.0;
        NPC.frame.Y = frameHeight;
        
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => ZombieApocalypseConfig.GetInstance().EvenMoreZomb && !Main.IsItDay() && (spawnInfo.Player.ZoneDesert || spawnInfo.Player.ZoneUndergroundDesert) && Main.hardMode ? 0.3f : 0f;
}
