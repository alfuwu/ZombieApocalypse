using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse.Common.Hooks;

public class ItemSlotHooks : ModHook {
    public static bool CanUseSlot(Item[] inv, int slot) => inv != Main.LocalPlayer.inventory || !Main.LocalPlayer.IsZombie() || slot < 5 || slot >= 50 && ZombieApocalypseConfig.GetInstance().ZombiesCanUseAmmoAndCoinSlots;

    public override void Apply() {
        On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += OnDraw;
        On_ItemSlot.LeftClick_ItemArray_int_int += OnLeftClick;
        On_ItemSlot.RightClick_ItemArray_int_int += OnRightClick;
    }

    public override void Unapply() {
        On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color -= OnDraw;
        On_ItemSlot.LeftClick_ItemArray_int_int -= OnLeftClick;
        On_ItemSlot.RightClick_ItemArray_int_int -= OnRightClick;
    }

    private void OnDraw(On_ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor) {
        Color o = Main.inventoryBack;
        if (!CanUseSlot(inv, slot))
            Main.inventoryBack = new(80, 80, 80, 80); // why is this a field
        orig(spriteBatch, inv, context, slot, position, lightColor);
        Main.inventoryBack = o; // i mean i'll take it, makes this very simple
        // but holy fuck it took forever to figure out that i needed to change it
    }

    private void OnLeftClick(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
        if (CanUseSlot(inv, slot) || Main.mouseItem.IsAir)
            orig(inv, context, slot);
    }

    private void OnRightClick(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
        if (CanUseSlot(inv, slot))
            orig(inv, context, slot);
    }
}
