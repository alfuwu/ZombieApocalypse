using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.UI;
using ZombieApocalypse.Common.Extensions;

namespace ZombieApocalypse.Common.Hooks;

public class UICharacterSelectHook : ModHook {
    public override void Apply() { // prolly doesn't work if u Alt + F4 in the Character Select screen and then disable this mod
        // but surely no one would do that
        On_UICharacterSelect.GoBackClick += OnGoBackClick;
    }

    public override void Unapply() {
        On_UICharacterSelect.GoBackClick -= OnGoBackClick;
    }

    private void OnGoBackClick(On_UICharacterSelect.orig_GoBackClick orig, UICharacterSelect self, UIMouseEvent evt, UIElement listeningElement) {
        orig(self, evt, listeningElement);
        foreach (PlayerFileData p in Main.PlayerList) {
            if (p.Player.IsZombie() && ZombieApocalypseConfig.GetInstance().ZombiesHaveADifferentSkinColor) {
                p.Player.skinColor = p.Player.GetModPlayer<ZombifiablePlayer>().OriginalSkinColor;
                Player.SavePlayer(p);
            }
        }
    }
}
