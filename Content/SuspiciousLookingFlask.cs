using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ZombieApocalypse.Common;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse.Content;

public class SuspiciousLookingFlask : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 1;

        ItemID.Sets.DrinkParticleColors[Type] = [
            new(240, 240, 240),
            new(200, 200, 200),
            new(140, 140, 140)
        ];
    }

    public override void SetDefaults() {
        Item.consumable = true;
        Item.value = Item.buyPrice(gold: -25);
        Item.stack = 1;
        Item.width = 20;
        Item.height = 30;
        Item.rare = ModContent.RarityType<ZombieRarity>();
        Item.useAnimation = 30;
        Item.useTime = 30;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useTurn = true;
        Item.UseSound = SoundID.Item3;
    }

    public override bool? UseItem(Player player) {
        if (player.ItemAnimationJustStarted && !player.IsZombie() && player.whoAmI == Main.myPlayer && ZombieApocalypseConfig.GetInstance().EnableSuspiciousLookingFlask) {
            player.SetZombie(true);
            player.GetModPlayer<ZombifiablePlayer>().ClientHandleZombification();
        }
        return true;
    }

    public override void AddRecipes() {
        if (ZombieApocalypseConfig.GetInstance().EnableSuspiciousLookingFlask)
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.GlowingMushroom, 20)
                .AddIngredient(ItemID.Acorn, 7)
                .AddIngredient(ItemID.Gel, 2)
                .AddIngredient(ItemID.ZombieArm)
                .AddRecipeGroup(RecipeGroupID.Fruit)
                .AddTile(TileID.Bottles)
                .Register();
    }
}
