using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ZombieApocalypse.Common;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse.Content;

public class HolyCure : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 1;

        ItemID.Sets.DrinkParticleColors[Type] = [
            new(239, 191, 45),
            new(237, 201, 94),
            new(234, 210, 145)
        ];
    }

    public override void SetDefaults() {
        Item.consumable = true;
        Item.value = Item.buyPrice(silver: 25);
        Item.stack = 1;
        Item.width = 20;
        Item.height = 30;
        Item.rare = ModContent.RarityType<ZombieRarity>();
        Item.useAnimation = 30;
        Item.useTime = 30;
        Item.useStyle = ItemUseStyleID.DrinkLong;
        Item.useTurn = true;
        Item.UseSound = SoundID.Item3;
    }

    public override bool? UseItem(Player player) {
        if (player.ItemAnimationJustStarted && player.IsZombie() && player.whoAmI == Main.myPlayer && Main.rand.NextFloat() < 0.99f && ZombieApocalypseConfig.GetInstance().EnableHolyCure) { // gotta stay lore accurate yk?
            player.SetZombie(false);
            player.GetModPlayer<ZombifiablePlayer>().ClientHandleZombification();
        }
        return true;
    }

    public override void AddRecipes() {
        if (ZombieApocalypseConfig.GetInstance().EnableSuspiciousLookingFlask)
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Blinkroot, 20)
                .AddIngredient(ItemID.Daybloom, 20)
                .AddIngredient(ItemID.Deathweed, 20)
                .AddIngredient(ItemID.Fireblossom, 20)
                .AddIngredient(ItemID.Moonglow, 20)
                .AddIngredient(ItemID.Shiverthorn, 20)
                .AddIngredient(ItemID.Blinkroot, 20)
                .AddIngredient(ItemID.Waterleaf, 20)
                .AddTile(TileID.LunarCraftingStation)
                .AddTile(TileID.Bottles)
                .Register();
    }
}
