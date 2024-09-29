using MonoMod.Cil;
using System.IO;
using Terraria.ModLoader;

namespace ZombieApocalypse.Common.Hooks;

public abstract class ModHook : ModType { // most useless OOP class
    public sealed override void SetupContent() => Apply();
    public override void Unload() => Unapply();

    protected sealed override void Register() => ModTypeLookup<ModHook>.Register(this);

    /// <summary>
    /// Runs during mod loading
    /// </summary>
    public abstract void Apply();

    /// <summary>
    /// Runs during mod unloading, should be used to unregister hooks that were added in <see cref="Apply"/>
    /// </summary>
    public abstract void Unapply();

    internal void DumpIL(ILContext il) {
        string methodName = il.Method.Name.Replace(':', '_');
        if (methodName.Contains('?')) // MonoMod IL copies are created with mangled names like DMD<Terraria.Player::beeType>?38504011::Terraria.Player::beeType(Terraria.Player)
            methodName = methodName[(methodName.LastIndexOf('?') + 1)..];

        string filePath = Path.Combine(Logging.LogDir, "ILDumps", Mod.Name, methodName + ".txt");
        string folderPath = Path.GetDirectoryName(filePath);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        File.WriteAllText(filePath, il.ToString());
    }
}
